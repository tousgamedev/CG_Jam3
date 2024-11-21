using Movement;
using UnityEngine;

namespace Turrets
{
    public class TurretRotator : MonoBehaviour
    {
        [SerializeField]
        private Transform basePivot; // Pivot for Y-axis rotation

        [SerializeField]
        private Transform gunPivot; // Pivot for X-axis rotation
        
        [SerializeField]
        private float baseRotationSpeed;

        [SerializeField]
        private float gunRotationSpeed;
        
        [SerializeField]
        private float gunMinAngle = -10f; // Minimum X-axis angle for the gun

        [SerializeField]
        private float gunMaxAngle = 80f;
        
        private EnemyController target;
        private Transform targetTransform;
        private float projectileSpeed;
        
        private void Update()
        {
            if (target == null)
            {
                return;
            }

            Vector3 predictedTargetPosition = PredictTargetPosition();
            Vector3 directionToTarget = basePivot.DirectionTo(predictedTargetPosition);
            
            Vector3 baseDirection = new (directionToTarget.x, 0f, directionToTarget.z);
            if (baseDirection.sqrMagnitude > 0.01f)
            {
                Quaternion baseRotation = Quaternion.LookRotation(baseDirection);
                basePivot.rotation = Quaternion.Euler(0f, baseRotation.eulerAngles.y, 0f);
            }
            
            Vector3 gunDirection = basePivot.InverseTransformDirection(directionToTarget);
            float gunAngle = Mathf.Atan2(gunDirection.y, gunDirection.z) * Mathf.Rad2Deg;

            // Clamp the gun's rotation
            gunAngle = Mathf.Clamp(gunAngle, gunMinAngle, gunMaxAngle);
            gunPivot.localRotation = Quaternion.Euler(gunAngle, 0f, 0f);
        }

        private Vector3 PredictTargetPosition()
        {
            // Calculate time to hit based on distance and projectile speed
            Vector3 directionToTarget = basePivot.DirectionTo(targetTransform);
            float distanceToTarget = directionToTarget.magnitude;
            float timeToHit = distanceToTarget / projectileSpeed;

            // Predict future position
            Vector3 targetVelocity = target.MovementMode.Velocity;
            return targetTransform.position + targetVelocity * timeToHit;
        }

        public void SetTarget(EnemyController newTarget)
        {
            target = newTarget;
            targetTransform = target.transform;
        }

        public void ClearTarget()
        {
            target = null;
            targetTransform = null;
        }

        public void SetProjectileSpeed(float speed)
        {
            projectileSpeed = speed;
        }
    }
}