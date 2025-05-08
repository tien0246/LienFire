using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    public GameObject bulletPrefab;  // Prefab viên đạn
    public float bulletSpeed = 20f;  // Tốc độ viên đạn
    public Transform playerTransform; // Transform của Player (hoặc Capsule)
    public Vector3 shootOffset = new Vector3(0, 1, 2); // Vị trí bắn tính từ Player

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))  // Chuột trái để bắn
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Tính toán vị trí shootPoint từ Player (hoặc Capsule)
        Vector3 shootPoint = playerTransform.position + shootOffset;  // Vị trí đầu nòng súng (hoặc tay)

        // Tạo viên đạn tại shootPoint và hướng về phía trước
        GameObject bullet = Instantiate(bulletPrefab, shootPoint, playerTransform.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = playerTransform.forward * bulletSpeed;  // Bắn viên đạn ra từ hướng của Player
    }
}
