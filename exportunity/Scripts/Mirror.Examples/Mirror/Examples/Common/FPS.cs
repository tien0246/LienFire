using UnityEngine;

namespace Mirror.Examples.Common;

public class FPS : MonoBehaviour
{
	public bool showGUI = true;

	public bool showLog;

	private int count;

	private double startTime;

	public int framesPerSecond { get; private set; }

	protected void Update()
	{
		count++;
		if ((double)Time.time >= startTime + 1.0)
		{
			framesPerSecond = count;
			startTime = Time.time;
			count = 0;
			if (showLog)
			{
				Debug.Log($"FPS: {framesPerSecond}");
			}
		}
	}

	protected void OnGUI()
	{
		if (showGUI)
		{
			GUI.Label(new Rect(Screen.width - 70, 0f, 70f, 25f), $"FPS: {framesPerSecond}");
		}
	}
}
