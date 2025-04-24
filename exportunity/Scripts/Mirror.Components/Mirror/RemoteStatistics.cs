using System;
using System.IO;
using Mirror.RemoteCalls;
using UnityEngine;

namespace Mirror;

public class RemoteStatistics : NetworkBehaviour
{
	protected NetworkStatistics NetworkStatistics;

	[Tooltip("Send stats every 'interval' seconds to client.")]
	public float sendInterval = 1f;

	private double lastSendTime;

	[Header("GUI")]
	public bool showGui;

	public KeyCode hotKey = KeyCode.F11;

	private Rect windowRect = new Rect(0f, 0f, 400f, 400f);

	[Header("Authentication")]
	public string passwordFile = "remote_statistics.txt";

	protected bool serverAuthenticated;

	protected bool clientAuthenticated;

	protected string serverPassword;

	protected string clientPassword = "";

	private Stats stats;

	private void LoadPassword()
	{
		string fullPath = Path.GetFullPath(passwordFile);
		if (File.Exists(fullPath))
		{
			try
			{
				serverPassword = File.ReadAllText(fullPath);
				return;
			}
			catch (Exception arg)
			{
				Debug.LogWarning($"RemoteStatistics: failed to read password file: {arg}");
				return;
			}
		}
		Debug.LogWarning("RemoteStatistics: password file has not been created. Authentication will be impossible. Please save the password in: " + fullPath);
	}

	private void OnValidate()
	{
		syncMode = SyncMode.Owner;
	}

	public override void OnStartServer()
	{
		NetworkStatistics = NetworkManager.singleton.GetComponent<NetworkStatistics>();
		if (NetworkStatistics == null)
		{
			throw new Exception("RemoteStatistics requires a NetworkStatistics component on " + NetworkManager.singleton.name + "!");
		}
		LoadPassword();
	}

	public override void OnStartLocalPlayer()
	{
		windowRect.x = (float)(Screen.width / 2) - windowRect.width / 2f;
		windowRect.y = (float)(Screen.height / 2) - windowRect.height / 2f;
	}

	[TargetRpc]
	private void TargetRpcSync(Stats v)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_Mirror_002EStats(writer, v);
		SendTargetRPCInternal(null, "System.Void Mirror.RemoteStatistics::TargetRpcSync(Mirror.Stats)", 1857970636, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void CmdAuthenticate(string v)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(v);
		SendCommandInternal("System.Void Mirror.RemoteStatistics::CmdAuthenticate(System.String)", -1315927130, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void UpdateServer()
	{
		if (serverAuthenticated && NetworkTime.localTime >= lastSendTime + (double)sendInterval)
		{
			lastSendTime = NetworkTime.localTime;
			TargetRpcSync(new Stats(NetworkServer.connections.Count, NetworkTime.time, NetworkServer.tickRate, NetworkServer.actualTickRate, NetworkStatistics.serverSentBytesPerSecond, NetworkStatistics.serverReceivedBytesPerSecond, NetworkServer.tickInterval, NetworkServer.fullUpdateDuration.average, NetworkServer.earlyUpdateDuration.average, NetworkServer.lateUpdateDuration.average, 0.0, 0.0));
		}
	}

	private void UpdateClient()
	{
		if (Input.GetKeyDown(hotKey))
		{
			showGui = !showGui;
		}
	}

	private void Update()
	{
		if (base.isServer)
		{
			UpdateServer();
		}
		if (base.isLocalPlayer)
		{
			UpdateClient();
		}
	}

	private void OnGUI()
	{
		if (base.isLocalPlayer && showGui)
		{
			windowRect = GUILayout.Window(0, windowRect, OnWindow, "Remote Statistics");
			windowRect = Utils.KeepInScreen(windowRect);
		}
	}

	private void GUILayout_TextAndValue(string text, string value)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(text);
		GUILayout.FlexibleSpace();
		GUILayout.Label(value);
		GUILayout.EndHorizontal();
	}

	private void GUILayout_ProgressBar(double ratio, int width)
	{
		ratio = Mathd.Clamp01(ratio);
		GUILayout.HorizontalScrollbar(0f, (float)ratio, 0f, 1f, GUILayout.Width(width));
	}

	private void GUILayout_TextAndProgressBar(string text, double ratio, int progressbarWidth, string caption, int captionWidth, Color captionColor)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(text);
		GUILayout.FlexibleSpace();
		GUILayout_ProgressBar(ratio, progressbarWidth);
		GUI.color = captionColor;
		GUILayout.Label(caption, GUILayout.Width(captionWidth));
		GUI.color = Color.white;
		GUILayout.EndHorizontal();
	}

