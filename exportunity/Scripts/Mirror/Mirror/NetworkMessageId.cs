namespace Mirror;

public static class NetworkMessageId<T> where T : struct, NetworkMessage
{
	public static readonly ushort Id = (ushort)typeof(T).FullName.GetStableHashCode();
}
