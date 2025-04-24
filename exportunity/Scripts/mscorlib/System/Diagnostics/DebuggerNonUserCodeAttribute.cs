using System.Runtime.InteropServices;

namespace System.Diagnostics;

[Serializable]
[ComVisible(true)]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, Inherited = false)]
public sealed class DebuggerNonUserCodeAttribute : Attribute
{
}
