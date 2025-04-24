using UnityEngine;

namespace Mirror.Examples.MultipleAdditiveScenes;

public class PhysicsSimulator : MonoBehaviour
{
	private PhysicsScene physicsScene;

	private PhysicsScene2D physicsScene2D;

	private bool simulatePhysicsScene;

	private bool simulatePhysicsScene2D;

	private void Awake()
	{
		if (NetworkServer.active)
		{
			physicsScene = base.gameObject.scene.GetPhysicsScene();
			simulatePhysicsScene = physicsScene.IsValid() && physicsScene != Physics.defaultPhysicsScene;
			physicsScene2D = base.gameObject.scene.GetPhysicsScene2D();
			simulatePhysicsScene2D = physicsScene2D.IsValid() && physicsScene2D != Physics2D.defaultPhysicsScene;
		}
		else
		{
			base.enabled = false;
		}
	}

	[ServerCallback]
	private void FixedUpdate()
	{
		if (NetworkServer.active)
		{
			if (simulatePhysicsScene)
			{
				physicsScene.Simulate(Time.fixedDeltaTime);
			}
			if (simulatePhysicsScene2D)
			{
				physicsScene2D.Simulate(Time.fixedDeltaTime);
			}
		}
	}
}
