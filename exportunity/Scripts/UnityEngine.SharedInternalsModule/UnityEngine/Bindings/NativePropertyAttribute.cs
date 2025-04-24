using System;

namespace UnityEngine.Bindings;

[VisibleToOtherModules]
[AttributeUsage(AttributeTargets.Property)]
internal class NativePropertyAttribute : NativeMethodAttribute
{
	public TargetType TargetType { get; set; }

	public NativePropertyAttribute()
	{
	}

	public NativePropertyAttribute(string name)
		: base(name)
	{
	}

	public NativePropertyAttribute(string name, TargetType targetType)
		: base(name)
	{
		TargetType = targetType;
	}

	public NativePropertyAttribute(string name, bool isFree, TargetType targetType)
		: base(name, isFree)
	{
		TargetType = targetType;
	}

	public NativePropertyAttribute(string name, bool isFree, TargetType targetType, bool isThreadSafe)
		: base(name, isFree, isThreadSafe)
	{
		TargetType = targetType;
	}
}
