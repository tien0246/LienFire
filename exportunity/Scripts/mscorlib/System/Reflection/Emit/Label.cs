using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

[Serializable]
[ComVisible(true)]
public readonly struct Label : IEquatable<Label>
{
	internal readonly int label;

	internal Label(int val)
	{
		label = val;
	}

	public override bool Equals(object obj)
	{
		bool flag = obj is Label;
		if (flag)
		{
			Label label = (Label)obj;
			flag = this.label == label.label;
		}
		return flag;
	}

	public bool Equals(Label obj)
	{
		return label == obj.label;
	}

	public static bool operator ==(Label a, Label b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(Label a, Label b)
	{
		return !(a == b);
	}

	public override int GetHashCode()
	{
		return label.GetHashCode();
	}
}
