using System;

namespace UnityEngine;

[Obsolete("ParticleSystemEmissionType no longer does anything. Time and Distance based emission are now both always active.", false)]
public enum ParticleSystemEmissionType
{
	Time = 0,
	Distance = 1
}
