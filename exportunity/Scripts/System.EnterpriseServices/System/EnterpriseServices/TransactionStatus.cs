using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[Serializable]
[ComVisible(false)]
public enum TransactionStatus
{
	Commited = 0,
	LocallyOk = 1,
	NoTransaction = 2,
	Aborting = 3,
	Aborted = 4
}
