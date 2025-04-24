using System.Collections;
using System.Runtime.InteropServices;
using Unity;

namespace System.Security.Permissions;

[Serializable]
[ComVisible(true)]
public sealed class KeyContainerPermissionAccessEntryEnumerator : IEnumerator
{
	private IEnumerator e;

	public KeyContainerPermissionAccessEntry Current => (KeyContainerPermissionAccessEntry)e.Current;

	object IEnumerator.Current => e.Current;

	internal KeyContainerPermissionAccessEntryEnumerator(ArrayList list)
	{
		e = list.GetEnumerator();
	}

	public bool MoveNext()
	{
		return e.MoveNext();
	}

	public void Reset()
	{
		e.Reset();
	}

	internal KeyContainerPermissionAccessEntryEnumerator()
	{
		ThrowStub.ThrowNotSupportedException();
	}
}
