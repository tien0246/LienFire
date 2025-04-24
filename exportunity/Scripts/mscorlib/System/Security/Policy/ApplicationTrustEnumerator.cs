using System.Collections;
using System.Runtime.InteropServices;
using Unity;

namespace System.Security.Policy;

[ComVisible(true)]
public sealed class ApplicationTrustEnumerator : IEnumerator
{
	private ApplicationTrustCollection trusts;

	private int current;

	public ApplicationTrust Current => trusts[current];

	object IEnumerator.Current => trusts[current];

	internal ApplicationTrustEnumerator(ApplicationTrustCollection atc)
	{
		trusts = atc;
		current = -1;
	}

	public void Reset()
	{
		current = -1;
	}

	[SecuritySafeCritical]
	public bool MoveNext()
	{
		if (current == trusts.Count - 1)
		{
			return false;
		}
		current++;
		return true;
	}

	internal ApplicationTrustEnumerator()
	{
		ThrowStub.ThrowNotSupportedException();
	}
}
