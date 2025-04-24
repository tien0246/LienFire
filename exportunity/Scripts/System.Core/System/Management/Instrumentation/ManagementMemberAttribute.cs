using System.Security.Permissions;
using Unity;

namespace System.Management.Instrumentation;

[AttributeUsage(AttributeTargets.All)]
[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
public abstract class ManagementMemberAttribute : Attribute
{
	public string Name
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
}
