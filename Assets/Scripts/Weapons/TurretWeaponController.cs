using System.Collections.Generic;
using Aircraft;
using Characters.Enemy;
using Movement;
using UnityEngine;
using Weapons;

namespace Characters.Player
{
    public class TurretWeaponController : MonoBehaviour
    {
        public float TimeSinceLastShot => shotTimer;
        
        [SerializeField, Tooltip("Shots per minute")]
        private List<float> ratesOfFire = new()
        {
            15,
            30,
            45
        };

        [SerializeField] private List<GameObject> bulletPrefabs;
        [SerializeField] private Transform bulletSpawn;

        private TurretController hub;
        private float timeBetweenShots;
        private float shotTimer;
        private float spawnAdjust;

        private int prefabIndex;
        private int rateOfFireIndex;

        private void Start()
        {
            rateOfFireIndex = Mathf.Clamp(rateOfFireIndex, 0, ratesOfFire.Count - 1);
            prefabIndex = Mathf.Clamp(prefabIndex, 0, bulletPrefabs.Count - 1);
            timeBetweenShots = 60f / ratesOfFire[rateOfFireIndex];
            shotTimer = timeBetweenShots;
        }

        private void Update()
        {
            if (Time.timeScale == 0)
            {
                return;
            }

            shotTimer += Time.deltaTime;
        }

        public void SpawnProjectile(Transform target)
        {
            if (shotTimer < timeBetweenShots)
            {
                return;
            }

            if (!IsTargetWithinArc(target, 30f))
            {
                return;
            }

            Vector3 adjustedPosition = CalculateSpawnPosition();
            var laser = Instantiate(bulletPrefabs[prefabIndex], adjustedPosition, bulletSpawn.rotation).GetComponent<Laser>();
            if (laser == null)
            {
                return;
            }
            
            laser.SetFaction(Faction.Ally);
            if (!target.gameObject.TryGetComponent(out EnemyController enemy))
            {
                laser.AimAtTarget(target.position);
                return;
            }

            if (!TryGetFuturePosition(target, enemy, laser, out Vector3 futurePosition))
            {
                return;
            }
            
            shotTimer = 0;
            laser.AimAtTarget(futurePosition);
        }

        private bool TryGetFuturePosition(Transform target, EnemyController enemy, Laser laser, out Vector3 futurePosition)
        {
            float playerSpeed = enemy.CurrentSpeed;
            Vector3 playerVelocity = target.forward * playerSpeed;

            float distance = Vector3.Distance(target.position, transform.position);
            Vector3 toTarget = target.position - transform.position;

            float a = laser.Speed * laser.Speed - playerSpeed * playerSpeed;
            float b = 2 * Vector3.Dot(toTarget, playerVelocity);
            float c = -distance * distance;
            float discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
            {
                futurePosition = Vector3.zero;
                Debug.Log("Target is out of reach or too fast to intercept.");
                return false;
            }

            float timeToIntercept = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
            futurePosition = target.position + playerVelocity * timeToIntercept;
            return true;
        }

        public bool IsTargetWithinArc(Transform targetTransform, float arcAngle = 60f)
        {
            Vector3 directionToTarget = (targetTransform.position - bulletSpawn.position).normalized;
            float cosAngleThreshold = Mathf.Cos(arcAngle / 2 * Mathf.Deg2Rad);
            return Vector3.Dot(bulletSpawn.forward, directionToTarget) >= cosAngleThreshold;
        }

        public void UpgradeFirepower()
        {
            prefabIndex = Mathf.Clamp(prefabIndex + 1, 0, bulletPrefabs.Count - 1);
        }

        public void UpgradeRateOfFire()
        {
            rateOfFireIndex = Mathf.Clamp(rateOfFireIndex + 1, 0, ratesOfFire.Count - 1);
        }

        private Vector3 CalculateSpawnPosition()
        {
            return bulletSpawn.position + bulletSpawn.forward;
        }
    }
}