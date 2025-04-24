namespace System.Runtime.InteropServices;

[Serializable]
[Obsolete("Use System.Runtime.InteropServices.ComTypes.FUNCKIND instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
public enum FUNCKIND
{
	FUNC_VIRTUAL = 0,
	FUNC_PUREVIRTUAL = 1,
	FUNC_NONVIRTUAL = 2,
	FUNC_STATIC = 3,
	FUNC_DISPATCH = 4
}
