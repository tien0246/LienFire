using UnityEngine;
using UnityEngine.UI;
public class SwitchTeamButton : MonoBehaviour
{
    [SerializeField] LobbyManager lobby;
    void Start() => GetComponent<Button>().onClick.AddListener(() => lobby.SwitchLocalTeam());
}
