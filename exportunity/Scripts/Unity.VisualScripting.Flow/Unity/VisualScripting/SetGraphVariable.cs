using System;

namespace Unity.VisualScripting;

[UnitSurtitle("Graph")]
public sealed class SetGraphVariable : SetVariableUnit, IGraphVariableUnit, IVariableUnit, IUnit, IGraphElementWithDebugData, IGraphElement, IGraphItem, INotifiedCollectionItem, IDisposable, IPrewarmable, IAotStubbable, IIdentifiable, IAnalyticsIdentifiable
{
	FlowGraph IUnit.graph => base.graph;

	public SetGraphVariable()
	{
	}

	public SetGraphVariable(string defaultName)
		: base(defaultName)
	{
	}

	protected override VariableDeclarations GetDeclarations(Flow flow)
	{
		return Variables.Graph(flow.stack);
	}
}
