using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MainMenuUI : MonoBehaviourPunCallbacks
{
    public TMP_InputField nameInput;
    public TMP_InputField roomInput;

    public void OnClickSolo()
    {
        if (nameInput.text == "") nameInput.text = "Player" + Random.Range(0, 1000);
        PhotonNetwork.NickName = nameInput.text;
        PhotonNetwork.OfflineMode = true;
        SceneManager.LoadScene("GameScene");
    }

    public void OnClickOnline()
    {
        if (nameInput.text == "") nameInput.text = "Player" + Random.Range(0, 1000);
        PhotonNetwork.NickName = nameInput.text;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        if (roomInput.text != "")
        {
            RoomOptions opt = new RoomOptions { MaxPlayers = 6 };
            PhotonNetwork.JoinOrCreateRoom(roomInput.text, opt, TypedLobby.Default);
        }
        else
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinRandomFailed(short code, string msg)
    {
        string roomName = Random.Range(1000, 9999).ToString();
        RoomOptions opt = new RoomOptions { MaxPlayers = 6 };
        PhotonNetwork.CreateRoom(roomName, opt);
    }

    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
