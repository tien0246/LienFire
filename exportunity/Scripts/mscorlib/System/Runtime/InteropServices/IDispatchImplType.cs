namespace System.Runtime.InteropServices;

[Serializable]
[ComVisible(true)]
[Obsolete("The IDispatchImplAttribute is deprecated.", false)]
public enum IDispatchImplType
{
	SystemDefinedImpl = 0,
	InternalImpl = 1,
	CompatibleImpl = 2
}
