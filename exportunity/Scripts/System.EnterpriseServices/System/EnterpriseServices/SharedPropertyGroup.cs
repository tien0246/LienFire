using System.Runtime.InteropServices;
using Unity;

namespace System.EnterpriseServices;

[ComVisible(false)]
public sealed class SharedPropertyGroup
{
	private ISharedPropertyGroup propertyGroup;

	internal SharedPropertyGroup(ISharedPropertyGroup propertyGroup)
	{
		this.propertyGroup = propertyGroup;
	}

	public SharedProperty CreateProperty(string name, out bool fExists)
	{
		return new SharedProperty(propertyGroup.CreateProperty(name, out fExists));
	}

	public SharedProperty CreatePropertyByPosition(int position, out bool fExists)
	{
		return new SharedProperty(propertyGroup.CreatePropertyByPosition(position, out fExists));
	}

	public SharedProperty Property(string name)
	{
		return new SharedProperty(propertyGroup.Property(name));
	}

	public SharedProperty PropertyByPosition(int position)
	{
		return new SharedProperty(propertyGroup.PropertyByPosition(position));
	}

	internal SharedPropertyGroup()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
