using Aircraft;
using Movement;
using UnityEngine;

namespace Weapons.Controllers
{
    public class TurretWeaponController : BaseWeaponController
    {
        public float TimeSinceLastShot { get; private set; }

        private void Update()
        {
            if (Time.timeScale == 0)
            {
                return;
            }

            TimeSinceLastShot += Time.deltaTime;
        }

        public void SpawnProjectile(Transform target)
        {
            if (TimeSinceLastShot < FireInterval)
            {
                return;
            }

            if (!IsTargetWithinArc(target, 30f))
            {
                return;
            }

            Vector3 adjustedPosition = CalculateSpawnPosition();
            var laser = Instantiate(bulletPrefabs[PrefabIndex], adjustedPosition, bulletSpawn.rotation).GetComponent<Laser>();
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
            
            TimeSinceLastShot = 0;
            laser.AimAtTarget(futurePosition);
        }

        private bool TryGetFuturePosition(Transform target, EnemyController enemy, Laser laser, out Vector3 futurePosition)
        {
            float playerSpeed = enemy.MovementMode.Speed;
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

        protected override Vector3 CalculateSpawnPosition()
        {
            return bulletSpawn.position + bulletSpawn.forward;
        }
    }
}