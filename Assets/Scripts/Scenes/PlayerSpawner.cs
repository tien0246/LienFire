using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public Transform[] teamASpawnPoints;  // Các điểm spawn cho Team A
    public Transform[] teamBSpawnPoints;  // Các điểm spawn cho Team B

    void Start()
    {
        // Nếu đã vào phòng trước khi Start chạy
        if (PhotonNetwork.InRoom)
        {
            SpawnPlayer();
        }
    }

    // Gọi khi join room thành công
    public override void OnJoinedRoom()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        // Lấy thuộc tính 'team' đã set từ Lobby hoặc trước đó
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("team", out object teamObj))
        {
            string team = (string)teamObj;
            Transform spawnPoint = null;

            if (team == "A" && teamASpawnPoints.Length > 0)
            {
                spawnPoint = teamASpawnPoints[Random.Range(0, teamASpawnPoints.Length)];
            }
            else if (team == "B" && teamBSpawnPoints.Length > 0)
            {
                spawnPoint = teamBSpawnPoints[Random.Range(0, teamBSpawnPoints.Length)];
            }

            if (spawnPoint != null)
            {
                PhotonNetwork.Instantiate("PlayerPrefab", spawnPoint.position, spawnPoint.rotation);
            }
            else
            {
                Debug.LogWarning("No spawn point found for team '" + team + "'.");
            }
        }
        else
        {
            Debug.LogWarning("LocalPlayer has no 'team' property set. Cannot spawn.");
        }
    }
}
