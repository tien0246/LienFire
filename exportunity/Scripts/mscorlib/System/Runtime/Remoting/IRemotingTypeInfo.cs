using System.Runtime.InteropServices;

namespace System.Runtime.Remoting;

[ComVisible(true)]
public interface IRemotingTypeInfo
{
	string TypeName { get; set; }

	bool CanCastTo(Type fromType, object o);
}
