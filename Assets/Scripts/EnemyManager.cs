using System;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

namespace Managers
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance;

        private const float Minute = 60f;

        // TODO: This is fucking jank. Should be a single container
        [Header("Enemies")]
        [SerializeField] private List<GameObject> enemyPrefabs = new();
        [SerializeField] private List<Vector2> spawnRates = new();
        
        [Header("Spawn Behavior")]
        [SerializeField] private AnimationCurve spawnCurve;
        [SerializeField] private float minutesToMaxSpawnRate = 20f;
        [SerializeField] private float maxSpawnsPerMinute = 60f;
        [SerializeField] private int maxEnemies = 100;
        
        private List<EnemyController> enemies = new();
        private float timer;
        
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                if (spawnRates.Count != enemyPrefabs.Count)
                {
                    Debug.LogError("Enemy spawn rate and prefab list need to be same size");
                }
                
                return;
            }
            
            Destroy(gameObject);
        }


        public EnemyController FindNearestTarget(Transform seeker)
        {
            return null;
        }
    }
}