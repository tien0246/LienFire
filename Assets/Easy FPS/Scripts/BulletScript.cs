using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float maxDistance = 100f;  // Khoảng cách tối đa viên đạn có thể bay
    RaycastHit hit;
    public GameObject decalHitWall;
    public float floatInfrontOfWall = 0.1f;
    public GameObject bloodEffect;
    public LayerMask ignoreLayer;  // Để bỏ qua va chạm với Layer của Player hoặc Weapon

    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, ~ignoreLayer)) // ~ignoreLayer là để bỏ qua các Layer không muốn va chạm
        {
            // Kiểm tra xem Raycast có va chạm với Zombie không
            if (hit.collider.CompareTag("Zombie"))
            {
                ZombieAI zombie = hit.collider.GetComponent<ZombieAI>();

                if (zombie != null)
                {
                    // Tạo hiệu ứng máu khi viên đạn trúng Zombie
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));

                    // Gọi phương thức TakeDamage để giảm máu của Zombie
                    zombie.TakeDamage(1);  // 1 là damage của viên đạn
                }

                Destroy(gameObject);  // Hủy viên đạn sau khi va chạm
            }
            // Kiểm tra va chạm với bề 
        }
        // Hủy viên đạn nếu không va chạm
        Destroy(gameObject, 0.1f);
    }
}
