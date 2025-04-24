using System.Collections;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[AttributeUsage(AttributeTargets.Assembly)]
[ComVisible(false)]
public sealed class ApplicationActivationAttribute : Attribute, IConfigurationAttribute
{
	private ActivationOption opt;

	private string soapMailbox;

	private string soapVRoot;

	public string SoapMailbox
	{
		get
		{
			return soapMailbox;
		}
		set
		{
			soapMailbox = value;
		}
	}

	public string SoapVRoot
	{
		get
		{
			return soapVRoot;
		}
		set
		{
			soapVRoot = value;
		}
	}

	public ActivationOption Value => opt;

	public ApplicationActivationAttribute(ActivationOption opt)
	{
		this.opt = opt;
	}

	[System.MonoTODO]
	bool IConfigurationAttribute.AfterSaveChanges(Hashtable info)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	bool IConfigurationAttribute.Apply(Hashtable cache)
	{
		throw new NotImplementedException();
	}

	bool IConfigurationAttribute.IsValidTarget(string s)
	{
		return s == "Application";
	}
}
