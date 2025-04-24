using System;
using System.Security.Cryptography;

namespace Mirror.SimpleWeb;

internal sealed class MaskHelper : IDisposable
{
	private readonly byte[] maskBuffer;

	private readonly RNGCryptoServiceProvider random;

	public MaskHelper()
	{
		maskBuffer = new byte[4];
		random = new RNGCryptoServiceProvider();
	}

	public void Dispose()
	{
		random.Dispose();
	}

	public int WriteMask(byte[] buffer, int offset)
	{
		random.GetBytes(maskBuffer);
		Buffer.BlockCopy(maskBuffer, 0, buffer, offset, 4);
		return offset + 4;
	}
}
