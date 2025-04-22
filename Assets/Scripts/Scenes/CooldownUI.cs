using UnityEngine;
using TMPro;
public class CooldownUI : MonoBehaviour
{
    public SkillManager sm;
    public TextMeshProUGUI[] cdText;
    void Update(){
        for(int i=0;i<cdText.Length;i++)
            cdText[i].text=Mathf.Max(0,Mathf.Ceil(sm.GetCooldown(i))).ToString();
    }
}