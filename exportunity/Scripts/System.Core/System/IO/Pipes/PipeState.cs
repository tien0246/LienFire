namespace System.IO.Pipes;

internal enum PipeState
{
	WaitingToConnect = 0,
	Connected = 1,
	Broken = 2,
	Disconnected = 3,
	Closed = 4
}
