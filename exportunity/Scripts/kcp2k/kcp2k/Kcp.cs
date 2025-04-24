using System;
using System.Collections.Generic;

namespace kcp2k;

public class Kcp
{
	internal struct AckItem
	{
		internal uint serialNumber;

		internal uint timestamp;
	}

	public const int RTO_NDL = 30;

	public const int RTO_MIN = 100;

	public const int RTO_DEF = 200;

	public const int RTO_MAX = 60000;

	public const int CMD_PUSH = 81;

	public const int CMD_ACK = 82;

	public const int CMD_WASK = 83;

	public const int CMD_WINS = 84;

	public const int ASK_SEND = 1;

	public const int ASK_TELL = 2;

	public const int WND_SND = 32;

	public const int WND_RCV = 128;

	public const int MTU_DEF = 1200;

	public const int ACK_FAST = 3;

	public const int INTERVAL = 100;

	public const int OVERHEAD = 24;

	public const int FRG_MAX = 255;

	public const int DEADLINK = 20;

	public const int THRESH_INIT = 2;

	public const int THRESH_MIN = 2;

	public const int PROBE_INIT = 7000;

	public const int PROBE_LIMIT = 120000;

	public const int FASTACK_LIMIT = 5;

	internal int state;

	private readonly uint conv;

	internal uint mtu;

	internal uint mss;

	internal uint snd_una;

	internal uint snd_nxt;

	internal uint rcv_nxt;

	internal uint ssthresh;

	internal int rx_rttval;

	internal int rx_srtt;

	internal int rx_rto;

	internal int rx_minrto;

	internal uint snd_wnd;

	internal uint rcv_wnd;

	internal uint rmt_wnd;

	internal uint cwnd;

	internal uint probe;

	internal uint interval;

	internal uint ts_flush;

	internal uint xmit;

	internal uint nodelay;

	internal bool updated;

	internal uint ts_probe;

	internal uint probe_wait;

	internal uint dead_link;

	internal uint incr;

	internal uint current;

	internal int fastresend;

	internal int fastlimit;

	internal bool nocwnd;

	internal readonly Queue<Segment> snd_queue = new Queue<Segment>(16);

	internal readonly Queue<Segment> rcv_queue = new Queue<Segment>(16);

	internal readonly List<Segment> snd_buf = new List<Segment>(16);

	internal readonly List<Segment> rcv_buf = new List<Segment>(16);

	internal readonly List<AckItem> acklist = new List<AckItem>(16);

	internal byte[] buffer;

	private readonly Action<byte[], int> output;

	private readonly Pool<Segment> SegmentPool = new Pool<Segment>(() => new Segment(), delegate(Segment segment)
	{
		segment.Reset();
	}, 32);

	public int WaitSnd => snd_buf.Count + snd_queue.Count;

	public Kcp(uint conv, Action<byte[], int> output)
	{
		this.conv = conv;
		this.output = output;
		snd_wnd = 32u;
		rcv_wnd = 128u;
		rmt_wnd = 128u;
		mtu = 1200u;
		mss = mtu - 24;
		rx_rto = 200;
		rx_minrto = 100;
		interval = 100u;
		ts_flush = 100u;
		ssthresh = 2u;
		fastlimit = 5;
		dead_link = 20u;
		buffer = new byte[(mtu + 24) * 3];
	}

	private Segment SegmentNew()
	{
		return SegmentPool.Take();
	}

	private void SegmentDelete(Segment seg)
	{
		SegmentPool.Return(seg);
	}

	public int Receive(byte[] buffer, int len)
	{
		if (len < 0)
		{
			throw new NotSupportedException("Receive ispeek for negative len is not supported!");
		}
		if (rcv_queue.Count == 0)
		{
			return -1;
		}
		if (len < 0)
		{
			len = -len;
		}
		int num = PeekSize();
		if (num < 0)
		{
			return -2;
		}
		if (num > len)
		{
			return -3;
		}
		bool flag = rcv_queue.Count >= rcv_wnd;
		int num2 = 0;
		len = 0;
		while (rcv_queue.Count > 0)
		{
			Segment segment = rcv_queue.Dequeue();
			Buffer.BlockCopy(segment.data.GetBuffer(), 0, buffer, num2, (int)segment.data.Position);
			num2 += (int)segment.data.Position;
			len += (int)segment.data.Position;
			uint frg = segment.frg;
			SegmentDelete(segment);
			if (frg == 0)
			{
				break;
			}
		}
		int num3 = 0;
		foreach (Segment item in rcv_buf)
		{
			if (item.sn == rcv_nxt && rcv_queue.Count < rcv_wnd)
			{
				num3++;
				rcv_queue.Enqueue(item);
				rcv_nxt++;
				continue;
			}
			break;
		}
		rcv_buf.RemoveRange(0, num3);
		if (rcv_queue.Count < rcv_wnd && flag)
		{
			probe |= 2u;
		}
		return len;
	}

