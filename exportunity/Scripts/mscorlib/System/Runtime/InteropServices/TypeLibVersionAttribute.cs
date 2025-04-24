namespace System.Runtime.InteropServices;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
public sealed class TypeLibVersionAttribute : Attribute
{
	internal int _major;

	internal int _minor;

	public int MajorVersion => _major;

	public int MinorVersion => _minor;

	public TypeLibVersionAttribute(int major, int minor)
	{
		_major = major;
		_minor = minor;
	}
}
