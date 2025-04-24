namespace System.Net;

internal enum WriteBufferState
{
	Disabled = 0,
	Headers = 1,
	Buffer = 2,
	Playback = 3
}