	private void GUI_Authenticate()
	{
		GUILayout.BeginVertical("Box");
		GUILayout.Label("<b>Authentication</b>");
		GUILayout.Label("<i>Connection is not encrypted. Use with care!</i>");
		clientPassword = GUILayout.PasswordField(clientPassword, '*');
		GUI.enabled = !string.IsNullOrWhiteSpace(clientPassword);
		if (GUILayout.Button("Authenticate"))
		{
			CmdAuthenticate(clientPassword);
		}
		GUI.enabled = true;
		GUILayout.EndVertical();
	}

	private void GUI_General(int connections, double uptime, int configuredTickRate, int actualTickRate)
	{
		GUILayout.BeginVertical("Box");
		GUILayout.Label("<b>General</b>");
		GUILayout_TextAndValue("Connections:", $"<b>{connections}</b>");
		GUILayout_TextAndValue("Uptime:", "<b>" + Utils.PrettySeconds(uptime) + "</b>");
		GUI.color = ((actualTickRate < configuredTickRate) ? Color.red : Color.green);
		GUILayout_TextAndValue("Tick Rate:", $"<b>{actualTickRate} Hz / {configuredTickRate} Hz</b>");
		GUI.color = Color.white;
		GUILayout.EndVertical();
	}

	private void GUI_Traffic(long serverSentBytesPerSecond, long serverReceivedBytesPerSecond)
	{
		GUILayout.BeginVertical("Box");
		GUILayout.Label("<b>Network</b>");
		GUILayout_TextAndValue("Outgoing:", "<b>" + Utils.PrettyBytes(serverSentBytesPerSecond) + "/s</b>");
		GUILayout_TextAndValue("Incoming:", "<b>" + Utils.PrettyBytes(serverReceivedBytesPerSecond) + "/s</b>");
		GUILayout.EndVertical();
	}

	private void GUI_Cpu(float serverTickInterval, double fullUpdateAvg, double serverEarlyAvg, double serverLateAvg, double transportEarlyAvg, double transportLateAvg)
	{
		GUILayout.BeginVertical("Box");
		GUILayout.Label("<b>CPU</b>");
		double num = fullUpdateAvg / (double)serverTickInterval;
		GUILayout_TextAndProgressBar("World Update Avg:", num, 120, $"<b>{fullUpdateAvg * 1000.0:F1} ms</b>", 90, (num <= 0.9) ? Color.green : Color.red);
		double num2 = (serverEarlyAvg + serverLateAvg) / (double)serverTickInterval;
		GUILayout_TextAndProgressBar("Server Update Avg:", num2, 120, $"<b>{serverEarlyAvg * 1000.0:F1} + {serverLateAvg * 1000.0:F1} ms</b>", 90, (num2 <= 0.9) ? Color.green : Color.red);
		GUILayout.EndVertical();
	}

	private void GUI_Notice()
	{
	}

	private void OnWindow(int windowID)
	{
		if (!clientAuthenticated)
		{
			GUI_Authenticate();
		}
		else
		{
			GUI_General(stats.connections, stats.uptime, stats.configuredTickRate, stats.actualTickRate);
			GUI_Traffic(stats.sentBytesPerSecond, stats.receiveBytesPerSecond);
			GUI_Cpu(stats.serverTickInterval, stats.fullUpdateAvg, stats.serverEarlyAvg, stats.serverLateAvg, stats.transportEarlyAvg, stats.transportLateAvg);
			GUI_Notice();
		}
		GUI.DragWindow(new Rect(0f, 0f, 10000f, 10000f));
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_TargetRpcSync__Stats(Stats v)
	{
		clientAuthenticated = true;
		stats = v;
	}

	protected static void InvokeUserCode_TargetRpcSync__Stats(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC TargetRpcSync called on server.");
		}
		else
		{
			((RemoteStatistics)obj).UserCode_TargetRpcSync__Stats(GeneratedNetworkCode._Read_Mirror_002EStats(reader));
		}
	}

	protected void UserCode_CmdAuthenticate__String(string v)
	{
		if (!string.IsNullOrWhiteSpace(serverPassword) && serverPassword.Equals(v))
		{
			serverAuthenticated = true;
			Debug.Log($"RemoteStatistics: connectionId {base.connectionToClient.connectionId} authenticated with player {base.name}");
		}
	}

	protected static void InvokeUserCode_CmdAuthenticate__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdAuthenticate called on client.");
		}
		else
		{
			((RemoteStatistics)obj).UserCode_CmdAuthenticate__String(reader.ReadString());
		}
	}

	static RemoteStatistics()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(RemoteStatistics), "System.Void Mirror.RemoteStatistics::CmdAuthenticate(System.String)", InvokeUserCode_CmdAuthenticate__String, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(RemoteStatistics), "System.Void Mirror.RemoteStatistics::TargetRpcSync(Mirror.Stats)", InvokeUserCode_TargetRpcSync__Stats);
	}
}
