namespace System.IO.Ports;

internal interface ISerialStream : IDisposable
{
	int BytesToRead { get; }

	int BytesToWrite { get; }

	int ReadTimeout { get; set; }

	int WriteTimeout { get; set; }

	int Read(byte[] buffer, int offset, int count);

	void Write(byte[] buffer, int offset, int count);

	void SetAttributes(int baud_rate, Parity parity, int data_bits, StopBits sb, Handshake hs);

	void DiscardInBuffer();

	void DiscardOutBuffer();

	SerialSignal GetSignals();

	void SetSignal(SerialSignal signal, bool value);

	void SetBreakState(bool value);

	void Close();
}
