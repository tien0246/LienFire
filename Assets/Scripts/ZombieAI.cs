using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public enum ZombieType { Fast, Strong }  // Enum để phân biệt loại zombie
    public ZombieType zombieType;  // Kiểu zombie (Fast hoặc Strong)
    public int health = 1;  // Số lần hit zombie phải chịu trước khi chết
    private NavMeshAgent agent;  // NavMeshAgent để điều khiển di chuyển của Zombie
    private Transform player;  // Player mà Zombie sẽ di chuyển tới

    public GameObject boostItemPrefab;  // Prefab của BoostItem sẽ xuất hiện khi Zombie chết

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;  // Tìm Player trong scene
        SetZombieAttributes();  // Cấu hình các thuộc tính của Zombie
    }

    void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);  // Di chuyển đến vị trí Player
        }
    }

    // Cập nhật tốc độ và máu của zombie tùy theo loại
    void SetZombieAttributes()
    {
        if (zombieType == ZombieType.Fast)
        {
            agent.speed = 8f;  // Tốc độ của zombie nhanh
            health = 1;  // Zombie loại này chỉ cần 1 hit là chết
        }
        else if (zombieType == ZombieType.Strong)
        {
            agent.speed = 3f;  // Tốc độ của zombie chậm
            health = 10;  // Zombie loại này cần 10 hit để chết
        }
    }

    // Hàm xử lý khi zombie bị bắn
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Tạo BoostItem tại vị trí Zombie chết
        if (boostItemPrefab != null)
        {
            Instantiate(boostItemPrefab, transform.position, Quaternion.identity);  // Spawn BoostItem tại vị trí Zombie
        }

        Destroy(gameObject);  // Hủy Zombie
    }
}
