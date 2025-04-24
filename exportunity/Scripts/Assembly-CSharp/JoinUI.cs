using System;
using System.Net;
using System.Net.Sockets;
using Mirror;
using TMPro;
using UnityEngine;

public class JoinUI : MonoBehaviour
{
	public TMP_InputField ip_input;

	public GameObject netUI;

	public TextMeshProUGUI ipText;

	private NetworkManager net;

	private void Awake()
	{
		net = GetComponent<NetworkManager>();
	}

	public void HostB()
	{
		net.StartHost();
		GetLocalIPAddress();
		netUI.SetActive(value: false);
	}

	public void JoinB()
	{
		net.networkAddress = ip_input.text;
		net.StartClient();
		netUI.SetActive(value: false);
	}

	public string GetLocalIPAddress()
	{
		IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
		foreach (IPAddress iPAddress in addressList)
		{
			if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
			{
				ipText.text = iPAddress.ToString();
				return iPAddress.ToString();
			}
		}
		throw new Exception("No network adapters with an IPv4 address in the system!");
	}
}
