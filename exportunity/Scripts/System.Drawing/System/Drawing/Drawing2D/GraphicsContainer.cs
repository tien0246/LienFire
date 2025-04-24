using Unity;

namespace System.Drawing.Drawing2D;

public sealed class GraphicsContainer : MarshalByRefObject
{
	private uint nativeState;

	internal uint NativeObject => nativeState;

	internal GraphicsContainer(uint state)
	{
		nativeState = state;
	}

	internal GraphicsContainer()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
