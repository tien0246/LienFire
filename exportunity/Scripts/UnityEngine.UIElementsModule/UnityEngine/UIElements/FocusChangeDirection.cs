using System;

namespace UnityEngine.UIElements;

public class FocusChangeDirection : IDisposable
{
	private readonly int m_Value;

	public static FocusChangeDirection unspecified { get; } = new FocusChangeDirection(-1);

	public static FocusChangeDirection none { get; } = new FocusChangeDirection(0);

	protected static FocusChangeDirection lastValue { get; } = none;

	protected FocusChangeDirection(int value)
	{
		m_Value = value;
	}

	public static implicit operator int(FocusChangeDirection fcd)
	{
		return fcd?.m_Value ?? 0;
	}

	void IDisposable.Dispose()
	{
		Dispose();
	}

	protected virtual void Dispose()
	{
	}

	internal virtual void ApplyTo(FocusController focusController, Focusable f)
	{
		focusController.SwitchFocus(f, this);
	}
}
