namespace UnityEngine;

public struct JointMotor
{
	private float m_TargetVelocity;

	private float m_Force;

	private int m_FreeSpin;

	public float targetVelocity
	{
		get
		{
			return m_TargetVelocity;
		}
		set
		{
			m_TargetVelocity = value;
		}
	}

	public float force
	{
		get
		{
			return m_Force;
		}
		set
		{
			m_Force = value;
		}
	}

	public bool freeSpin
	{
		get
		{
			return m_FreeSpin == 1;
		}
		set
		{
			m_FreeSpin = (value ? 1 : 0);
		}
	}
}
