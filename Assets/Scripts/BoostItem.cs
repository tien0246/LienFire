using UnityEngine;

public class BoostItem : MonoBehaviour
{
    public float speedBoost = 2f;  // Tăng tốc độ chạy
    public float jumpBoost = 2f;   // Tăng chiều cao nhảy
    public float boostDuration = 5f;  // Thời gian vật phẩm có hiệu lực

    void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem có phải Player nhặt vật phẩm không
        if (other.CompareTag("Player"))
        {
            PlayerMovementScript playerBoost = other.GetComponent<PlayerMovementScript>();
            if (playerBoost != null)
            {
                // Gọi hàm ActivateBoost từ PlayerBoost
                playerBoost.maxSpeed += 5;
                playerBoost.currentSpeed += 5;  
            }
            Destroy(gameObject);  // Hủy vật phẩm sau khi nhặt
        }
    }
}
