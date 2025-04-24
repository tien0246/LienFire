namespace System.Runtime.InteropServices;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
public sealed class PrimaryInteropAssemblyAttribute : Attribute
{
	internal int _major;

	internal int _minor;

	public int MajorVersion => _major;

	public int MinorVersion => _minor;

	public PrimaryInteropAssemblyAttribute(int major, int minor)
	{
		_major = major;
		_minor = minor;
	}
}
