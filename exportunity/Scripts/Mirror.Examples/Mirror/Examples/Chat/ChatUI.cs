using System.Collections;
using System.Collections.Generic;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.Chat;

public class ChatUI : NetworkBehaviour
{
	[Header("UI Elements")]
	[SerializeField]
	private Text chatHistory;

	[SerializeField]
	private Scrollbar scrollbar;

	[SerializeField]
	private InputField chatMessage;

	[SerializeField]
	private Button sendButton;

	internal static string localPlayerName;

	internal static readonly Dictionary<NetworkConnectionToClient, string> connNames;

	public override void OnStartServer()
	{
		connNames.Clear();
	}

	public override void OnStartClient()
	{
		chatHistory.text = "";
	}

	[Command(requiresAuthority = false)]
	private void CmdSend(string message, NetworkConnectionToClient sender = null)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(message);
		SendCommandInternal("System.Void Mirror.Examples.Chat.ChatUI::CmdSend(System.String,Mirror.NetworkConnectionToClient)", -1640312036, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcReceive(string playerName, string message)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(playerName);
		writer.WriteString(message);
		SendRPCInternal("System.Void Mirror.Examples.Chat.ChatUI::RpcReceive(System.String,System.String)", -1186680841, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void AppendMessage(string message)
	{
		StartCoroutine(AppendAndScroll(message));
	}

	private IEnumerator AppendAndScroll(string message)
	{
		Text text = chatHistory;
		text.text = text.text + message + "\n";
		yield return null;
		yield return null;
		scrollbar.value = 0f;
	}

	public void ExitButtonOnClick()
	{
		NetworkManager.singleton.StopHost();
	}

	public void ToggleButton(string input)
	{
		sendButton.interactable = !string.IsNullOrWhiteSpace(input);
	}

	public void OnEndEdit(string input)
	{
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetButtonDown("Submit"))
		{
			SendMessage();
		}
	}

	public void SendMessage()
	{
		if (!string.IsNullOrWhiteSpace(chatMessage.text))
		{
			CmdSend(chatMessage.text.Trim());
			chatMessage.text = string.Empty;
			chatMessage.ActivateInputField();
		}
	}

	static ChatUI()
	{
		connNames = new Dictionary<NetworkConnectionToClient, string>();
		RemoteProcedureCalls.RegisterCommand(typeof(ChatUI), "System.Void Mirror.Examples.Chat.ChatUI::CmdSend(System.String,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdSend__String__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterRpc(typeof(ChatUI), "System.Void Mirror.Examples.Chat.ChatUI::RpcReceive(System.String,System.String)", InvokeUserCode_RpcReceive__String__String);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_CmdSend__String__NetworkConnectionToClient(string message, NetworkConnectionToClient sender)
	{
		if (!connNames.ContainsKey(sender))
		{
			connNames.Add(sender, sender.identity.GetComponent<Player>().playerName);
		}
		if (!string.IsNullOrWhiteSpace(message))
		{
			RpcReceive(connNames[sender], message.Trim());
		}
	}

	protected static void InvokeUserCode_CmdSend__String__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSend called on client.");
		}
		else
		{
			((ChatUI)obj).UserCode_CmdSend__String__NetworkConnectionToClient(reader.ReadString(), senderConnection);
		}
	}

	protected void UserCode_RpcReceive__String__String(string playerName, string message)
	{
		string message2 = ((playerName == localPlayerName) ? ("<color=red>" + playerName + ":</color> " + message) : ("<color=blue>" + playerName + ":</color> " + message));
		AppendMessage(message2);
	}

	protected static void InvokeUserCode_RpcReceive__String__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcReceive called on server.");
		}
		else
		{
			((ChatUI)obj).UserCode_RpcReceive__String__String(reader.ReadString(), reader.ReadString());
		}
	}
}
