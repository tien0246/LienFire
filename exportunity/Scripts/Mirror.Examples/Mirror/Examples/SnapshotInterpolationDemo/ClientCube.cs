using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Examples.SnapshotInterpolationDemo;

public class ClientCube : MonoBehaviour
{
	[Header("Components")]
	public ServerCube server;

	public Renderer render;

	[Header("Toggle")]
	public bool interpolate = true;

	[Header("Snapshot Interpolation")]
	public SnapshotInterpolationSettings snapshotSettings = new SnapshotInterpolationSettings();

	public SortedList<double, Snapshot3D> snapshots = new SortedList<double, Snapshot3D>();

	private double localTimeline;

	private double localTimescale = 1.0;

	private ExponentialMovingAverage driftEma;

	private ExponentialMovingAverage deliveryTimeEma;

	[Header("Debug")]
	public Color catchupColor = Color.green;

	public Color slowdownColor = Color.red;

	private Color defaultColor;

	[Header("Simulation")]
	private bool lowFpsMode;

	private double accumulatedDeltaTime;

	public double bufferTime => (double)server.sendInterval * snapshotSettings.bufferTimeMultiplier;

	private void Awake()
	{
		Debug.Log("Reminder: Snapshot interpolation is smoothest & easiest to debug with Vsync off.");
		defaultColor = render.sharedMaterial.color;
		driftEma = new ExponentialMovingAverage(server.sendRate * snapshotSettings.driftEmaDuration);
		deliveryTimeEma = new ExponentialMovingAverage(server.sendRate * snapshotSettings.deliveryTimeEmaDuration);
	}

	public void OnMessage(Snapshot3D snap)
	{
		snap.localTime = NetworkTime.localTime;
		if (snapshotSettings.dynamicAdjustment)
		{
			snapshotSettings.bufferTimeMultiplier = SnapshotInterpolation.DynamicAdjustment(server.sendInterval, deliveryTimeEma.StandardDeviation, snapshotSettings.dynamicAdjustmentTolerance);
		}
		SnapshotInterpolation.InsertAndAdjust(snapshots, snap, ref localTimeline, ref localTimescale, server.sendInterval, bufferTime, snapshotSettings.catchupSpeed, snapshotSettings.slowdownSpeed, ref driftEma, snapshotSettings.catchupNegativeThreshold, snapshotSettings.catchupPositiveThreshold, ref deliveryTimeEma);
	}

	private void Update()
	{
		accumulatedDeltaTime += Time.unscaledDeltaTime;
		if (lowFpsMode && accumulatedDeltaTime < 1.0)
		{
			return;
		}
		if (snapshots.Count > 0)
		{
			if (interpolate)
			{
				SnapshotInterpolation.Step(snapshots, accumulatedDeltaTime, ref localTimeline, localTimescale, out var fromSnapshot, out var toSnapshot, out var t);
				Snapshot3D snapshot3D = Snapshot3D.Interpolate(fromSnapshot, toSnapshot, t);
				base.transform.position = snapshot3D.position;
			}
			else
			{
				Snapshot3D snapshot3D2 = snapshots.Values[0];
				base.transform.position = snapshot3D2.position;
				snapshots.RemoveAt(0);
			}
		}
		accumulatedDeltaTime = 0.0;
		if (localTimescale < 1.0)
		{
			render.material.color = slowdownColor;
		}
		else if (localTimescale > 1.0)
		{
			render.material.color = catchupColor;
		}
		else
		{
			render.material.color = defaultColor;
		}
	}

	private void OnGUI()
	{
		Vector2 vector = Camera.main.WorldToScreenPoint(base.transform.position);
		string text = $"{snapshots.Count}";
		GUI.Label(new Rect(vector.x - 15f, vector.y - 10f, 30f, 20f), text);
		float num = 100f;
		GUILayout.BeginArea(new Rect(0f, (float)Screen.height - num, Screen.width, num));
		GUILayout.BeginHorizontal();
		GUILayout.Label("Client Simulation:");
		if (GUILayout.Button((lowFpsMode ? "Disable" : "Enable") + " 1 FPS"))
		{
			lowFpsMode = !lowFpsMode;
		}
		GUILayout.Label("|");
		if (GUILayout.Button("Timeline 10s behind"))
		{
			localTimeline -= 10.0;
		}
		if (GUILayout.Button("Timeline 1s behind"))
		{
			localTimeline -= 1.0;
		}
		if (GUILayout.Button("Timeline 0.1s behind"))
		{
			localTimeline -= 0.1;
		}
		GUILayout.Label("|");
		if (GUILayout.Button("Timeline 0.1s ahead"))
		{
			localTimeline += 0.1;
		}
		if (GUILayout.Button("Timeline 1s ahead"))
		{
			localTimeline += 1.0;
		}
		if (GUILayout.Button("Timeline 10s ahead"))
		{
			localTimeline += 10.0;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void OnValidate()
	{
		snapshotSettings.catchupNegativeThreshold = Math.Min(snapshotSettings.catchupNegativeThreshold, 0f);
		snapshotSettings.catchupPositiveThreshold = Math.Max(snapshotSettings.catchupPositiveThreshold, 0f);
	}
}
