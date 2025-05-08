using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    public GameObject playerPrefab;  // prefab của Player
    public GameObject zombiePrefab1;  // prefab của Zombie
    public GameObject zombiePrefab2;  // prefab của Zombie
    public Transform spawnPlayerPoint;  // Vị trí spawn cho player
    public Transform[] spawnZombiePoints;  // Vị trí spawn cho Zombie (có thể là nhiều điểm)

    void Start()
    {
        SpawnPlayer();
        SpawnZombies();
    }

    void SpawnPlayer()
    {
        // Spawn player ở vị trí spawnPlayerPoint
        Instantiate(playerPrefab, spawnPlayerPoint.position, spawnPlayerPoint.rotation);
    }

    void SpawnZombies()
    {
        foreach (var spawnPoint in spawnZombiePoints)
        {
            // Tìm vị trí hợp lệ trên NavMesh để spawn zombie
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPoint.position, out hit, 10f, NavMesh.AllAreas))
            {
                // Spawn Zombie 1 tại vị trí hợp lệ trên NavMesh
                GameObject zombie1 = Instantiate(zombiePrefab1, hit.position, spawnPoint.rotation);
                NavMeshAgent agent1 = zombie1.GetComponent<NavMeshAgent>();
                if (agent1 != null) agent1.enabled = true;

                // Spawn Zombie 2 tại vị trí hợp lệ trên NavMesh
                GameObject zombie2 = Instantiate(zombiePrefab2, hit.position, spawnPoint.rotation);
                NavMeshAgent agent2 = zombie2.GetComponent<NavMeshAgent>();
                if (agent2 != null) agent2.enabled = true;
            }
            else
            {
                Debug.LogWarning("Zombie không thể spawn trên NavMesh tại vị trí " + spawnPoint.position);
            }
        }
    }
}
