using System.Collections.Generic;
using LunarConsolePlugin;
using UnityEngine;

namespace LunarConsolePluginInternal;

public class LunarConsoleAction : MonoBehaviour
{
	[SerializeField]
	private string m_title = "Untitled Action";

	[SerializeField]
	[HideInInspector]
	private List<LunarConsoleActionCall> m_calls;

	public List<LunarConsoleActionCall> calls => m_calls;

	private bool actionsEnabled => LunarConsoleConfig.actionsEnabled;

	private void Awake()
	{
		if (!actionsEnabled)
		{
			Object.Destroy(this);
		}
	}

	private void Start()
	{
		if (actionsEnabled)
		{
			RegisterAction();
		}
		else
		{
			Object.Destroy(this);
		}
	}

	private void OnValidate()
	{
		if (m_calls == null || m_calls.Count <= 0)
		{
			return;
		}
		foreach (LunarConsoleActionCall call in m_calls)
		{
			Validate(call);
		}
	}

	private void Validate(LunarConsoleActionCall call)
	{
		if (call.target == null)
		{
			Debug.LogWarning($"Action '{m_title}' ({base.gameObject.name}) is missing a target object", base.gameObject);
		}
		else if (!LunarConsoleActionCall.IsPersistantListenerValid(call.target, call.methodName, call.mode))
		{
			Debug.LogWarning($"Action '{m_title}' ({base.gameObject.name}) is missing a handler <{call.target.GetType()}.{call.methodName} ({ModeParamTypeName(call.mode)})>", base.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (actionsEnabled)
		{
			UnregisterAction();
		}
	}

	private void RegisterAction()
	{
		LunarConsole.RegisterAction(m_title, InvokeAction);
	}

	private void UnregisterAction()
	{
		LunarConsole.UnregisterAction(InvokeAction);
	}

	private void InvokeAction()
	{
		if (m_calls != null && m_calls.Count > 0)
		{
			foreach (LunarConsoleActionCall call in m_calls)
			{
				call.Invoke();
			}
			return;
		}
		Debug.LogWarningFormat("Action '{0}' has 0 calls", m_title);
	}

	private static string ModeParamTypeName(LunarPersistentListenerMode mode)
	{
		return mode switch
		{
			LunarPersistentListenerMode.Void => "", 
			LunarPersistentListenerMode.Bool => "bool", 
			LunarPersistentListenerMode.Float => "float", 
			LunarPersistentListenerMode.Int => "int", 
			LunarPersistentListenerMode.String => "string", 
			LunarPersistentListenerMode.Object => "UnityEngine.Object", 
			_ => "???", 
		};
	}
}