	public int PeekSize()
	{
		int num = 0;
		if (rcv_queue.Count == 0)
		{
			return -1;
		}
		Segment segment = rcv_queue.Peek();
		if (segment.frg == 0)
		{
			return (int)segment.data.Position;
		}
		if (rcv_queue.Count < segment.frg + 1)
		{
			return -1;
		}
		foreach (Segment item in rcv_queue)
		{
			num += (int)item.data.Position;
			if (item.frg == 0)
			{
				break;
			}
		}
		return num;
	}

	public int Send(byte[] buffer, int offset, int len)
	{
		if (len < 0)
		{
			return -1;
		}
		int num = (int)((len <= mss) ? 1 : ((len + mss - 1) / mss));
		if (num > 255)
		{
			throw new Exception($"Send len={len} requires {num} fragments, but kcp can only handle up to {255} fragments.");
		}
		if (num >= rcv_wnd)
		{
			return -2;
		}
		if (num == 0)
		{
			num = 1;
		}
		for (int i = 0; i < num; i++)
		{
			int num2 = ((len > (int)mss) ? ((int)mss) : len);
			Segment segment = SegmentNew();
			if (len > 0)
			{
				segment.data.Write(buffer, offset, num2);
			}
			segment.frg = (byte)(num - i - 1);
			snd_queue.Enqueue(segment);
			offset += num2;
			len -= num2;
		}
		return 0;
	}

	private void UpdateAck(int rtt)
	{
		if (rx_srtt == 0)
		{
			rx_srtt = rtt;
			rx_rttval = rtt / 2;
		}
		else
		{
			int num = rtt - rx_srtt;
			if (num < 0)
			{
				num = -num;
			}
			rx_rttval = (3 * rx_rttval + num) / 4;
			rx_srtt = (7 * rx_srtt + rtt) / 8;
			if (rx_srtt < 1)
			{
				rx_srtt = 1;
			}
		}
		int value = rx_srtt + Math.Max((int)interval, 4 * rx_rttval);
		rx_rto = Utils.Clamp(value, rx_minrto, 60000);
	}

	internal void ShrinkBuf()
	{
		if (snd_buf.Count > 0)
		{
			Segment segment = snd_buf[0];
			snd_una = segment.sn;
		}
		else
		{
			snd_una = snd_nxt;
		}
	}

	internal void ParseAck(uint sn)
	{
		if (Utils.TimeDiff(sn, snd_una) < 0 || Utils.TimeDiff(sn, snd_nxt) >= 0)
		{
			return;
		}
		for (int i = 0; i < snd_buf.Count; i++)
		{
			Segment segment = snd_buf[i];
			if (sn == segment.sn)
			{
				snd_buf.RemoveAt(i);
				SegmentDelete(segment);
				break;
			}
			if (Utils.TimeDiff(sn, segment.sn) < 0)
			{
				break;
			}
		}
	}

	private void ParseUna(uint una)
	{
		int num = 0;
		foreach (Segment item in snd_buf)
		{
			if (Utils.TimeDiff(una, item.sn) > 0)
			{
				num++;
				SegmentDelete(item);
				continue;
			}
			break;
		}
		snd_buf.RemoveRange(0, num);
	}

	private void ParseFastack(uint sn, uint ts)
	{
		if (Utils.TimeDiff(sn, snd_una) < 0 || Utils.TimeDiff(sn, snd_nxt) >= 0)
		{
			return;
		}
		foreach (Segment item in snd_buf)
		{
			if (Utils.TimeDiff(sn, item.sn) < 0)
			{
				break;
			}
			if (sn != item.sn)
			{
				item.fastack++;
			}
		}
	}

