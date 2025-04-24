using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32;

namespace System.Diagnostics;

[StructLayout(LayoutKind.Sequential)]
[TypeConverter(typeof(ExpandableObjectConverter))]
[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
[HostProtection(SecurityAction.LinkDemand, SharedState = true, SelfAffectingProcessMgmt = true)]
public sealed class ProcessStartInfo
{
	private string fileName;

	private string arguments;

	private string directory;

	private string verb;

	private ProcessWindowStyle windowStyle;

	private bool errorDialog;

	private IntPtr errorDialogParentHandle;

	private bool useShellExecute = true;

	private string userName;

	private string domain;

	private SecureString password;

	private string passwordInClearText;

	private bool loadUserProfile;

	private bool redirectStandardInput;

	private bool redirectStandardOutput;

	private bool redirectStandardError;

	private Encoding standardOutputEncoding;

	private Encoding standardErrorEncoding;

	private bool createNoWindow;

	private WeakReference weakParentProcess;

	internal StringDictionary environmentVariables;

	private static readonly string[] empty = new string[0];

	private Collection<string> _argumentList;

	private IDictionary<string, string> environment;

	public Collection<string> ArgumentList
	{
		get
		{
			if (_argumentList == null)
			{
				_argumentList = new Collection<string>();
			}
			return _argumentList;
		}
	}

	[MonitoringDescription("The verb to apply to the document specified by the FileName property.")]
	[NotifyParentProperty(true)]
	[DefaultValue("")]
	[TypeConverter("System.Diagnostics.Design.VerbConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public string Verb
	{
		get
		{
			if (verb == null)
			{
				return string.Empty;
			}
			return verb;
		}
		set
		{
			verb = value;
		}
	}

	[DefaultValue("")]
	[SettingsBindable(true)]
	[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[NotifyParentProperty(true)]
	[MonitoringDescription("Command line arguments that will be passed to the application specified by the FileName property.")]
	public string Arguments
	{
		get
		{
			if (arguments == null)
			{
				return string.Empty;
			}
			return arguments;
		}
		set
		{
			arguments = value;
		}
	}

	[DefaultValue(false)]
	[MonitoringDescription("Whether to start the process without creating a new window to contain it.")]
	[NotifyParentProperty(true)]
	public bool CreateNoWindow
	{
		get
		{
			return createNoWindow;
		}
		set
		{
			createNoWindow = value;
		}
	}

	[NotifyParentProperty(true)]
	[Editor("System.Diagnostics.Design.StringDictionaryEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[MonitoringDescription("Set of environment variables that apply to this process and child processes.")]
	[DefaultValue(null)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public StringDictionary EnvironmentVariables
	{
		get
		{
			if (environmentVariables == null)
			{
				environmentVariables = new CaseSensitiveStringDictionary();
				if (weakParentProcess == null || !weakParentProcess.IsAlive || ((Component)weakParentProcess.Target).Site == null || !((Component)weakParentProcess.Target).Site.DesignMode)
				{
					foreach (DictionaryEntry environmentVariable in System.Environment.GetEnvironmentVariables())
					{
						environmentVariables.Add((string)environmentVariable.Key, (string)environmentVariable.Value);
					}
				}
			}
			return environmentVariables;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[DefaultValue(null)]
	[NotifyParentProperty(true)]
	public IDictionary<string, string> Environment
	{
		get
		{
			if (environment == null)
			{
				environment = EnvironmentVariables.AsGenericDictionary();
			}
			return environment;
		}
	}

	[MonitoringDescription("Whether the process command input is read from the Process instance's StandardInput member.")]
	[DefaultValue(false)]
	[NotifyParentProperty(true)]
	public bool RedirectStandardInput
	{
		get
		{
			return redirectStandardInput;
		}
		set
		{
			redirectStandardInput = value;
		}
	}

	[NotifyParentProperty(true)]
	[MonitoringDescription("Whether the process output is written to the Process instance's StandardOutput member.")]
	[DefaultValue(false)]
	public bool RedirectStandardOutput
	{
		get
		{
			return redirectStandardOutput;
		}
		set
		{
			redirectStandardOutput = value;
		}
	}

	[NotifyParentProperty(true)]
	[MonitoringDescription("Whether the process's error output is written to the Process instance's StandardError member.")]
	[DefaultValue(false)]
	public bool RedirectStandardError
	{
		get
		{
			return redirectStandardError;
		}
		set
		{
			redirectStandardError = value;
		}
	}

	public Encoding StandardErrorEncoding
	{
		get
		{
			return standardErrorEncoding;
		}
		set
		{
			standardErrorEncoding = value;
		}
	}

	public Encoding StandardOutputEncoding
	{
		get
		{
			return standardOutputEncoding;
		}
		set
		{
			standardOutputEncoding = value;
		}
	}

	[DefaultValue(true)]
	[MonitoringDescription("Whether to use the operating system shell to start the process.")]
	[NotifyParentProperty(true)]
	public bool UseShellExecute
	{
		get
		{
			return useShellExecute;
		}
		set
		{
			useShellExecute = value;
		}
	}

	[NotifyParentProperty(true)]
	public string UserName
	{
		get
		{
			if (userName == null)
			{
				return string.Empty;
			}
			return userName;
		}
		set
		{
			userName = value;
		}
	}

	public SecureString Password
	{
		get
		{
			return password;
		}
		set
		{
			password = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public string PasswordInClearText
	{
		get
		{
			return passwordInClearText;
		}
		set
		{
			passwordInClearText = value;
		}
	}

	[NotifyParentProperty(true)]
	public string Domain
	{
		get
		{
			if (domain == null)
			{
				return string.Empty;
			}
			return domain;
		}
		set
		{
			domain = value;
		}
	}

	[NotifyParentProperty(true)]
	public bool LoadUserProfile
	{
		get
		{
			return loadUserProfile;
		}
		set
		{
			loadUserProfile = value;
		}
	}

	[Editor("System.Diagnostics.Design.StartFileNameEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DefaultValue("")]
	[MonitoringDescription("The name of the application, document or URL to start.")]
	[SettingsBindable(true)]
	[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[NotifyParentProperty(true)]
	public string FileName
	{
		get
		{
			if (fileName == null)
			{
				return string.Empty;
			}
			return fileName;
		}
		set
		{
			fileName = value;
		}
	}

	[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DefaultValue("")]
	[SettingsBindable(true)]
	[Editor("System.Diagnostics.Design.WorkingDirectoryEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[MonitoringDescription("The initial working directory for the process.")]
	[NotifyParentProperty(true)]
	public string WorkingDirectory
	{
		get
		{
			if (directory == null)
			{
				return string.Empty;
			}
			return directory;
		}
		set
		{
			directory = value;
		}
	}

	[MonitoringDescription("Whether to show an error dialog to the user if there is an error.")]
	[DefaultValue(false)]
	[NotifyParentProperty(true)]
	public bool ErrorDialog
	{
		get
		{
			return errorDialog;
		}
		set
		{
			errorDialog = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IntPtr ErrorDialogParentHandle
	{
		get
		{
			return errorDialogParentHandle;
		}
		set
		{
			errorDialogParentHandle = value;
		}
	}

	[MonitoringDescription("How the main window should be created when the process starts.")]
	[DefaultValue(ProcessWindowStyle.Normal)]
	[NotifyParentProperty(true)]
	public ProcessWindowStyle WindowStyle
	{
		get
		{
			return windowStyle;
		}
		set
		{
			if (!Enum.IsDefined(typeof(ProcessWindowStyle), value))
			{
				throw new InvalidEnumArgumentException("value", (int)value, typeof(ProcessWindowStyle));
			}
			windowStyle = value;
		}
	}

	internal bool HaveEnvVars => environmentVariables != null;

	public Encoding StandardInputEncoding { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string[] Verbs
	{
		get
		{
			PlatformID platform = System.Environment.OSVersion.Platform;
			if (platform == PlatformID.Unix || platform == PlatformID.MacOSX || platform == (PlatformID)128)
			{
				return empty;
			}
			string text = (string.IsNullOrEmpty(fileName) ? null : Path.GetExtension(fileName));
			if (text == null)
			{
				return empty;
			}
			RegistryKey registryKey = null;
			RegistryKey registryKey2 = null;
			RegistryKey registryKey3 = null;
			try
			{
				registryKey = Registry.ClassesRoot.OpenSubKey(text);
				string text2 = ((registryKey != null) ? (registryKey.GetValue(null) as string) : null);
				registryKey2 = ((text2 != null) ? Registry.ClassesRoot.OpenSubKey(text2) : null);
				registryKey3 = registryKey2?.OpenSubKey("shell");
				return registryKey3?.GetSubKeyNames();
			}
			finally
			{
				registryKey3?.Close();
				registryKey2?.Close();
				registryKey?.Close();
			}
		}
	}

	public ProcessStartInfo()
	{
	}

	internal ProcessStartInfo(Process parent)
	{
		weakParentProcess = new WeakReference(parent);
	}

	public ProcessStartInfo(string fileName)
	{
		this.fileName = fileName;
	}

	public ProcessStartInfo(string fileName, string arguments)
	{
		this.fileName = fileName;
		this.arguments = arguments;
	}
}
