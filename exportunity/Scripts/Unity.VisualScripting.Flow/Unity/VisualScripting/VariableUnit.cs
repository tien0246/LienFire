using System;

namespace Unity.VisualScripting;

[SpecialUnit]
[Obsolete("Use the new unified variable nodes instead.")]
public abstract class VariableUnit : Unit, IVariableUnit, IUnit, IGraphElementWithDebugData, IGraphElement, IGraphItem, INotifiedCollectionItem, IDisposable, IPrewarmable, IAotStubbable, IIdentifiable, IAnalyticsIdentifiable
{
	[DoNotSerialize]
	public string defaultName { get; } = string.Empty;

	[DoNotSerialize]
	[PortLabelHidden]
	public ValueInput name { get; private set; }

	FlowGraph IUnit.graph => base.graph;

	protected VariableUnit()
	{
	}

	protected VariableUnit(string defaultName)
	{
		Ensure.That("defaultName").IsNotNull(defaultName);
		this.defaultName = defaultName;
	}

	protected abstract VariableDeclarations GetDeclarations(Flow flow);

	protected override void Definition()
	{
		name = ValueInput("name", defaultName);
	}
}
