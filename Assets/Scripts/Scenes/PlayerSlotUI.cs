using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image avatar;
    public void SetEmpty()
    {
        nameText.text = "Empty Slot";
        avatar.enabled = false;
    }
    public void SetPlayer(string playerName)
    {
        nameText.text = playerName;
        avatar.enabled = true;
    }
    public bool IsEmpty() => !avatar.enabled;
}
