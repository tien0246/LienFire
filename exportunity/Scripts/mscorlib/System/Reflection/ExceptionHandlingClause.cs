using System.Runtime.InteropServices;

namespace System.Reflection;

[StructLayout(LayoutKind.Sequential)]
[ComVisible(true)]
public class ExceptionHandlingClause
{
	internal Type catch_type;

	internal int filter_offset;

	internal ExceptionHandlingClauseOptions flags;

	internal int try_offset;

	internal int try_length;

	internal int handler_offset;

	internal int handler_length;

	public virtual Type CatchType => catch_type;

	public virtual int FilterOffset => filter_offset;

	public virtual ExceptionHandlingClauseOptions Flags => flags;

	public virtual int HandlerLength => handler_length;

	public virtual int HandlerOffset => handler_offset;

	public virtual int TryLength => try_length;

	public virtual int TryOffset => try_offset;

	protected ExceptionHandlingClause()
	{
	}

	public override string ToString()
	{
		string text = $"Flags={flags}, TryOffset={try_offset}, TryLength={try_length}, HandlerOffset={handler_offset}, HandlerLength={handler_length}";
		if (catch_type != null)
		{
			text = $"{text}, CatchType={catch_type}";
		}
		if (flags == ExceptionHandlingClauseOptions.Filter)
		{
			text = $"{text}, FilterOffset={filter_offset}";
		}
		return text;
	}
}
