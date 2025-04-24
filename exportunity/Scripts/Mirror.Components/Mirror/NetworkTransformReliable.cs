using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mirror;

[AddComponentMenu("Network/Network Transform (Reliable)")]
public class NetworkTransformReliable : NetworkTransformBase
{
	[Header("Sync Only If Changed")]
	[Tooltip("When true, changes are not sent unless greater than sensitivity values below.")]
	public bool onlySyncOnChange = true;

	private uint sendIntervalCounter;

	private double lastSendIntervalTime = double.MinValue;

	[Tooltip("If we only sync on change, then we need to correct old snapshots if more time than sendInterval * multiplier has elapsed.\n\nOtherwise the first move will always start interpolating from the last move sequence's time, which will make it stutter when starting every time.")]
	public float onlySyncOnChangeCorrectionMultiplier = 2f;

	[Header("Send Interval Multiplier")]
	[Tooltip("Check/Sync every multiple of Network Manager send interval (= 1 / NM Send Rate), instead of every send interval.")]
	[Range(1f, 120f)]
	public uint sendIntervalMultiplier = 3u;

	[Header("Rotation")]
	[Tooltip("Sensitivity of changes needed before an updated state is sent over the network")]
	public float rotationSensitivity = 0.01f;

	[Tooltip("Apply smallest-three quaternion compression. This is lossy, you can disable it if the small rotation inaccuracies are noticeable in your project.")]
	public bool compressRotation;

	[Header("Precision")]
	[Tooltip("Position is rounded in order to drastically minimize bandwidth.\n\nFor example, a precision of 0.01 rounds to a centimeter. In other words, sub-centimeter movements aren't synced until they eventually exceeded an actual centimeter.\n\nDepending on how important the object is, a precision of 0.01-0.10 (1-10 cm) is recommended.\n\nFor example, even a 1cm precision combined with delta compression cuts the Benchmark demo's bandwidth in half, compared to sending every tiny change.")]
	[Range(0.0001f, 1f)]
	public float positionPrecision = 0.01f;

	[Range(0.0001f, 1f)]
	public float scalePrecision = 0.01f;

	[Header("Snapshot Interpolation")]
	[Tooltip("Add a small timeline offset to account for decoupled arrival of NetworkTime and NetworkTransform snapshots.\nfixes: https://github.com/MirrorNetworking/Mirror/issues/3427")]
	public bool timelineOffset;

	protected Vector3Long lastSerializedPosition = Vector3Long.zero;

	protected Vector3Long lastDeserializedPosition = Vector3Long.zero;

	protected Vector3Long lastSerializedScale = Vector3Long.zero;

	protected Vector3Long lastDeserializedScale = Vector3Long.zero;

	protected TransformSnapshot last;

	protected int lastClientCount = 1;

	private double timeStampAdjustment => NetworkServer.sendInterval * (float)(sendIntervalMultiplier - 1);

	private double offset => timelineOffset ? (NetworkServer.sendInterval * (float)sendIntervalMultiplier) : 0f;

	private void Update()
	{
		if (base.isServer)
		{
			UpdateServer();
		}
		else if (base.isClient)
		{
			UpdateClient();
		}
	}

	private void LateUpdate()
	{
		if (base.isServer || (base.IsClientWithAuthority && NetworkClient.ready))
		{
			if (sendIntervalCounter == sendIntervalMultiplier && (!onlySyncOnChange || Changed(Construct())))
			{
				SetDirty();
			}
			CheckLastSendTime();
		}
	}

	protected virtual void UpdateServer()
	{
		if (syncDirection == SyncDirection.ClientToServer && base.connectionToClient != null && !base.isOwned && serverSnapshots.Count > 0)
		{
			SnapshotInterpolation.StepInterpolation(serverSnapshots, base.connectionToClient.remoteTimeline, out var fromSnapshot, out var toSnapshot, out var t);
			TransformSnapshot interpolated = TransformSnapshot.Interpolate(fromSnapshot, toSnapshot, t);
			Apply(interpolated, toSnapshot);
		}
	}

