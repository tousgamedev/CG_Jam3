using Movement;
using Player;
using UnityEngine;

namespace Weapons.Controllers
{
    public class EnemyWeaponController : BaseWeaponController
    {
        private EnemyController hub;

        private void Update()
        {
            ShotTimer += Time.deltaTime;
        }

        public void SpawnProjectile(Transform target)
        {
            if (ShotTimer < FireInterval)
            {
                return;
            }

            if (!IsTargetWithinArc(target))
            {
                return;
            }

            Vector3 adjustedPosition = CalculateSpawnPosition();
            ShotTimer = 0;
            var laser = Instantiate(bulletPrefabs[PrefabIndex], adjustedPosition, bulletSpawn.rotation).GetComponent<Laser>();
            if (laser == null)
            {
                return;
            }
            
            laser.SetFaction(hub.Health.Faction);
            if (!target.gameObject.TryGetComponent(out PlayerController player))
            {
                laser.AimAtTarget(target.position);
                return;
            }

            if (CalculateMovingTargetPosition(target, player, laser, out Vector3 futurePosition))
            {
                return;
            }

            int doRandom = Random.Range(0, 5);
            // accurately aim 20% of shots
            if (doRandom > 0)
            {
                futurePosition = AddRandomOffset(futurePosition);
            }

            laser.AimAtTarget(futurePosition);
        }

        private bool CalculateMovingTargetPosition(Transform target, PlayerController player, Laser laser, out Vector3 futurePosition)
        {
            float playerSpeed = player.MovementMode.Speed;
            Vector3 playerDirection = target.forward; // Assuming player is moving in its forward direction
            Vector3 playerVelocity = playerDirection * playerSpeed;

            // Calculate the distance between the enemy and the player
            float distance = Vector3.Distance(target.position, transform.position);
            Vector3 toTarget = target.position - transform.position;

            // Calculate the time to intercept
            float a = laser.Speed * laser.Speed - playerSpeed * playerSpeed;
            float b = 2 * Vector3.Dot(toTarget, playerVelocity);
            float c = -distance * distance;

            // Solve the quadratic equation for time to intercept
            float discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
            {
                // No real solution, target is out of reach or too fast
                Debug.Log("Target is out of reach or too fast to intercept.");
                futurePosition = target.position;
                return true;
            }

            // Calculate the positive time to intercept
            float timeToIntercept = (-b + Mathf.Sqrt(discriminant)) / (2 * a);

            // Calculate the future position of the player
            futurePosition = target.position + playerVelocity * timeToIntercept;
            return false;
        }
        
        public Vector3 AddRandomOffset(Vector3 position)
        {
            // Generate a random deviation within a sphere of radius maxDeviation
            Vector3 deviation = Random.insideUnitSphere * 4;
            return position + deviation;
        }

        public bool IsTargetWithinArc(Transform targetTransform, float arcAngle = 60f)
        {
            Vector3 directionToTarget = (targetTransform.position - bulletSpawn.position).normalized;
            float cosAngleThreshold = Mathf.Cos(arcAngle / 2 * Mathf.Deg2Rad);
            return Vector3.Dot(bulletSpawn.forward, directionToTarget) >= cosAngleThreshold;
        }
        
        public void SetEnemyController(EnemyController controller)
        {
            hub = controller;
        }

        protected override Vector3 CalculateSpawnPosition()
        {
            return bulletSpawn.position + bulletSpawn.forward;
        }
    }
}