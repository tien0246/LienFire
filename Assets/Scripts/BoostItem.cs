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
            // Gọi hàm để tăng tốc độ và nhảy
            other.GetComponent<PlayerBoost>().ActivateBoost(speedBoost, jumpBoost, boostDuration);
            Destroy(gameObject);  // Hủy vật phẩm sau khi nhặt
        }
    }
}
