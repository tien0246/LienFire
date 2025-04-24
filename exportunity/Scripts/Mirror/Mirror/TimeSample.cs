using System.Diagnostics;
using System.Threading;

namespace Mirror;

public struct TimeSample
{
	private readonly Stopwatch watch;

	private double beginTime;

	private ExponentialMovingAverage ema;

	public double average;

	public TimeSample(int n)
	{
		watch = new Stopwatch();
		watch.Start();
		ema = new ExponentialMovingAverage(n);
		beginTime = 0.0;
		average = 0.0;
	}

	public void Begin()
	{
		beginTime = watch.Elapsed.TotalSeconds;
	}

	public void End()
	{
		double newValue = watch.Elapsed.TotalSeconds - beginTime;
		ema.Add(newValue);
		Interlocked.Exchange(ref average, ema.Value);
	}
}
