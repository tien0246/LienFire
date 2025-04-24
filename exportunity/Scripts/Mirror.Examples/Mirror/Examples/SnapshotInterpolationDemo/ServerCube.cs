using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Examples.SnapshotInterpolationDemo;

public class ServerCube : MonoBehaviour
{
	[Header("Components")]
	public ClientCube client;

	[Header("Movement")]
	public float distance = 10f;

	public float speed = 3f;

	private Vector3 start;

	[Header("Snapshot Interpolation")]
	[Tooltip("Send N snapshots per second. Multiples of frame rate make sense.")]
	public int sendRate = 30;

	private float lastSendTime;

	[Header("Latency Simulation")]
	[Tooltip("Latency in seconds")]
	public float latency = 0.05f;

	[Tooltip("Latency jitter, randomly added to latency.")]
	[Range(0f, 1f)]
	public float jitter = 0.05f;

	[Tooltip("Packet loss in %")]
	[Range(0f, 1f)]
	public float loss = 0.1f;

	[Tooltip("Scramble % of unreliable messages, just like over the real network. Mirror unreliable is unordered.")]
	[Range(0f, 1f)]
	public float scramble = 0.1f;

	private System.Random random = new System.Random();

	private List<(double, Snapshot3D)> queue = new List<(double, Snapshot3D)>();

	public float sendInterval => 1f / (float)sendRate;

	private float SimulateLatency()
	{
		return latency + UnityEngine.Random.value * jitter;
	}

	private void Start()
	{
		start = base.transform.position;
	}

	private void Update()
	{
		float num = Mathf.PingPong(Time.time * speed, distance);
		base.transform.position = new Vector3(start.x + num, start.y, start.z);
		if (Time.time >= lastSendTime + sendInterval)
		{
			Send(base.transform.position);
			lastSendTime = Time.time;
		}
		Flush();
	}

	private void Send(Vector3 position)
	{
		Snapshot3D item = new Snapshot3D(NetworkTime.localTime, 0.0, position);
		if (!(random.NextDouble() < (double)loss))
		{
			bool num = random.NextDouble() < (double)scramble;
			int count = queue.Count;
			int index = (num ? random.Next(0, count + 1) : count);
			float num2 = SimulateLatency();
			double item2 = NetworkTime.localTime + (double)num2;
			queue.Insert(index, (item2, item));
		}
	}

	private void Flush()
	{
		for (int i = 0; i < queue.Count; i++)
		{
			var (num, snap) = queue[i];
			if (NetworkTime.localTime >= num)
			{
				client.OnMessage(snap);
				queue.RemoveAt(i);
				i--;
			}
		}
	}
}
