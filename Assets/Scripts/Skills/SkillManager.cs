using UnityEngine;
using Photon.Pun;
public class SkillManager : MonoBehaviourPun
{
    public PlayerStats stats;
    float[] cds;
    void Start() { cds = new float[stats.data.skills.Length]; }
    void Update()
    {
        if (!photonView.IsMine) return;
        for (int i = 0; i < cds.Length; i++) cds[i] -= Time.deltaTime;
        if (Input.GetMouseButtonDown(0)) TryCast(4);
        if (Input.GetKeyDown(KeyCode.Q)) TryCast(0);
        if (Input.GetKeyDown(KeyCode.E)) TryCast(1);
        if (Input.GetKeyDown(KeyCode.R)) TryCast(2);
        if (Input.GetKeyDown(KeyCode.F)) TryCast(3);
    }
    void TryCast(int id)
    {
        if (cds[id] > 0) return;
        photonView.RPC("ExecuteSkill", RpcTarget.All, id);
        cds[id] = stats.data.skills[id].cooldown;
    }
    [PunRPC] void ExecuteSkill(int id)
    {
        var s = stats.data.skills[id];
        if (s.type == SkillType.Buff)
            stats.ApplyBuff(s.buffStat, s.value, s.duration);
        else if (s.type == SkillType.Projectile)
            Instantiate(s.prefab, transform.position + transform.forward, Quaternion.LookRotation(transform.forward)).GetComponent<Rigidbody>().velocity = transform.forward * 20;
        else if (s.type == SkillType.Grenade)
        {
            var g = Instantiate(s.prefab, transform.position + Vector3.up, Quaternion.identity).GetComponent<Rigidbody>();
            g.AddForce(transform.forward * 15 + Vector3.up * 5, ForceMode.Impulse);
        }
    }
    public float GetCooldown(int i) => cds[i];
}
