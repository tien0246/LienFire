using System;

namespace Unity.VisualScripting;

[UnitSurtitle("Graph")]
public sealed class IsGraphVariableDefined : IsVariableDefinedUnit, IGraphVariableUnit, IVariableUnit, IUnit, IGraphElementWithDebugData, IGraphElement, IGraphItem, INotifiedCollectionItem, IDisposable, IPrewarmable, IAotStubbable, IIdentifiable, IAnalyticsIdentifiable
{
	FlowGraph IUnit.graph => base.graph;

	public IsGraphVariableDefined()
	{
	}

	public IsGraphVariableDefined(string defaultName)
		: base(defaultName)
	{
	}

	protected override VariableDeclarations GetDeclarations(Flow flow)
	{
		return Variables.Graph(flow.stack);
	}
}
