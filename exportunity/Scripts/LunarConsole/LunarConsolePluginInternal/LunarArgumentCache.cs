using System;
using UnityEngine;

namespace LunarConsolePluginInternal;

[Serializable]
internal class LunarArgumentCache : ISerializationCallbackReceiver
{
	[SerializeField]
	private UnityEngine.Object m_objectArgument;

	[SerializeField]
	private string m_objectArgumentAssemblyTypeName;

	[SerializeField]
	private int m_intArgument;

	[SerializeField]
	private float m_floatArgument;

	[SerializeField]
	private string m_stringArgument;

	[SerializeField]
	private bool m_boolArgument;

	public UnityEngine.Object unityObjectArgument
	{
		get
		{
			return m_objectArgument;
		}
		set
		{
			m_objectArgument = value;
			m_objectArgumentAssemblyTypeName = ((!(value != null)) ? string.Empty : value.GetType().AssemblyQualifiedName);
		}
	}

	public string unityObjectArgumentAssemblyTypeName => m_objectArgumentAssemblyTypeName;

	public int intArgument
	{
		get
		{
			return m_intArgument;
		}
		set
		{
			m_intArgument = value;
		}
	}

	public float floatArgument
	{
		get
		{
			return m_floatArgument;
		}
		set
		{
			m_floatArgument = value;
		}
	}

	public string stringArgument
	{
		get
		{
			return m_stringArgument;
		}
		set
		{
			m_stringArgument = value;
		}
	}

	public bool boolArgument
	{
		get
		{
			return m_boolArgument;
		}
		set
		{
			m_boolArgument = value;
		}
	}

	private void TidyAssemblyTypeName()
	{
		if (!string.IsNullOrEmpty(m_objectArgumentAssemblyTypeName))
		{
			int num = int.MaxValue;
			int num2 = m_objectArgumentAssemblyTypeName.IndexOf(", Version=");
			if (num2 != -1)
			{
				num = Math.Min(num2, num);
			}
			num2 = m_objectArgumentAssemblyTypeName.IndexOf(", Culture=");
			if (num2 != -1)
			{
				num = Math.Min(num2, num);
			}
			num2 = m_objectArgumentAssemblyTypeName.IndexOf(", PublicKeyToken=");
			if (num2 != -1)
			{
				num = Math.Min(num2, num);
			}
			if (num != int.MaxValue)
			{
				m_objectArgumentAssemblyTypeName = m_objectArgumentAssemblyTypeName.Substring(0, num);
			}
		}
	}

	public void OnBeforeSerialize()
	{
		TidyAssemblyTypeName();
	}

	public void OnAfterDeserialize()
	{
		TidyAssemblyTypeName();
	}
}