	protected virtual void UpdateClient()
	{
		if (!base.IsClientWithAuthority)
		{
			if (clientSnapshots.Count > 0)
			{
				SnapshotInterpolation.StepInterpolation(clientSnapshots, NetworkTime.time, out var fromSnapshot, out var toSnapshot, out var t);
				TransformSnapshot interpolated = TransformSnapshot.Interpolate(fromSnapshot, toSnapshot, t);
				Apply(interpolated, toSnapshot);
			}
			lastClientCount = clientSnapshots.Count;
		}
	}

	protected virtual void CheckLastSendTime()
	{
		if (AccurateInterval.Elapsed(NetworkTime.localTime, NetworkServer.sendInterval, ref lastSendIntervalTime))
		{
			if (sendIntervalCounter == sendIntervalMultiplier)
			{
				sendIntervalCounter = 0u;
			}
			sendIntervalCounter++;
		}
	}

	protected virtual bool Changed(TransformSnapshot current)
	{
		if (!QuantizedChanged(last.position, current.position, positionPrecision) && !(Quaternion.Angle(last.rotation, current.rotation) > rotationSensitivity))
		{
			return QuantizedChanged(last.scale, current.scale, scalePrecision);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool QuantizedChanged(Vector3 u, Vector3 v, float precision)
	{
		Compression.ScaleToLong(u, precision, out var quantized);
		Compression.ScaleToLong(v, precision, out var quantized2);
		return quantized != quantized2;
	}

	public override void OnSerialize(NetworkWriter writer, bool initialState)
	{
		TransformSnapshot transformSnapshot = Construct();
		if (initialState)
		{
			if (last.remoteTime > 0.0)
			{
				transformSnapshot = last;
			}
			if (syncPosition)
			{
				writer.WriteVector3(transformSnapshot.position);
			}
			if (syncRotation)
			{
				if (compressRotation)
				{
					writer.WriteUInt(Compression.CompressQuaternion(transformSnapshot.rotation));
				}
				else
				{
					writer.WriteQuaternion(transformSnapshot.rotation);
				}
			}
			if (syncScale)
			{
				writer.WriteVector3(transformSnapshot.scale);
			}
		}
		else
		{
			if (syncPosition)
			{
				Compression.ScaleToLong(transformSnapshot.position, positionPrecision, out var quantized);
				DeltaCompression.Compress(writer, lastSerializedPosition, quantized);
			}
			if (syncRotation)
			{
				if (compressRotation)
				{
					writer.WriteUInt(Compression.CompressQuaternion(transformSnapshot.rotation));
				}
				else
				{
					writer.WriteQuaternion(transformSnapshot.rotation);
				}
			}
			if (syncScale)
			{
				Compression.ScaleToLong(transformSnapshot.scale, scalePrecision, out var quantized2);
				DeltaCompression.Compress(writer, lastSerializedScale, quantized2);
			}
		}
		if (syncPosition)
		{
			Compression.ScaleToLong(transformSnapshot.position, positionPrecision, out lastSerializedPosition);
		}
		if (syncScale)
		{
			Compression.ScaleToLong(transformSnapshot.scale, scalePrecision, out lastSerializedScale);
		}
		last = transformSnapshot;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
		Vector3? position = null;
		Quaternion? rotation = null;
		Vector3? scale = null;
		if (initialState)
		{
			if (syncPosition)
			{
				position = reader.ReadVector3();
			}
			if (syncRotation)
			{
				rotation = ((!compressRotation) ? new Quaternion?(reader.ReadQuaternion()) : new Quaternion?(Compression.DecompressQuaternion(reader.ReadUInt())));
			}
			if (syncScale)
			{
				scale = reader.ReadVector3();
			}
		}
		else
		{
			if (syncPosition)
			{
				Vector3Long value = DeltaCompression.Decompress(reader, lastDeserializedPosition);
				position = Compression.ScaleToFloat(value, positionPrecision);
			}
			if (syncRotation)
			{
				rotation = ((!compressRotation) ? new Quaternion?(reader.ReadQuaternion()) : new Quaternion?(Compression.DecompressQuaternion(reader.ReadUInt())));
			}
			if (syncScale)
			{
				Vector3Long value2 = DeltaCompression.Decompress(reader, lastDeserializedScale);
				scale = Compression.ScaleToFloat(value2, scalePrecision);
			}
		}
		if (base.isServer)
		{
			OnClientToServerSync(position, rotation, scale);
		}
		else if (base.isClient)
		{
			OnServerToClientSync(position, rotation, scale);
		}
		if (syncPosition)
		{
			Compression.ScaleToLong(position.Value, positionPrecision, out lastDeserializedPosition);
		}
		if (syncScale)
		{
			Compression.ScaleToLong(scale.Value, scalePrecision, out lastDeserializedScale);
		}
	}

	protected virtual void OnClientToServerSync(Vector3? position, Quaternion? rotation, Vector3? scale)
	{
		if (syncDirection == SyncDirection.ClientToServer && serverSnapshots.Count < base.connectionToClient.snapshotBufferSizeLimit)
		{
			if (onlySyncOnChange && NeedsCorrection(serverSnapshots, base.connectionToClient.remoteTimeStamp, NetworkServer.sendInterval * (float)sendIntervalMultiplier, onlySyncOnChangeCorrectionMultiplier))
			{
				RewriteHistory(serverSnapshots, base.connectionToClient.remoteTimeStamp, NetworkTime.localTime, NetworkServer.sendInterval * (float)sendIntervalMultiplier, target.localPosition, target.localRotation, target.localScale);
			}
			AddSnapshot(serverSnapshots, base.connectionToClient.remoteTimeStamp + timeStampAdjustment + offset, position, rotation, scale);
		}
	}

	protected virtual void OnServerToClientSync(Vector3? position, Quaternion? rotation, Vector3? scale)
	{
		if (!base.IsClientWithAuthority)
		{
			if (onlySyncOnChange && NeedsCorrection(clientSnapshots, NetworkClient.connection.remoteTimeStamp, NetworkClient.sendInterval * (float)sendIntervalMultiplier, onlySyncOnChangeCorrectionMultiplier))
			{
				RewriteHistory(clientSnapshots, NetworkClient.connection.remoteTimeStamp, NetworkTime.localTime, NetworkClient.sendInterval * (float)sendIntervalMultiplier, target.localPosition, target.localRotation, target.localScale);
			}
			AddSnapshot(clientSnapshots, NetworkClient.connection.remoteTimeStamp + timeStampAdjustment + offset, position, rotation, scale);
		}
	}

	private static bool NeedsCorrection(SortedList<double, TransformSnapshot> snapshots, double remoteTimestamp, double bufferTime, double toleranceMultiplier)
	{
		if (snapshots.Count == 1)
		{
			return remoteTimestamp - snapshots.Keys[0] >= bufferTime * toleranceMultiplier;
		}
		return false;
	}

	private static void RewriteHistory(SortedList<double, TransformSnapshot> snapshots, double remoteTimeStamp, double localTime, double sendInterval, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		snapshots.Clear();
		SnapshotInterpolation.InsertIfNotExists(snapshots, new TransformSnapshot(remoteTimeStamp - sendInterval, localTime - sendInterval, position, rotation, scale));
	}

	public override void Reset()
	{
		base.Reset();
		lastSerializedPosition = Vector3Long.zero;
		lastDeserializedPosition = Vector3Long.zero;
		lastSerializedScale = Vector3Long.zero;
		lastDeserializedScale = Vector3Long.zero;
		last = new TransformSnapshot(0.0, 0.0, Vector3.zero, Quaternion.identity, Vector3.zero);
	}

	private void MirrorProcessed()
	{
	}
}
