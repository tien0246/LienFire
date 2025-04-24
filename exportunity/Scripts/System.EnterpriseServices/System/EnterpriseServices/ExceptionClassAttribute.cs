using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[AttributeUsage(AttributeTargets.Class)]
[ComVisible(false)]
public sealed class ExceptionClassAttribute : Attribute
{
	private string name;

	public string Value => name;

	public ExceptionClassAttribute(string name)
	{
		this.name = name;
	}
}
