using System;
using System.Collections.Generic;

namespace Mirror;

public class Batcher
{
	private readonly int threshold;

	public const int HeaderSize = 8;

	private readonly Queue<NetworkWriterPooled> batches = new Queue<NetworkWriterPooled>();

	private NetworkWriterPooled batch;

	public Batcher(int threshold)
	{
		this.threshold = threshold;
	}

	public void AddMessage(ArraySegment<byte> message, double timeStamp)
	{
		if (batch != null && batch.Position + message.Count > threshold)
		{
			batches.Enqueue(batch);
			batch = null;
		}
		if (batch == null)
		{
			batch = NetworkWriterPool.Get();
			batch.WriteDouble(timeStamp);
		}
		batch.WriteBytes(message.Array, message.Offset, message.Count);
	}

	private static void CopyAndReturn(NetworkWriterPooled batch, NetworkWriter writer)
	{
		if (writer.Position != 0)
		{
			throw new ArgumentException("GetBatch needs a fresh writer!");
		}
		ArraySegment<byte> arraySegment = batch.ToArraySegment();
		writer.WriteBytes(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
		NetworkWriterPool.Return(batch);
	}

	public bool GetBatch(NetworkWriter writer)
	{
		if (batches.TryDequeue(out var result))
		{
			CopyAndReturn(result, writer);
			return true;
		}
		if (batch != null)
		{
			CopyAndReturn(batch, writer);
			batch = null;
			return true;
		}
		return false;
	}
}
