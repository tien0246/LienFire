using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    const string TEAM_KEY = "team";

    [SerializeField] PlayerSlotUI[] teamASlots;
    [SerializeField] PlayerSlotUI[] teamBSlots;
    [SerializeField] Button startButton;
    [SerializeField] Text roomIdText;

    void Start()
    {
        StartCoroutine(InitWhenReady());
    }

    IEnumerator InitWhenReady()
    {
        yield return new WaitUntil(() => PhotonNetwork.InRoom && PhotonNetwork.NetworkClientState == ClientState.Joined);

        roomIdText.text = "Room: " + PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.IsMasterClient)
        {
            var ht = new ExitGames.Client.Photon.Hashtable { [TEAM_KEY] = "A" };
            PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
        }
        else
        {
            AssignInitialTeam();
        }

        UpdateAll();
    }

    void AssignInitialTeam()
    {
        string team = Count("A") <= Count("B") ? "A" : "B"; // Đảm bảo đội có số người ít hơn được chọn
        var ht = new ExitGames.Client.Photon.Hashtable { [TEAM_KEY] = team };
        PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
    }

    int Count(string t)
    {
        int c = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
            if (p.CustomProperties.TryGetValue(TEAM_KEY, out var v) && (string)v == t) c++;
        return c;
    }

    void UpdateAll() { UpdateSlots(); UpdateStartButton(); }

    void UpdateSlots()
    {
        foreach (var s in teamASlots) s.SetEmpty();
        foreach (var s in teamBSlots) s.SetEmpty();

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!p.CustomProperties.ContainsKey(TEAM_KEY)) continue;
            string t = (string)p.CustomProperties[TEAM_KEY];
            if (t == "A")
            {
                if (p.IsMasterClient) teamASlots[1].SetPlayer(p.NickName);
                else FillNext(teamASlots, p.NickName, 1);
            }
            else
            {
                FillNext(teamBSlots, p.NickName, -1);
            }
        }
    }

    void FillNext(PlayerSlotUI[] arr, string n, int skip)
    {
        for (int i = 0; i < arr.Length; i++)
            if (i != skip && arr[i].IsEmpty()) { arr[i].SetPlayer(n); break; }
    }

    void UpdateStartButton()
    {
        if (!PhotonNetwork.IsMasterClient) { startButton.gameObject.SetActive(false); return; }
        startButton.gameObject.SetActive(true);
        startButton.interactable = Count("A") > 0 && Count("B") > 0;
    }

    public void StartMatch()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel("Map_v2");  // Chuyển sang scene "Map_v2" khi trận đấu bắt đầu
    }

    public void SwitchLocalTeam()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient) return;
        string cur = (string)PhotonNetwork.LocalPlayer.CustomProperties[TEAM_KEY];
        string target = cur == "A" ? "B" : "A";
        if (Count(target) >= 3) return;
        var ht = new ExitGames.Client.Photon.Hashtable { [TEAM_KEY] = target };
        PhotonNetwork.LocalPlayer.SetCustomProperties(ht);

        // Không spawn trong lobby, spawn lại khi vào trận
    }

    public override void OnPlayerEnteredRoom(Player _) => UpdateAll();
    public override void OnPlayerLeftRoom(Player _) => UpdateAll();
    public override void OnPlayerPropertiesUpdate(Player _, ExitGames.Client.Photon.Hashtable __) => UpdateAll();

    public override void OnJoinedRoom()
    {
        string team = Count("A") <= Count("B") ? "A" : "B";  // Chọn đội có số người ít hơn

        // Lưu đội của người chơi vào Custom Properties
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
        playerProperties["team"] = team;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

        UpdateAll();  // Cập nhật UI sau khi gia nhập phòng
    }
}
