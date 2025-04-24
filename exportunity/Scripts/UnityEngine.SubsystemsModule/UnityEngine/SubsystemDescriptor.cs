using System;

namespace UnityEngine;

public abstract class SubsystemDescriptor : ISubsystemDescriptor
{
	public string id { get; set; }

	public Type subsystemImplementationType { get; set; }

	ISubsystem ISubsystemDescriptor.Create()
	{
		return CreateImpl();
	}

	internal abstract ISubsystem CreateImpl();
}
public class SubsystemDescriptor<TSubsystem> : SubsystemDescriptor where TSubsystem : Subsystem
{
	internal override ISubsystem CreateImpl()
	{
		return Create();
	}

	public TSubsystem Create()
	{
		if (SubsystemManager.FindDeprecatedSubsystemByDescriptor(this) is TSubsystem result)
		{
			return result;
		}
		TSubsystem val = Activator.CreateInstance(base.subsystemImplementationType) as TSubsystem;
		val.m_SubsystemDescriptor = this;
		SubsystemManager.AddDeprecatedSubsystem(val);
		return val;
	}
}
