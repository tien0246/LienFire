using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.Chat;

public class LoginUI : MonoBehaviour
{
	[Header("UI Elements")]
	[SerializeField]
	internal InputField usernameInput;

	[SerializeField]
	internal Button hostButton;

	[SerializeField]
	internal Button clientButton;

	[SerializeField]
	internal Text errorText;

	public static LoginUI instance;

	private void Awake()
	{
		instance = this;
	}

	public void ToggleButtons(string username)
	{
		hostButton.interactable = !string.IsNullOrWhiteSpace(username);
		clientButton.interactable = !string.IsNullOrWhiteSpace(username);
	}
}
