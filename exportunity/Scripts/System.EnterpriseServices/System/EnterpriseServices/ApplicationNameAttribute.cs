using System.Collections;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[AttributeUsage(AttributeTargets.Assembly)]
[ComVisible(false)]
public sealed class ApplicationNameAttribute : Attribute, IConfigurationAttribute
{
	private string name;

	public string Value => name;

	public ApplicationNameAttribute(string name)
	{
		this.name = name;
	}

	bool IConfigurationAttribute.AfterSaveChanges(Hashtable info)
	{
		return false;
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
