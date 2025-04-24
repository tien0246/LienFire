using System;
using System.Collections.Generic;
using UnityEngine;

namespace LunarConsolePluginInternal;

[Obsolete("Use 'Lunar Console Action' instead")]
public class LunarConsoleLegacyActions : MonoBehaviour
{
	[SerializeField]
	private bool m_dontDestroyOnLoad;

	[SerializeField]
	[HideInInspector]
	private List<LunarConsoleLegacyAction> m_actions;

	public List<LunarConsoleLegacyAction> actions => m_actions;

	private bool actionsEnabled => LunarConsoleConfig.actionsEnabled;

	private void Awake()
	{
		if (!actionsEnabled)
		{
			UnityEngine.Object.Destroy(this);
		}
		if (m_dontDestroyOnLoad)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	private void Start()
	{
		if (actionsEnabled)
		{
			foreach (LunarConsoleLegacyAction action in m_actions)
			{
				action.Register();
			}
			return;
		}
		UnityEngine.Object.Destroy(this);
	}

	private void OnDestroy()
	{
		if (!actionsEnabled)
		{
			return;
		}
		foreach (LunarConsoleLegacyAction action in m_actions)
		{
			action.Unregister();
		}
	}

	public void AddAction(LunarConsoleLegacyAction action)
	{
		m_actions.Add(action);
	}
}
