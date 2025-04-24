using System.Runtime.InteropServices;

namespace System.EnterpriseServices.CompensatingResourceManager;

[AttributeUsage(AttributeTargets.Assembly)]
[ComVisible(false)]
[ProgId("System.EnterpriseServices.Crm.ApplicationCrmEnabledAttribute")]
public sealed class ApplicationCrmEnabledAttribute : Attribute
{
	private bool val;

	public bool Value => val;

	public ApplicationCrmEnabledAttribute()
	{
		val = true;
	}

	public ApplicationCrmEnabledAttribute(bool val)
	{
		this.val = val;
	}
}
