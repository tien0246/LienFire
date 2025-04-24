using System;
using System.Collections.Generic;

namespace Mirror;

public class Unbatcher
{
	private Queue<NetworkWriterPooled> batches = new Queue<NetworkWriterPooled>();

	private NetworkReader reader = new NetworkReader(new byte[0]);

	private double readerRemoteTimeStamp;

	public int BatchesCount => batches.Count;

	private void StartReadingBatch(NetworkWriterPooled batch)
	{
		reader.SetBuffer(batch.ToArraySegment());
		readerRemoteTimeStamp = reader.ReadDouble();
	}

	public bool AddBatch(ArraySegment<byte> batch)
	{
		if (batch.Count < 8)
		{
			return false;
		}
		NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		networkWriterPooled.WriteBytes(batch.Array, batch.Offset, batch.Count);
		if (batches.Count == 0)
		{
			StartReadingBatch(networkWriterPooled);
		}
		batches.Enqueue(networkWriterPooled);
		return true;
	}

	public bool GetNextMessage(out NetworkReader message, out double remoteTimeStamp)
	{
		message = null;
		if (batches.Count == 0)
		{
			remoteTimeStamp = 0.0;
			return false;
		}
		if (reader.Capacity == 0)
		{
			remoteTimeStamp = 0.0;
			return false;
		}
		if (reader.Remaining == 0)
		{
			NetworkWriterPool.Return(batches.Dequeue());
			if (batches.Count <= 0)
			{
				remoteTimeStamp = 0.0;
				return false;
			}
			NetworkWriterPooled batch = batches.Peek();
			StartReadingBatch(batch);
		}
		remoteTimeStamp = readerRemoteTimeStamp;
		message = reader;
		return true;
	}
}
