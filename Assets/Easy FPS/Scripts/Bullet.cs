using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Zombie"))
        {
            // Kiểm tra loại Zombie và gây damage
            ZombieAI zombie = collision.collider.GetComponent<ZombieAI>();

            if (zombie != null)
            {
                int damage = 1;  // Mỗi viên đạn gây 1 damage
                zombie.TakeDamage(damage);  // Gọi hàm TakeDamage của Zombie
            }

            Destroy(gameObject);  // Hủy viên đạn sau khi va chạm
        }
    }
}