	private void AckPush(uint sn, uint ts)
	{
		acklist.Add(new AckItem
		{
			serialNumber = sn,
			timestamp = ts
		});
	}

	private void ParseData(Segment newseg)
	{
		uint sn = newseg.sn;
		if (Utils.TimeDiff(sn, rcv_nxt + rcv_wnd) >= 0 || Utils.TimeDiff(sn, rcv_nxt) < 0)
		{
			SegmentDelete(newseg);
			return;
		}
		InsertSegmentInReceiveBuffer(newseg);
		MoveReceiveBufferDataToReceiveQueue();
	}

	internal void InsertSegmentInReceiveBuffer(Segment newseg)
	{
		bool flag = false;
		int num;
		for (num = rcv_buf.Count - 1; num >= 0; num--)
		{
			Segment segment = rcv_buf[num];
			if (segment.sn == newseg.sn)
			{
				flag = true;
				break;
			}
			if (Utils.TimeDiff(newseg.sn, segment.sn) > 0)
			{
				break;
			}
		}
		if (!flag)
		{
			rcv_buf.Insert(num + 1, newseg);
		}
		else
		{
			SegmentDelete(newseg);
		}
	}

	private void MoveReceiveBufferDataToReceiveQueue()
	{
		int num = 0;
		foreach (Segment item in rcv_buf)
		{
			if (item.sn == rcv_nxt && rcv_queue.Count < rcv_wnd)
			{
				num++;
				rcv_queue.Enqueue(item);
				rcv_nxt++;
				continue;
			}
			break;
		}
		rcv_buf.RemoveRange(0, num);
	}

	public int Input(byte[] data, int offset, int size)
	{
		uint earlier = snd_una;
		uint num = 0u;
		uint ts = 0u;
		int num2 = 0;
		if (data == null || size < 24)
		{
			return -1;
		}
		while (true)
		{
			uint c = 0u;
			uint c2 = 0u;
			uint c3 = 0u;
			uint c4 = 0u;
			uint c5 = 0u;
			ushort c6 = 0;
			byte c7 = 0;
			byte c8 = 0;
			if (size < 24)
			{
				break;
			}
			offset += Utils.Decode32U(data, offset, ref c5);
			if (c5 != conv)
			{
				return -1;
			}
			offset += Utils.Decode8u(data, offset, ref c7);
			offset += Utils.Decode8u(data, offset, ref c8);
			offset += Utils.Decode16U(data, offset, ref c6);
			offset += Utils.Decode32U(data, offset, ref c);
			offset += Utils.Decode32U(data, offset, ref c2);
			offset += Utils.Decode32U(data, offset, ref c4);
			offset += Utils.Decode32U(data, offset, ref c3);
			size -= 24;
			if (size < c3 || (int)c3 < 0)
			{
				return -2;
			}
			if (c7 != 81 && c7 != 82 && c7 != 83 && c7 != 84)
			{
				return -3;
			}
			rmt_wnd = c6;
			ParseUna(c4);
			ShrinkBuf();
			switch (c7)
			{
			case 82:
				if (Utils.TimeDiff(current, c) >= 0)
				{
					UpdateAck(Utils.TimeDiff(current, c));
				}
				ParseAck(c2);
				ShrinkBuf();
				if (num2 == 0)
				{
					num2 = 1;
					num = c2;
					ts = c;
				}
				else if (Utils.TimeDiff(c2, num) > 0)
				{
					num = c2;
					ts = c;
				}
				break;
			case 81:
				if (Utils.TimeDiff(c2, rcv_nxt + rcv_wnd) >= 0)
				{
					break;
				}
				AckPush(c2, c);
				if (Utils.TimeDiff(c2, rcv_nxt) >= 0)
				{
					Segment segment = SegmentNew();
					segment.conv = c5;
					segment.cmd = c7;
					segment.frg = c8;
					segment.wnd = c6;
					segment.ts = c;
					segment.sn = c2;
					segment.una = c4;
					if (c3 != 0)
					{
						segment.data.Write(data, offset, (int)c3);
					}
					ParseData(segment);
				}
				break;
			case 83:
				probe |= 2u;
				break;
			default:
				return -3;
			case 84:
				break;
			}
			offset += (int)c3;
			size -= (int)c3;
		}
		if (num2 != 0)
		{
			ParseFastack(num, ts);
		}
		if (Utils.TimeDiff(snd_una, earlier) > 0 && cwnd < rmt_wnd)
		{
			if (cwnd < ssthresh)
			{
				cwnd++;
				incr += mss;
			}
			else
			{
				if (incr < mss)
				{
					incr = mss;
				}
				incr += mss * mss / incr + mss / 16;
				if ((cwnd + 1) * mss <= incr)
				{
					cwnd = (incr + mss - 1) / ((mss == 0) ? 1 : mss);
				}
			}
			if (cwnd > rmt_wnd)
			{
				cwnd = rmt_wnd;
				incr = rmt_wnd * mss;
			}
		}
		return 0;
	}

