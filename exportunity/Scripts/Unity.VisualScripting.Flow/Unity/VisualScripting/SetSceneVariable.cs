using System;
using UnityEngine.SceneManagement;

namespace Unity.VisualScripting;

[UnitSurtitle("Scene")]
public sealed class SetSceneVariable : SetVariableUnit, ISceneVariableUnit, IVariableUnit, IUnit, IGraphElementWithDebugData, IGraphElement, IGraphItem, INotifiedCollectionItem, IDisposable, IPrewarmable, IAotStubbable, IIdentifiable, IAnalyticsIdentifiable
{
	FlowGraph IUnit.graph => base.graph;

	public SetSceneVariable()
	{
	}

	public SetSceneVariable(string defaultName)
		: base(defaultName)
	{
	}

	protected override VariableDeclarations GetDeclarations(Flow flow)
	{
		Scene? scene = flow.stack.scene;
		if (!scene.HasValue)
		{
			return null;
		}
		return Variables.Scene(scene.Value);
	}
}
