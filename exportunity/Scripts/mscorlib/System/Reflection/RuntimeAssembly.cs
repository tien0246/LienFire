using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;

namespace System.Reflection;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
[ComVisible(true)]
[ComDefaultInterface(typeof(_Assembly))]
[ClassInterface(ClassInterfaceType.None)]
internal class RuntimeAssembly : Assembly
{
	internal class UnmanagedMemoryStreamForModule : UnmanagedMemoryStream
	{
		private Module module;

		public unsafe UnmanagedMemoryStreamForModule(byte* pointer, long length, Module module)
			: base(pointer, length)
		{
			this.module = module;
		}

		protected override void Dispose(bool disposing)
		{
			if (_isOpen)
			{
				module = null;
			}
			base.Dispose(disposing);
		}
	}

	internal IntPtr _mono_assembly;

	internal Evidence _evidence;

	internal ResolveEventHolder resolve_event_holder;

	internal PermissionSet _minimum;

	internal PermissionSet _optional;

	internal PermissionSet _refuse;

	internal PermissionSet _granted;

	internal PermissionSet _denied;

	internal bool fromByteArray;

	internal string assemblyName;

	[ComVisible(false)]
	public override Module ManifestModule => GetManifestModule();

	public override bool GlobalAssemblyCache => get_global_assembly_cache();

	public override extern MethodInfo EntryPoint
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[ComVisible(false)]
	public override extern bool ReflectionOnly
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public override string CodeBase => GetCodeBase(this, escaped: false);

	public override string EscapedCodeBase => GetCodeBase(this, escaped: true);

	public override string FullName => get_fullname(this);

	[ComVisible(false)]
	public override string ImageRuntimeVersion => InternalImageRuntimeVersion(this);

	internal override IntPtr MonoAssembly => _mono_assembly;

	internal override bool FromByteArray
	{
		set
		{
			fromByteArray = value;
		}
	}

	public override string Location
	{
		get
		{
			if (fromByteArray)
			{
				return string.Empty;
			}
			string location = get_location();
			if (location != string.Empty && SecurityManager.SecurityEnabled)
			{
				new FileIOPermission(FileIOPermissionAccess.PathDiscovery, location).Demand();
			}
			return location;
		}
	}

	public override Evidence Evidence
	{
		[SecurityPermission(SecurityAction.Demand, ControlEvidence = true)]
		get
		{
			return UnprotectedGetEvidence();
		}
	}

	internal override PermissionSet GrantedPermissionSet
	{
		get
		{
			if (_granted == null)
			{
				if (SecurityManager.ResolvingPolicyLevel != null)
				{
					if (SecurityManager.ResolvingPolicyLevel.IsFullTrustAssembly(this))
					{
						return DefaultPolicies.FullTrust;
					}
					return null;
				}
				Resolve();
			}
			return _granted;
		}
	}

	internal override PermissionSet DeniedPermissionSet
	{
		get
		{
			if (_granted == null)
			{
				if (SecurityManager.ResolvingPolicyLevel != null)
				{
					if (SecurityManager.ResolvingPolicyLevel.IsFullTrustAssembly(this))
					{
						return null;
					}
					return DefaultPolicies.FullTrust;
				}
				Resolve();
			}
			return _denied;
		}
	}

	public override PermissionSet PermissionSet => GrantedPermissionSet;

