using UnityEngine;
using UnityEngine.UI;
public class StartMatchButton : MonoBehaviour
{
    [SerializeField] LobbyManager lobby;
    void Start() => GetComponent<Button>().onClick.AddListener(lobby.StartMatch);
}
