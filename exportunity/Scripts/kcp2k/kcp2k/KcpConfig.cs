using System;

namespace kcp2k;

[Serializable]
public class KcpConfig
{
	public bool DualMode;

	public int RecvBufferSize;

	public int SendBufferSize;

	public int Mtu;

	public bool NoDelay;

	public uint Interval;

	public int FastResend;

	public bool CongestionWindow;

	public uint SendWindowSize;

	public uint ReceiveWindowSize;

	public int Timeout;

	public uint MaxRetransmits;

	public KcpConfig(bool DualMode = true, int RecvBufferSize = 7340032, int SendBufferSize = 7340032, int Mtu = 1200, bool NoDelay = true, uint Interval = 10u, int FastResend = 0, bool CongestionWindow = false, uint SendWindowSize = 32u, uint ReceiveWindowSize = 128u, int Timeout = 10000, uint MaxRetransmits = 20u)
	{
		this.DualMode = DualMode;
		this.RecvBufferSize = RecvBufferSize;
		this.SendBufferSize = SendBufferSize;
		this.Mtu = Mtu;
		this.NoDelay = NoDelay;
		this.Interval = Interval;
		this.FastResend = FastResend;
		this.CongestionWindow = CongestionWindow;
		this.SendWindowSize = SendWindowSize;
		this.ReceiveWindowSize = ReceiveWindowSize;
		this.Timeout = Timeout;
		this.MaxRetransmits = MaxRetransmits;
	}
}