	public override event ModuleResolveEventHandler ModuleResolve
	{
		[SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
		add
		{
			resolve_event_holder.ModuleResolve += value;
		}
		[SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
		remove
		{
			resolve_event_holder.ModuleResolve -= value;
		}
	}

	protected RuntimeAssembly()
	{
		resolve_event_holder = new ResolveEventHolder();
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		UnitySerializationHolder.GetUnitySerializationInfo(info, 6, FullName, this);
	}

	internal static RuntimeAssembly GetExecutingAssembly(ref StackCrawlMark stackMark)
	{
		throw new NotSupportedException();
	}

	[SecurityCritical]
	internal static AssemblyName CreateAssemblyName(string assemblyString, bool forIntrospection, out RuntimeAssembly assemblyFromResolveEvent)
	{
		if (assemblyString == null)
		{
			throw new ArgumentNullException("assemblyString");
		}
		if (assemblyString.Length == 0 || assemblyString[0] == '\0')
		{
			throw new ArgumentException(Environment.GetResourceString("String cannot have zero length."));
		}
		if (forIntrospection)
		{
			AppDomain.CheckReflectionOnlyLoadSupported();
		}
		AssemblyName result = new AssemblyName
		{
			Name = assemblyString
		};
		assemblyFromResolveEvent = null;
		return result;
	}

	internal static RuntimeAssembly InternalLoadAssemblyName(AssemblyName assemblyRef, Evidence assemblySecurity, RuntimeAssembly reqAssembly, ref StackCrawlMark stackMark, bool throwOnFileNotFound, bool forIntrospection, bool suppressSecurityChecks)
	{
		if (assemblyRef == null)
		{
			throw new ArgumentNullException("assemblyRef");
		}
		if (assemblyRef.CodeBase != null)
		{
			AppDomain.CheckLoadFromSupported();
		}
		assemblyRef = (AssemblyName)assemblyRef.Clone();
		if (assemblySecurity != null)
		{
		}
		return (RuntimeAssembly)Assembly.Load(assemblyRef);
	}

	internal static RuntimeAssembly LoadWithPartialNameInternal(string partialName, Evidence securityEvidence, ref StackCrawlMark stackMark)
	{
		return (RuntimeAssembly)Assembly.LoadWithPartialName(partialName, securityEvidence);
	}

	internal static RuntimeAssembly LoadWithPartialNameInternal(AssemblyName an, Evidence securityEvidence, ref StackCrawlMark stackMark)
	{
		return LoadWithPartialNameInternal(an.ToString(), securityEvidence, ref stackMark);
	}

	public override AssemblyName GetName(bool copiedName)
	{
		if (SecurityManager.SecurityEnabled)
		{
			_ = CodeBase;
		}
		return AssemblyName.Create(this, fillCodebase: true);
	}

	public override Type GetType(string name, bool throwOnError, bool ignoreCase)
	{
		if (name == null)
		{
			throw new ArgumentNullException(name);
		}
		if (name.Length == 0)
		{
			throw new ArgumentException("name", "Name cannot be empty");
		}
		return InternalGetType(null, name, throwOnError, ignoreCase);
	}

	public override Module GetModule(string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (name.Length == 0)
		{
			throw new ArgumentException("Name can't be empty");
		}
		Module[] modules = GetModules(getResourceModules: true);
		foreach (Module module in modules)
		{
			if (module.ScopeName == name)
			{
				return module;
			}
		}
		return null;
	}

	public override AssemblyName[] GetReferencedAssemblies()
	{
		return Assembly.GetReferencedAssemblies(this);
	}

	public override Module[] GetModules(bool getResourceModules)
	{
		Module[] modulesInternal = GetModulesInternal();
		if (!getResourceModules)
		{
			List<Module> list = new List<Module>(modulesInternal.Length);
			Module[] array = modulesInternal;
			foreach (Module module in array)
			{
				if (!module.IsResource())
				{
					list.Add(module);
				}
			}
			return list.ToArray();
		}
		return modulesInternal;
	}

	[MonoTODO("Always returns the same as GetModules")]
	public override Module[] GetLoadedModules(bool getResourceModules)
	{
		return GetModules(getResourceModules);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override Assembly GetSatelliteAssembly(CultureInfo culture)
	{
		StackCrawlMark stackMark = StackCrawlMark.LookForMyCaller;
		return GetSatelliteAssembly(culture, null, throwOnError: true, ref stackMark);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override Assembly GetSatelliteAssembly(CultureInfo culture, Version version)
	{
		StackCrawlMark stackMark = StackCrawlMark.LookForMyCaller;
		return GetSatelliteAssembly(culture, version, throwOnError: true, ref stackMark);
	}

	public override Type[] GetExportedTypes()
	{
		return GetTypes(exportedOnly: true);
	}

	internal static byte[] GetAotId()
	{
		byte[] array = new byte[16];
		if (GetAotIdInternal(array))
		{
			return array;
		}
		return null;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern string get_code_base(Assembly a, bool escaped);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern string get_location();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern string get_fullname(Assembly a);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool GetAotIdInternal(byte[] aotid);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern string InternalImageRuntimeVersion(Assembly a);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern bool get_global_assembly_cache();

	internal static string GetCodeBase(Assembly a, bool escaped)
	{
		string text = get_code_base(a, escaped);
		if (SecurityManager.SecurityEnabled && string.Compare("FILE://", 0, text, 0, 7, ignoreCase: true, CultureInfo.InvariantCulture) == 0)
		{
			string path = text.Substring(7);
			new FileIOPermission(FileIOPermissionAccess.PathDiscovery, path).Demand();
		}
		return text;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern bool GetManifestResourceInfoInternal(string name, ManifestResourceInfo info);

	public override ManifestResourceInfo GetManifestResourceInfo(string resourceName)
	{
		if (resourceName == null)
		{
			throw new ArgumentNullException("resourceName");
		}
		if (resourceName.Length == 0)
		{
			throw new ArgumentException("String cannot have zero length.");
		}
		ManifestResourceInfo manifestResourceInfo = new ManifestResourceInfo(null, null, (ResourceLocation)0);
		if (GetManifestResourceInfoInternal(resourceName, manifestResourceInfo))
		{
			return manifestResourceInfo;
		}
		return null;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public override extern string[] GetManifestResourceNames();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern IntPtr GetManifestResourceInternal(string name, out int size, out Module module);

	public unsafe override Stream GetManifestResourceStream(string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (name.Length == 0)
		{
			throw new ArgumentException("String cannot have zero length.", "name");
		}
		ManifestResourceInfo manifestResourceInfo = GetManifestResourceInfo(name);
		if (manifestResourceInfo == null)
		{
			Assembly assembly = AppDomain.CurrentDomain.DoResourceResolve(name, this);
			if (assembly != null && assembly != this)
			{
				return assembly.GetManifestResourceStream(name);
			}
			return null;
		}
		if (manifestResourceInfo.ReferencedAssembly != null)
		{
			return manifestResourceInfo.ReferencedAssembly.GetManifestResourceStream(name);
		}
		if (manifestResourceInfo.FileName != null && manifestResourceInfo.ResourceLocation == (ResourceLocation)0)
		{
			if (fromByteArray)
			{
				throw new FileNotFoundException(manifestResourceInfo.FileName);
			}
			return new FileStream(Path.Combine(Path.GetDirectoryName(Location), manifestResourceInfo.FileName), FileMode.Open, FileAccess.Read);
		}
		int size;
		Module module;
		IntPtr manifestResourceInternal = GetManifestResourceInternal(name, out size, out module);
		if (manifestResourceInternal == (IntPtr)0)
		{
			return null;
		}
		return new UnmanagedMemoryStreamForModule((byte*)(void*)manifestResourceInternal, size, module);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override Stream GetManifestResourceStream(Type type, string name)
	{
		StackCrawlMark stackMark = StackCrawlMark.LookForMyCaller;
		return GetManifestResourceStream(type, name, skipSecurityCheck: false, ref stackMark);
	}

	public override bool IsDefined(Type attributeType, bool inherit)
	{
		return MonoCustomAttrs.IsDefined(this, attributeType, inherit);
	}

	public override object[] GetCustomAttributes(bool inherit)
	{
		return MonoCustomAttrs.GetCustomAttributes(this, inherit);
	}

	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
	}

	public override IList<CustomAttributeData> GetCustomAttributesData()
	{
		return CustomAttributeData.GetCustomAttributes(this);
	}

	internal override Module GetManifestModule()
	{
		return GetManifestModuleInternal();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern Module GetManifestModuleInternal();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal override extern Module[] GetModulesInternal();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern object GetFilesInternal(string name, bool getResourceModules);

	public override FileStream[] GetFiles(bool getResourceModules)
	{
		string[] array = (string[])GetFilesInternal(null, getResourceModules);
		if (array == null)
		{
			return EmptyArray<FileStream>.Value;
		}
		string location = Location;
		FileStream[] array2;
		if (location != string.Empty)
		{
			array2 = new FileStream[array.Length + 1];
			array2[0] = new FileStream(location, FileMode.Open, FileAccess.Read);
			for (int i = 0; i < array.Length; i++)
			{
				array2[i + 1] = new FileStream(array[i], FileMode.Open, FileAccess.Read);
			}
		}
		else
		{
			array2 = new FileStream[array.Length];
			for (int j = 0; j < array.Length; j++)
			{
				array2[j] = new FileStream(array[j], FileMode.Open, FileAccess.Read);
			}
		}
		return array2;
	}

	public override FileStream GetFile(string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException(null, "Name cannot be null.");
		}
		if (name.Length == 0)
		{
			throw new ArgumentException("Empty name is not valid");
		}
		string text = (string)GetFilesInternal(name, getResourceModules: true);
		if (text != null)
		{
			return new FileStream(text, FileMode.Open, FileAccess.Read);
		}
		return null;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object o)
	{
		if (this == o)
		{
			return true;
		}
		if (o == null)
		{
			return false;
		}
		if (!(o is RuntimeAssembly))
		{
			return false;
		}
		return ((RuntimeAssembly)o)._mono_assembly == _mono_assembly;
	}

	public override string ToString()
	{
		if (assemblyName != null)
		{
			return assemblyName;
		}
		assemblyName = FullName;
		return assemblyName;
	}

	internal override Evidence UnprotectedGetEvidence()
	{
		if (_evidence == null)
		{
			lock (this)
			{
				_evidence = Evidence.GetDefaultHostEvidence(this);
			}
		}
		return _evidence;
	}

	internal void Resolve()
	{
		lock (this)
		{
			LoadAssemblyPermissions();
			Evidence evidence = new Evidence(UnprotectedGetEvidence());
			evidence.AddHost(new PermissionRequestEvidence(_minimum, _optional, _refuse));
			_granted = SecurityManager.ResolvePolicy(evidence, _minimum, _optional, _refuse, out _denied);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool LoadPermissions(Assembly a, ref IntPtr minimum, ref int minLength, ref IntPtr optional, ref int optLength, ref IntPtr refused, ref int refLength);

	private void LoadAssemblyPermissions()
	{
		IntPtr minimum = IntPtr.Zero;
		IntPtr optional = IntPtr.Zero;
		IntPtr refused = IntPtr.Zero;
		int minLength = 0;
		int optLength = 0;
		int refLength = 0;
		if (LoadPermissions(this, ref minimum, ref minLength, ref optional, ref optLength, ref refused, ref refLength))
		{
			if (minLength > 0)
			{
				byte[] array = new byte[minLength];
				Marshal.Copy(minimum, array, 0, minLength);
				_minimum = SecurityManager.Decode(array);
			}
			if (optLength > 0)
			{
				byte[] array2 = new byte[optLength];
				Marshal.Copy(optional, array2, 0, optLength);
				_optional = SecurityManager.Decode(array2);
			}
			if (refLength > 0)
			{
				byte[] array3 = new byte[refLength];
				Marshal.Copy(refused, array3, 0, refLength);
				_refuse = SecurityManager.Decode(array3);
			}
		}
	}
}
