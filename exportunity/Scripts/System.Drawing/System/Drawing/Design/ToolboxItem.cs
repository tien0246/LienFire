using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Unity;

namespace System.Drawing.Design;

[Serializable]
[System.MonoTODO("Implementation is incomplete.")]
[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
[PermissionSet(SecurityAction.InheritanceDemand, Unrestricted = true)]
public class ToolboxItem : ISerializable
{
	private bool locked;

	private Hashtable properties = new Hashtable();

	public AssemblyName AssemblyName
	{
		get
		{
			return (AssemblyName)properties["AssemblyName"];
		}
		set
		{
			SetValue("AssemblyName", value);
		}
	}

	public Bitmap Bitmap
	{
		get
		{
			return (Bitmap)properties["Bitmap"];
		}
		set
		{
			SetValue("Bitmap", value);
		}
	}

	public string DisplayName
	{
		get
		{
			return GetValue("DisplayName");
		}
		set
		{
			SetValue("DisplayName", value);
		}
	}

	public ICollection Filter
	{
		get
		{
			ICollection collection = (ICollection)properties["Filter"];
			if (collection == null)
			{
				collection = new ToolboxItemFilterAttribute[0];
			}
			return collection;
		}
		set
		{
			SetValue("Filter", value);
		}
	}

	public virtual bool Locked => locked;

	public string TypeName
	{
		get
		{
			return GetValue("TypeName");
		}
		set
		{
			SetValue("TypeName", value);
		}
	}

	public string Company
	{
		get
		{
			return (string)properties["Company"];
		}
		set
		{
			SetValue("Company", value);
		}
	}

	public virtual string ComponentType => ".NET Component";

	public AssemblyName[] DependentAssemblies
	{
		get
		{
			return (AssemblyName[])properties["DependentAssemblies"];
		}
		set
		{
			AssemblyName[] array = new AssemblyName[value.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = value[i];
			}
			SetValue("DependentAssemblies", array);
		}
	}

	public string Description
	{
		get
		{
			return (string)properties["Description"];
		}
		set
		{
			SetValue("Description", value);
		}
	}

	public bool IsTransient
	{
		get
		{
			object obj = properties["IsTransient"];
			if (obj != null)
			{
				return (bool)obj;
			}
			return false;
		}
		set
		{
			SetValue("IsTransient", value);
		}
	}

	public IDictionary Properties => properties;

	public virtual string Version => string.Empty;

	public Bitmap OriginalBitmap
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}
		set
		{
			Unity.ThrowStub.ThrowNotSupportedException();
		}
	}

	public event ToolboxComponentsCreatedEventHandler ComponentsCreated;

	public event ToolboxComponentsCreatingEventHandler ComponentsCreating;

	public ToolboxItem()
	{
	}

	public ToolboxItem(Type toolType)
	{
		Initialize(toolType);
	}

	protected void CheckUnlocked()
	{
		if (locked)
		{
			throw new InvalidOperationException("The ToolboxItem is locked");
		}
	}

	public IComponent[] CreateComponents()
	{
		return CreateComponents(null);
	}

	public IComponent[] CreateComponents(IDesignerHost host)
	{
		OnComponentsCreating(new ToolboxComponentsCreatingEventArgs(host));
		IComponent[] array = CreateComponentsCore(host);
		OnComponentsCreated(new ToolboxComponentsCreatedEventArgs(array));
		return array;
	}

	protected virtual IComponent[] CreateComponentsCore(IDesignerHost host)
	{
		if (host == null)
		{
			throw new ArgumentNullException("host");
		}
		Type type = GetType(host, AssemblyName, TypeName, reference: true);
		if (type == null)
		{
			return new IComponent[0];
		}
		return new IComponent[1] { host.CreateComponent(type) };
	}

	protected virtual IComponent[] CreateComponentsCore(IDesignerHost host, IDictionary defaultValues)
	{
		IComponent[] array = CreateComponentsCore(host);
		IComponent[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Component component = (Component)array2[i];
			(host.GetDesigner(component) as IComponentInitializer).InitializeNewComponent(defaultValues);
		}
		return array;
	}

	public IComponent[] CreateComponents(IDesignerHost host, IDictionary defaultValues)
	{
		OnComponentsCreating(new ToolboxComponentsCreatingEventArgs(host));
		IComponent[] array = CreateComponentsCore(host, defaultValues);
		OnComponentsCreated(new ToolboxComponentsCreatedEventArgs(array));
		return array;
	}

	protected virtual object FilterPropertyValue(string propertyName, object value)
	{
		switch (propertyName)
		{
		case "AssemblyName":
			if (value != null)
			{
				return (value as ICloneable).Clone();
			}
			return null;
		case "DisplayName":
		case "TypeName":
			if (value != null)
			{
				return value;
			}
			return string.Empty;
		case "Filter":
			if (value != null)
			{
				return value;
			}
			return new ToolboxItemFilterAttribute[0];
		default:
			return value;
		}
	}

	protected virtual void Deserialize(SerializationInfo info, StreamingContext context)
	{
		AssemblyName = (AssemblyName)info.GetValue("AssemblyName", typeof(AssemblyName));
		Bitmap = (Bitmap)info.GetValue("Bitmap", typeof(Bitmap));
		Filter = (ICollection)info.GetValue("Filter", typeof(ICollection));
		DisplayName = info.GetString("DisplayName");
		locked = info.GetBoolean("Locked");
		TypeName = info.GetString("TypeName");
	}

	public override bool Equals(object obj)
	{
		if (!(obj is ToolboxItem toolboxItem))
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		if (toolboxItem.AssemblyName.Equals(AssemblyName) && toolboxItem.Locked.Equals(locked) && toolboxItem.TypeName.Equals(TypeName) && toolboxItem.DisplayName.Equals(DisplayName))
		{
			return toolboxItem.Bitmap.Equals(Bitmap);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (TypeName + DisplayName).GetHashCode();
	}

	public Type GetType(IDesignerHost host)
	{
		return GetType(host, AssemblyName, TypeName, reference: false);
	}

	protected virtual Type GetType(IDesignerHost host, AssemblyName assemblyName, string typeName, bool reference)
	{
		if (typeName == null)
		{
			throw new ArgumentNullException("typeName");
		}
		if (host == null)
		{
			return null;
		}
		ITypeResolutionService typeResolutionService = host.GetService(typeof(ITypeResolutionService)) as ITypeResolutionService;
		Type result = null;
		if (typeResolutionService != null)
		{
			typeResolutionService.GetAssembly(assemblyName, throwOnError: true);
			if (reference)
			{
				typeResolutionService.ReferenceAssembly(assemblyName);
			}
			result = typeResolutionService.GetType(typeName, throwOnError: true);
		}
		else
		{
			Assembly assembly = Assembly.Load(assemblyName);
			if (assembly != null)
			{
				result = assembly.GetType(typeName);
			}
		}
		return result;
	}

	public virtual void Initialize(Type type)
	{
		CheckUnlocked();
		if (type == null)
		{
			return;
		}
		AssemblyName = type.Assembly.GetName();
		DisplayName = type.Name;
		TypeName = type.FullName;
		Image image = null;
		object[] customAttributes = type.GetCustomAttributes(inherit: true);
		for (int i = 0; i < customAttributes.Length; i++)
		{
			if (customAttributes[i] is ToolboxBitmapAttribute toolboxBitmapAttribute)
			{
				image = toolboxBitmapAttribute.GetImage(type);
				break;
			}
		}
		if (image == null)
		{
			image = ToolboxBitmapAttribute.GetImageFromResource(type, null, large: false);
		}
		if (image != null)
		{
			Bitmap = image as Bitmap;
			if (Bitmap == null)
			{
				Bitmap = new Bitmap(image);
			}
		}
		Filter = type.GetCustomAttributes(typeof(ToolboxItemFilterAttribute), inherit: true);
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		Serialize(info, context);
	}

	public virtual void Lock()
	{
		locked = true;
	}

	protected virtual void OnComponentsCreated(ToolboxComponentsCreatedEventArgs args)
	{
		if (this.ComponentsCreated != null)
		{
			this.ComponentsCreated(this, args);
		}
	}

	protected virtual void OnComponentsCreating(ToolboxComponentsCreatingEventArgs args)
	{
		if (this.ComponentsCreating != null)
		{
			this.ComponentsCreating(this, args);
		}
	}

	protected virtual void Serialize(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("AssemblyName", AssemblyName);
		info.AddValue("Bitmap", Bitmap);
		info.AddValue("Filter", Filter);
		info.AddValue("DisplayName", DisplayName);
		info.AddValue("Locked", locked);
		info.AddValue("TypeName", TypeName);
	}

	public override string ToString()
	{
		return DisplayName;
	}

	protected void ValidatePropertyType(string propertyName, object value, Type expectedType, bool allowNull)
	{
		if (!allowNull && value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (value != null && !expectedType.Equals(value.GetType()))
		{
			throw new ArgumentException(global::Locale.GetText("Type mismatch between value ({0}) and expected type ({1}).", value.GetType(), expectedType), "value");
		}
	}

	protected virtual object ValidatePropertyValue(string propertyName, object value)
	{
		switch (propertyName)
		{
		case "AssemblyName":
			ValidatePropertyType(propertyName, value, typeof(AssemblyName), allowNull: true);
			break;
		case "Bitmap":
			ValidatePropertyType(propertyName, value, typeof(Bitmap), allowNull: true);
			break;
		case "Company":
		case "Description":
		case "DisplayName":
		case "TypeName":
			ValidatePropertyType(propertyName, value, typeof(string), allowNull: true);
			if (value == null)
			{
				value = string.Empty;
			}
			break;
		case "IsTransient":
			ValidatePropertyType(propertyName, value, typeof(bool), allowNull: false);
			break;
		case "Filter":
			ValidatePropertyType(propertyName, value, typeof(ToolboxItemFilterAttribute[]), allowNull: true);
			if (value == null)
			{
				value = new ToolboxItemFilterAttribute[0];
			}
			break;
		case "DependentAssemblies":
			ValidatePropertyType(propertyName, value, typeof(AssemblyName[]), allowNull: true);
			break;
		}
		return value;
	}

	private void SetValue(string propertyName, object value)
	{
		CheckUnlocked();
		properties[propertyName] = ValidatePropertyValue(propertyName, value);
	}

	private string GetValue(string propertyName)
	{
		string text = (string)properties[propertyName];
		if (text != null)
		{
			return text;
		}
		return string.Empty;
	}
}
