using UnityEngine;
[CreateAssetMenu(menuName="HeroStats")]
public class HeroStats : ScriptableObject
{
    public string heroName;
    public Sprite icon;
    public float maxHP, baseDamage, attackSpeed, moveSpeed, attackRange;
    public SkillData[] skills; // Q, E, R
    public SkillData basicAttack;
    public SkillData supportSkill; // F
}
