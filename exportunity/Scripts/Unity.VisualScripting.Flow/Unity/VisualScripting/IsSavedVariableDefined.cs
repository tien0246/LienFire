using System;

namespace Unity.VisualScripting;

[UnitSurtitle("Save")]
public sealed class IsSavedVariableDefined : IsVariableDefinedUnit, ISavedVariableUnit, IVariableUnit, IUnit, IGraphElementWithDebugData, IGraphElement, IGraphItem, INotifiedCollectionItem, IDisposable, IPrewarmable, IAotStubbable, IIdentifiable, IAnalyticsIdentifiable
{
	FlowGraph IUnit.graph => base.graph;

	public IsSavedVariableDefined()
	{
	}

	public IsSavedVariableDefined(string defaultName)
		: base(defaultName)
	{
	}

	protected override VariableDeclarations GetDeclarations(Flow flow)
	{
		return Variables.Saved;
	}
}
