using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace Mirror.Discovery;

[DisallowMultipleComponent]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-discovery")]
public abstract class NetworkDiscoveryBase<Request, Response> : MonoBehaviour where Request : NetworkMessage where Response : NetworkMessage
{
	[SerializeField]
	[Tooltip("If true, broadcasts a discovery request every ActiveDiscoveryInterval seconds")]
	public bool enableActiveDiscovery = true;

	[Tooltip("iOS may require LAN IP address here (e.g. 192.168.x.x), otherwise leave blank.")]
	public string BroadcastAddress = "";

	[SerializeField]
	[Tooltip("The UDP port the server will listen for multi-cast messages")]
	protected int serverBroadcastListenPort = 47777;

	[SerializeField]
	[Tooltip("Time in seconds between multi-cast messages")]
	[Range(1f, 60f)]
	private float ActiveDiscoveryInterval = 3f;

	[Tooltip("Transport to be advertised during discovery")]
	public Transport transport;

	[Tooltip("Invoked when a server is found")]
	public ServerFoundUnityEvent OnServerFound;

	[HideInInspector]
	public long secretHandshake;

	protected UdpClient serverUdpClient;

	protected UdpClient clientUdpClient;

	private AndroidJavaObject multicastLock;

	private bool hasMulticastLock;

	public static bool SupportedOnThisPlatform => Application.platform != RuntimePlatform.WebGLPlayer;

	public long ServerId { get; private set; }

	public virtual void Start()
	{
		ServerId = RandomLong();
		if (transport == null)
		{
			transport = Transport.active;
		}
	}

	public static long RandomLong()
	{
		int num = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		int num2 = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		return num + ((long)num2 << 32);
	}

	private void OnApplicationQuit()
	{
		Shutdown();
	}

	private void OnDisable()
	{
		Shutdown();
	}

	private void OnDestroy()
	{
		Shutdown();
	}

	private void Shutdown()
	{
		EndpMulticastLock();
		if (serverUdpClient != null)
		{
			try
			{
				serverUdpClient.Close();
			}
			catch (Exception)
			{
			}
			serverUdpClient = null;
		}
		if (clientUdpClient != null)
		{
			try
			{
				clientUdpClient.Close();
			}
			catch (Exception)
			{
			}
			clientUdpClient = null;
		}
		CancelInvoke();
	}

	public void AdvertiseServer()
	{
		if (!SupportedOnThisPlatform)
		{
			throw new PlatformNotSupportedException("Network discovery not supported in this platform");
		}
		StopDiscovery();
		serverUdpClient = new UdpClient(serverBroadcastListenPort)
		{
			EnableBroadcast = true,
			MulticastLoopback = false
		};
		ServerListenAsync();
	}

	public async Task ServerListenAsync()
	{
		BeginMulticastLock();
		while (true)
		{
			try
			{
				await ReceiveRequestAsync(serverUdpClient);
			}
			catch (ObjectDisposedException)
			{
				break;
			}
			catch (Exception)
			{
			}
		}
	}

	private async Task ReceiveRequestAsync(UdpClient udpClient)
	{
		UdpReceiveResult udpReceiveResult = await udpClient.ReceiveAsync();
		using NetworkReaderPooled networkReaderPooled = NetworkReaderPool.Get(udpReceiveResult.Buffer);
		if (networkReaderPooled.ReadLong() != secretHandshake)
		{
			throw new ProtocolViolationException("Invalid handshake");
		}
		Request request = networkReaderPooled.Read<Request>();
		ProcessClientRequest(request, udpReceiveResult.RemoteEndPoint);
	}

	protected virtual void ProcessClientRequest(Request request, IPEndPoint endpoint)
	{
		Response val = ProcessRequest(request, endpoint);
		if (val == null)
		{
			return;
		}
		using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		try
		{
			networkWriterPooled.WriteLong(secretHandshake);
			networkWriterPooled.Write(val);
			ArraySegment<byte> arraySegment = networkWriterPooled.ToArraySegment();
			serverUdpClient.Send(arraySegment.Array, arraySegment.Count, endpoint);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception, this);
		}
	}

	protected abstract Response ProcessRequest(Request request, IPEndPoint endpoint);

	private void BeginMulticastLock()
	{
		if (hasMulticastLock || Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		using AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
		using AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getSystemService", new object[1] { "wifi" });
		multicastLock = androidJavaObject2.Call<AndroidJavaObject>("createMulticastLock", new object[1] { "lock" });
		multicastLock.Call("acquire");
		hasMulticastLock = true;
	}

	private void EndpMulticastLock()
	{
		if (hasMulticastLock)
		{
			multicastLock?.Call("release");
			hasMulticastLock = false;
		}
	}

	public void StartDiscovery()
	{
		if (!SupportedOnThisPlatform)
		{
			throw new PlatformNotSupportedException("Network discovery not supported in this platform");
		}
		StopDiscovery();
		try
		{
			clientUdpClient = new UdpClient(0)
			{
				EnableBroadcast = true,
				MulticastLoopback = false
			};
		}
		catch (Exception)
		{
			Shutdown();
			throw;
		}
		ClientListenAsync();
		if (enableActiveDiscovery)
		{
			InvokeRepeating("BroadcastDiscoveryRequest", 0f, ActiveDiscoveryInterval);
		}
	}

	public void StopDiscovery()
	{
		Shutdown();
	}

	public async Task ClientListenAsync()
	{
		while (clientUdpClient != null)
		{
			try
			{
				await ReceiveGameBroadcastAsync(clientUdpClient);
			}
			catch (ObjectDisposedException)
			{
				break;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}

	public void BroadcastDiscoveryRequest()
	{
		if (clientUdpClient == null)
		{
			return;
		}
		if (NetworkClient.isConnected)
		{
			StopDiscovery();
			return;
		}
		IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, serverBroadcastListenPort);
		if (!string.IsNullOrWhiteSpace(BroadcastAddress))
		{
			try
			{
				endPoint = new IPEndPoint(IPAddress.Parse(BroadcastAddress), serverBroadcastListenPort);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
		using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		networkWriterPooled.WriteLong(secretHandshake);
		try
		{
			Request request = GetRequest();
			networkWriterPooled.Write(request);
			ArraySegment<byte> arraySegment = networkWriterPooled.ToArraySegment();
			clientUdpClient.SendAsync(arraySegment.Array, arraySegment.Count, endPoint);
		}
		catch (Exception)
		{
		}
	}

	protected virtual Request GetRequest()
	{
		return default(Request);
	}

	private async Task ReceiveGameBroadcastAsync(UdpClient udpClient)
	{
		UdpReceiveResult udpReceiveResult = await udpClient.ReceiveAsync();
		using NetworkReaderPooled networkReaderPooled = NetworkReaderPool.Get(udpReceiveResult.Buffer);
		if (networkReaderPooled.ReadLong() == secretHandshake)
		{
			Response response = networkReaderPooled.Read<Response>();
			ProcessResponse(response, udpReceiveResult.RemoteEndPoint);
		}
	}

	protected abstract void ProcessResponse(Response response, IPEndPoint endpoint);
}
