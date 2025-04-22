using UnityEngine;
using Photon.Pun;
public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.Instantiate("PlayerPrefab", new Vector3(Random.Range(-3,3),0,Random.Range(-3,3)), Quaternion.identity);
    }
}
