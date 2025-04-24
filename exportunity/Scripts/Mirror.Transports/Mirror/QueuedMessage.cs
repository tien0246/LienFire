namespace Mirror;

internal struct QueuedMessage
{
	public int connectionId;

	public byte[] bytes;

	public double time;
}
