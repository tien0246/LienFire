using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerBoost : MonoBehaviour
{
    private Rigidbody rb;  // Rigidbody của Player

    private float originalSpeed;
    private float currentSpeed;

    private float originalJumpForce;
    private float currentJumpForce;

    private float gravity = -9.8f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Lấy Rigidbody của Player

        // Lưu lại tốc độ và lực nhảy ban đầu
        originalSpeed = 5f;  // Tốc độ mặc định
        originalJumpForce = 500f;  // Lực nhảy mặc định
        currentSpeed = originalSpeed;
        currentJumpForce = originalJumpForce;
    }

    void Update()
    {
        // Di chuyển Player
        MovePlayer();

        // Kiểm tra nếu người chơi nhảy
        if (rb.velocity.y == 0 && Input.GetButtonDown("Jump"))  // Kiểm tra Player có đứng trên mặt đất không
        {
            Jump();
        }
    }

    void MovePlayer()
    {
        // Di chuyển dựa trên Input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized * currentSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + moveDirection);
    }

    public void ActivateBoost(float speedBoost, float jumpBoost, float duration)
    {
        currentSpeed = originalSpeed + speedBoost;  // Tăng tốc độ chạy
        currentJumpForce = originalJumpForce + jumpBoost;  // Tăng chiều cao nhảy

        // Hủy boost sau một khoảng thời gian
        StartCoroutine(ResetBoost(duration));
    }

    private IEnumerator ResetBoost(float duration)
    {
        yield return new WaitForSeconds(duration);

        currentSpeed = originalSpeed;  // Khôi phục tốc độ gốc
        currentJumpForce = originalJumpForce;  // Khôi phục chiều cao nhảy gốc
    }

    // Hàm nhảy
    public void Jump()
    {
        if (rb.velocity.y == 0)  // Kiểm tra nếu Player đang đứng trên mặt đất
        {
            rb.AddForce(Vector3.up * currentJumpForce);  // Thêm lực nhảy vào Player
        }
    }
}
