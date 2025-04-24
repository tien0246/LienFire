using System.Security.Principal;

namespace Microsoft.SqlServer.Server;

public sealed class SqlContext
{
	public static bool IsAvailable => false;

	public static SqlPipe Pipe => null;

	public static SqlTriggerContext TriggerContext => null;

	public static WindowsIdentity WindowsIdentity => null;
}
