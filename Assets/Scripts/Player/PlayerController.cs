using UnityEngine;
using Photon.Pun;
public class PlayerController : MonoBehaviourPun
{
    public float moveSpeed = 6;
    public float rotateSpeed = 720;
    CharacterController cc;
    Camera cam;
    void Start()
    {
        cc = GetComponent<CharacterController>();
        if (photonView.IsMine) cam = Camera.main;
        else GetComponentInChildren<Cinemachine.CinemachineFreeLook>().enabled = false;
    }
    void Update()
    {
        if (!photonView.IsMine) return;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v);
        if (dir.sqrMagnitude > 0.01f)
        {
            dir = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * dir;
            cc.Move(dir.normalized * moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), rotateSpeed * Time.deltaTime);
        }
    }
}