	private uint WndUnused()
	{
		if (rcv_queue.Count < rcv_wnd)
		{
			return rcv_wnd - (uint)rcv_queue.Count;
		}
		return 0u;
	}

	public void Flush()
	{
		int size = 0;
		bool flag = false;
		if (!updated)
		{
			return;
		}
		Segment segment = SegmentNew();
		segment.conv = conv;
		segment.cmd = 82u;
		segment.wnd = WndUnused();
		segment.una = rcv_nxt;
		foreach (AckItem item in acklist)
		{
			MakeSpace(24);
			segment.sn = item.serialNumber;
			segment.ts = item.timestamp;
			size += segment.Encode(buffer, size);
		}
		acklist.Clear();
		if (rmt_wnd == 0)
		{
			if (probe_wait == 0)
			{
				probe_wait = 7000u;
				ts_probe = current + probe_wait;
			}
			else if (Utils.TimeDiff(current, ts_probe) >= 0)
			{
				if (probe_wait < 7000)
				{
					probe_wait = 7000u;
				}
				probe_wait += probe_wait / 2;
				if (probe_wait > 120000)
				{
					probe_wait = 120000u;
				}
				ts_probe = current + probe_wait;
				probe |= 1u;
			}
		}
		else
		{
			ts_probe = 0u;
			probe_wait = 0u;
		}
		if ((probe & 1) != 0)
		{
			segment.cmd = 83u;
			MakeSpace(24);
			size += segment.Encode(buffer, size);
		}
		if ((probe & 2) != 0)
		{
			segment.cmd = 84u;
			MakeSpace(24);
			size += segment.Encode(buffer, size);
		}
		probe = 0u;
		uint num = Math.Min(snd_wnd, rmt_wnd);
		if (!nocwnd)
		{
			num = Math.Min(cwnd, num);
		}
		while (Utils.TimeDiff(snd_nxt, snd_una + num) < 0 && snd_queue.Count != 0)
		{
			Segment segment2 = snd_queue.Dequeue();
			segment2.conv = conv;
			segment2.cmd = 81u;
			segment2.wnd = segment.wnd;
			segment2.ts = current;
			segment2.sn = snd_nxt++;
			segment2.una = rcv_nxt;
			segment2.resendts = current;
			segment2.rto = rx_rto;
			segment2.fastack = 0u;
			segment2.xmit = 0u;
			snd_buf.Add(segment2);
		}
		uint num2 = ((fastresend > 0) ? ((uint)fastresend) : uint.MaxValue);
		uint num3 = ((nodelay == 0) ? ((uint)rx_rto >> 3) : 0u);
		int num4 = 0;
		foreach (Segment item2 in snd_buf)
		{
			bool flag2 = false;
			if (item2.xmit == 0)
			{
				flag2 = true;
				item2.xmit++;
				item2.rto = rx_rto;
				item2.resendts = (uint)((int)current + item2.rto) + num3;
			}
			else if (Utils.TimeDiff(current, item2.resendts) >= 0)
			{
				flag2 = true;
				item2.xmit++;
				xmit++;
				if (nodelay == 0)
				{
					item2.rto += Math.Max(item2.rto, rx_rto);
				}
				else
				{
					int num5 = ((nodelay < 2) ? item2.rto : rx_rto);
					item2.rto += num5 / 2;
				}
				item2.resendts = current + (uint)item2.rto;
				flag = true;
			}
			else if (item2.fastack >= num2 && (item2.xmit <= fastlimit || fastlimit <= 0))
			{
				flag2 = true;
				item2.xmit++;
				item2.fastack = 0u;
				item2.resendts = current + (uint)item2.rto;
				num4++;
			}
			if (flag2)
			{
				item2.ts = current;
				item2.wnd = segment.wnd;
				item2.una = rcv_nxt;
				int space = 24 + (int)item2.data.Position;
				MakeSpace(space);
				size += item2.Encode(buffer, size);
				if (item2.data.Position > 0)
				{
					Buffer.BlockCopy(item2.data.GetBuffer(), 0, buffer, size, (int)item2.data.Position);
					size += (int)item2.data.Position;
				}
				if (item2.xmit >= dead_link)
				{
					state = -1;
				}
			}
		}
		SegmentDelete(segment);
		FlushBuffer();
		if (num4 > 0)
		{
			uint num6 = snd_nxt - snd_una;
			ssthresh = num6 / 2;
			if (ssthresh < 2)
			{
				ssthresh = 2u;
			}
			cwnd = ssthresh + num2;
			incr = cwnd * mss;
		}
		if (flag)
		{
			ssthresh = num / 2;
			if (ssthresh < 2)
			{
				ssthresh = 2u;
			}
			cwnd = 1u;
			incr = mss;
		}
		if (cwnd < 1)
		{
			cwnd = 1u;
			incr = mss;
		}
		void FlushBuffer()
		{
			if (size > 0)
			{
				output(buffer, size);
			}
		}
		void MakeSpace(int num7)
		{
			if (size + num7 > mtu)
			{
				output(buffer, size);
				size = 0;
			}
		}
	}

