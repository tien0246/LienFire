using UnityEngine;
public enum SkillType { Buff, Projectile, Grenade }
[CreateAssetMenu(menuName = "SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public SkillType type;
    public float cooldown;
    public float value;
    public StatType buffStat;         // dùng khi type = Buff
    public GameObject prefab;         // dùng khi bắn đạn hoặc lựu đạn
    public float duration;            // thời gian buff
}
