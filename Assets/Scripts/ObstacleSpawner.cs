
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefab")]
    [SerializeField] private GameObject obstaclePrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;
   
    [Header("Obstacle Count Chances (%)")]
    [Range(0f, 100f)]
    [SerializeField] private float oneObstacleChance = 30f;

    [Range(0f, 100f)]
    [SerializeField] private float twoObstacleChance = 50f;

    [Range(0f, 100f)]
    [SerializeField] private float threeObstacleChance = 20f;

  
    [SerializeField] private float moveSpeed = 0.15f;
    [SerializeField] private float minimumScale = 0.3f;
    [SerializeField] private float maximumScale = 2f;

    [Header("Player References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerHP playerHP;

    private float spawnTimer;

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnObstacles();
            spawnTimer -= spawnInterval;
        }
    }

    private void SpawnObstacles()
    {
        if (obstaclePrefab == null)
            return;

        float totalChance =
            oneObstacleChance +
            twoObstacleChance +
            threeObstacleChance;

        if (totalChance <= 0f)
            return;

        float randomValue = Random.Range(0f, totalChance);

        int obstacleCount;

        if (randomValue < oneObstacleChance)
        {
            obstacleCount = 1;
        }
        else if (randomValue < oneObstacleChance + twoObstacleChance)
        {
            obstacleCount = 2;
        }
        else
        {
            obstacleCount = 3;
        }

        // Randomize the three lanes.
        int[] lanes = { 0, 1, 2 };

        for (int i = 0; i < lanes.Length; i++)
        {
            int randomIndex = Random.Range(i, lanes.Length);

            int temp = lanes[i];
            lanes[i] = lanes[randomIndex];
            lanes[randomIndex] = temp;
        }

        // Spawn in different lanes.
        for (int i = 0; i < obstacleCount; i++)
        {
            SpawnObstacle(lanes[i]);
        }
    }

    private void SpawnObstacle(int lane)
    {
        GameObject obstacleObject = Instantiate(obstaclePrefab);

        Obstacle obstacle = obstacleObject.GetComponent<Obstacle>();

        if (obstacle == null)
        {
            Debug.LogWarning("Obstacle prefab is missing Obstacle.cs!");
            Destroy(obstacleObject);
            return;
        }

       
        obstacle.Initialize(
            lane,
            moveSpeed,
            minimumScale,
            maximumScale,
            playerMovement,
            playerHP
        );
        
    }
}