	public void Update(uint currentTimeMilliSeconds)
	{
		current = currentTimeMilliSeconds;
		if (!updated)
		{
			updated = true;
			ts_flush = current;
		}
		int num = Utils.TimeDiff(current, ts_flush);
		if (num >= 10000 || num < -10000)
		{
			ts_flush = current;
			num = 0;
		}
		if (num >= 0)
		{
			ts_flush += interval;
			if (current >= ts_flush)
			{
				ts_flush = current + interval;
			}
			Flush();
		}
	}

	public uint Check(uint current_)
	{
		uint num = ts_flush;
		int num2 = int.MaxValue;
		int num3 = int.MaxValue;
		if (!updated)
		{
			return current_;
		}
		if (Utils.TimeDiff(current_, num) >= 10000 || Utils.TimeDiff(current_, num) < -10000)
		{
			num = current_;
		}
		if (Utils.TimeDiff(current_, num) >= 0)
		{
			return current_;
		}
		num2 = Utils.TimeDiff(num, current_);
		foreach (Segment item in snd_buf)
		{
			int num4 = Utils.TimeDiff(item.resendts, current_);
			if (num4 <= 0)
			{
				return current_;
			}
			if (num4 < num3)
			{
				num3 = num4;
			}
		}
		uint num5 = (uint)((num3 < num2) ? num3 : num2);
		if (num5 >= interval)
		{
			num5 = interval;
		}
		return current_ + num5;
	}

	public void SetMtu(uint mtu)
	{
		if (mtu < 50 || mtu < 24)
		{
			throw new ArgumentException("MTU must be higher than 50 and higher than OVERHEAD");
		}
		buffer = new byte[(mtu + 24) * 3];
		this.mtu = mtu;
		mss = mtu - 24;
	}

	public void SetInterval(uint interval)
	{
		if (interval > 5000)
		{
			interval = 5000u;
		}
		else if (interval < 10)
		{
			interval = 10u;
		}
		this.interval = interval;
	}

	public void SetNoDelay(uint nodelay, uint interval = 100u, int resend = 0, bool nocwnd = false)
	{
		this.nodelay = nodelay;
		if (nodelay != 0)
		{
			rx_minrto = 30;
		}
		else
		{
			rx_minrto = 100;
		}
		if (interval >= 0)
		{
			if (interval > 5000)
			{
				interval = 5000u;
			}
			else if (interval < 10)
			{
				interval = 10u;
			}
			this.interval = interval;
		}
		if (resend >= 0)
		{
			fastresend = resend;
		}
		this.nocwnd = nocwnd;
	}

	public void SetWindowSize(uint sendWindow, uint receiveWindow)
	{
		if (sendWindow != 0)
		{
			snd_wnd = sendWindow;
		}
		if (receiveWindow != 0)
		{
			rcv_wnd = Math.Max(receiveWindow, 128u);
		}
	}
}
