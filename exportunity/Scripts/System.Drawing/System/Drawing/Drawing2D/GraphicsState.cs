using Unity;

namespace System.Drawing.Drawing2D;

public sealed class GraphicsState : MarshalByRefObject
{
	internal int nativeState;

	internal GraphicsState(int nativeState)
	{
		this.nativeState = nativeState;
	}

	internal GraphicsState()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
