using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerBoost : MonoBehaviour
{
    private CharacterController characterController;

    private float originalSpeed;
    private float originalJumpHeight;
    private float currentSpeed;
    private float currentJumpHeight;

    private float gravity = -9.8f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lưu lại các giá trị ban đầu của speed và jumpHeight
        originalSpeed = 5f;  // Gán giá trị speed mặc định
        originalJumpHeight = 2f;  // Gán giá trị jumpHeight mặc định
        currentSpeed = originalSpeed;
        currentJumpHeight = originalJumpHeight;
    }

    void Update()
    {
        // Di chuyển người chơi
        MovePlayer();

        // Kiểm tra nếu người chơi nhảy
        if (characterController.isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= currentSpeed;

        moveDirection.y += gravity * Time.deltaTime;  // Thêm lực trọng lực cho nhảy

        characterController.Move(moveDirection * Time.deltaTime);
    }

    void Jump()
    {
        Vector3 jump = new Vector3(0, currentJumpHeight, 0);
        characterController.Move(jump * Time.deltaTime);
    }

    public void ActivateBoost(float speed, float jumpHeight, float duration)
    {
        currentSpeed = originalSpeed + speed;  // Tăng tốc độ chạy
        currentJumpHeight = originalJumpHeight + jumpHeight;  // Tăng chiều cao nhảy

        // Hủy boost sau một khoảng thời gian
        StartCoroutine(ResetBoost(duration));
    }

    private IEnumerator ResetBoost(float duration)
    {
        yield return new WaitForSeconds(duration);

        currentSpeed = originalSpeed;  // Khôi phục tốc độ gốc
        currentJumpHeight = originalJumpHeight;  // Khôi phục chiều cao nhảy gốc
    }
}
