using System.Collections;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[ComVisible(false)]
public sealed class SharedPropertyGroupManager : IEnumerable
{
	[System.MonoTODO]
	public SharedPropertyGroup CreatePropertyGroup(string name, ref PropertyLockMode dwIsoMode, ref PropertyReleaseMode dwRelMode, out bool fExist)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public IEnumerator GetEnumerator()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public SharedPropertyGroup Group(string name)
	{
		throw new NotImplementedException();
	}
}
