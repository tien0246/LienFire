using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Networking;

[StructLayout(LayoutKind.Sequential)]
[Obsolete("MovieTexture is deprecated. Use VideoPlayer instead.", true)]
public sealed class DownloadHandlerMovieTexture : DownloadHandler
{
}
