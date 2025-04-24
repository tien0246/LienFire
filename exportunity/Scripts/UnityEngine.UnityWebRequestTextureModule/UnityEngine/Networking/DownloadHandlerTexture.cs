using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.Bindings;

namespace UnityEngine.Networking;

[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Modules/UnityWebRequestTexture/Public/DownloadHandlerTexture.h")]
public sealed class DownloadHandlerTexture : DownloadHandler
{
	private NativeArray<byte> m_NativeData;

	private Texture2D mTexture;

	private bool mHasTexture;

	private bool mNonReadable;

	public Texture2D texture => InternalGetTexture();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Create(DownloadHandlerTexture obj, bool readable);

	private void InternalCreateTexture(bool readable)
	{
		m_Ptr = Create(this, readable);
	}

	public DownloadHandlerTexture()
	{
		InternalCreateTexture(readable: true);
	}

	public DownloadHandlerTexture(bool readable)
	{
		InternalCreateTexture(readable);
		mNonReadable = !readable;
	}

	protected override NativeArray<byte> GetNativeData()
	{
		return DownloadHandler.InternalGetNativeArray(this, ref m_NativeData);
	}

	public override void Dispose()
	{
		DownloadHandler.DisposeNativeArray(ref m_NativeData);
		base.Dispose();
	}

	private Texture2D InternalGetTexture()
	{
		if (mHasTexture)
		{
			if (mTexture == null)
			{
				mTexture = new Texture2D(2, 2);
				mTexture.LoadImage(GetData(), mNonReadable);
			}
		}
		else if (mTexture == null)
		{
			try
			{
				mTexture = InternalGetTextureNative();
				mHasTexture = true;
			}
			finally
			{
				ClearNativeTexture();
			}
		}
		return mTexture;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private extern Texture2D InternalGetTextureNative();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void ClearNativeTexture();

	public static Texture2D GetContent(UnityWebRequest www)
	{
		return DownloadHandler.GetCheckedDownloader<DownloadHandlerTexture>(www).texture;
	}
}
