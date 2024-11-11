using System.Collections.Generic;
using Character;
using Characters.Enemy;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    [System.Serializable]
    public class EnemyData
    {
        public int SpawnChance;
        public GameObject Prefab;
    }

    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance { get; private set; }

        private const float Minute = 60f;

        [Header("Enemy Data")] [SerializeField]
        private List<EnemyData> enemyData = new();

        [Header("Spawn Behavior")] [SerializeField]
        private List<Transform> spawnPoints = new();

        [SerializeField] private AnimationCurve spawnRateCurve;

        [SerializeField] private float minutesToMaxSpawnRate = 20f;
        [SerializeField] private float maxSpawnsPerMinute = 60f;
        [SerializeField] private int maxEnemies = 100;

        private readonly List<EnemyController> enemies = new();
        private float spawnThreshold;
        private float spawnTimer;
        private float rateIncreaseTimer;
        private int numberToSpawn;
        private int spawnedThisMinute;
        private int currentMinute;
        private int totalChance;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            spawnTimer = spawnThreshold;
            UpdateSpawnPoints();
            UpdateSpawnTable();
            CalculateSpawnParameters();
        }

        private void Update()
        {
            if (GameManager.Instance.GameOver)
            {
                return;
            }
            
            UpdateSpawnRate();
            SpawnEnemy();
        }

        private void UpdateSpawnRate()
        {
            rateIncreaseTimer += Time.deltaTime;
            if (rateIncreaseTimer < Minute || currentMinute > minutesToMaxSpawnRate)
            {
                return;
            }

            rateIncreaseTimer = 0;
            currentMinute++;
            CalculateSpawnParameters();
        }

        private void SpawnEnemy()
        {
            spawnTimer += Time.deltaTime;
            if (enemies.Count >= maxEnemies || spawnTimer < spawnThreshold)
            {
                return;
            }

            spawnTimer = 0;
            GameObject prefab = SelectRandomEnemy();
            if (prefab == null)
            {
                Debug.LogError("No prefab selected for spawning; check spawn rates and configurations.");
                return;
            }

            Transform spawnPoint = SelectRandomSpawnPoint();
            var enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity).GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemies.Add(enemy);
                enemy.Health.SetController(enemy);
                enemy.Health.OnHealthDepleted += RemoveEnemy;
            }
            else
            {
                Debug.LogError("Bad spawn");
            }
        }

        public EnemyController FindNearestTarget(Transform seeker)
        {
            EnemyController closestEnemy = null;
            float closestDistance = Mathf.Infinity;

            foreach (EnemyController enemy in enemies)
            {
                float distance = Vector3.Distance(seeker.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestEnemy = enemy;
                    closestDistance = distance;
                }
            }

            return closestEnemy;
        }

        private void UpdateSpawnPoints()
        {
            if (spawnPoints.Count == 0)
            {
                Debug.LogWarning("No spawn points available");
                return;
            }

            foreach (var point in spawnPoints)
            {
                point.SetPositionAndRotation(new(point.position.x, 1000, point.position.z), point.rotation);
            }
        }

        private void UpdateSpawnTable()
        {
            if (enemyData.Count == 0)
            {
                Debug.LogError("No enemy data provided.");
                return;
            }

            int currentChance = 0;
            foreach (EnemyData data in enemyData)
            {
                currentChance += data.SpawnChance;
                data.SpawnChance = currentChance;
            }

            totalChance = currentChance;
            if (totalChance == 0)
            {
                Debug.LogError("Total chance is zero; check spawn rates.");
            }
        }

        private GameObject SelectRandomEnemy()
        {
            int random = Random.Range(0, totalChance);
            foreach (EnemyData data in enemyData)
            {
                if (random <= data.SpawnChance)
                {
                    return data.Prefab;
                }
            }

            return null;
        }

        private Transform SelectRandomSpawnPoint()
        {
            int random = Random.Range(0, spawnPoints.Count);
            return spawnPoints[random];
        }

        private void RemoveEnemy(ControllerBase controllerBase)
        {
            if (controllerBase is EnemyController enemy)
            {
                enemies.Remove(enemy);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void CalculateSpawnParameters()
        {
            numberToSpawn = (int)(spawnRateCurve.Evaluate(currentMinute) * maxSpawnsPerMinute);
            spawnThreshold = Minute / numberToSpawn;
        }
    }
}