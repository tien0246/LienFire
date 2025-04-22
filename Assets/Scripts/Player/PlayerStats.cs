using UnityEngine;
using Photon.Pun;
using System.Collections;
public class PlayerStats : MonoBehaviourPun
{
    public HeroStats data;
    public float hp;
    public float damage;
    public float atkSpeed;
    public float atkRange;
    public float moveSpeed;
    void Awake()
    {
        hp = data.maxHP;
        damage = data.baseDamage;
        atkSpeed = data.attackSpeed;
        atkRange = data.attackRange;
        moveSpeed = data.moveSpeed;
    }
    public void ApplyBuff(StatType type, float value, float dur)
    {
        photonView.RPC("RPC_Buff", RpcTarget.All, type, value, dur);
    }
    [PunRPC] void RPC_Buff(StatType t, float v, float d)
    {
        StartCoroutine(BuffRoutine(t, v, d));
    }
    IEnumerator BuffRoutine(StatType t, float v, float d)
    {
        Modify(t, v);
        yield return new WaitForSeconds(d);
        Modify(t, -v);
    }
    void Modify(StatType t, float v)
    {
        if (t == StatType.Damage) damage += v;
        else if (t == StatType.AttackSpeed) atkSpeed += v;
        else if (t == StatType.MoveSpeed) moveSpeed += v;
        else if (t == StatType.Range) atkRange += v;
        else hp = Mathf.Min(hp + v, data.maxHP);
    }
}
