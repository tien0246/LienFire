using System.Runtime.InteropServices;
using Unity;

namespace System.Reflection.Emit;

[StructLayout(LayoutKind.Sequential)]
[ComVisible(true)]
[ComDefaultInterface(typeof(_LocalBuilder))]
[ClassInterface(ClassInterfaceType.None)]
public sealed class LocalBuilder : LocalVariableInfo, _LocalBuilder
{
	private string name;

	internal ILGenerator ilgen;

	private int startOffset;

	private int endOffset;

	public override Type LocalType => type;

	public override bool IsPinned => is_pinned;

	public override int LocalIndex => position;

	internal string Name => name;

	internal int StartOffset => startOffset;

	internal int EndOffset => endOffset;

	void _LocalBuilder.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
	{
		throw new NotImplementedException();
	}

	void _LocalBuilder.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
	{
		throw new NotImplementedException();
	}

	void _LocalBuilder.GetTypeInfoCount(out uint pcTInfo)
	{
		throw new NotImplementedException();
	}

	void _LocalBuilder.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
	{
		throw new NotImplementedException();
	}

	internal LocalBuilder(Type t, ILGenerator ilgen)
	{
		type = t;
		this.ilgen = ilgen;
	}

	public void SetLocalSymInfo(string name, int startOffset, int endOffset)
	{
		this.name = name;
		this.startOffset = startOffset;
		this.endOffset = endOffset;
	}

	public void SetLocalSymInfo(string name)
	{
		SetLocalSymInfo(name, 0, 0);
	}

	internal LocalBuilder()
	{
		ThrowStub.ThrowNotSupportedException();
	}
}
