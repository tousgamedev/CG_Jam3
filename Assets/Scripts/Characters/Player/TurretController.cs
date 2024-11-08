using Character;
using Characters.Enemy;
using Managers;
using UnityEngine;

namespace Characters.Player
{
    public enum RotationAxis
    {
        YAxis,
        XAxis
    }

    public class TurretController : MonoBehaviour
    {
        private const float TargetFindTimeInterval = 3f;
        private const float FireTimeInterval = 5f;

        [SerializeField] private Transform pitchPivot;
        [SerializeField] private Transform yawPivot;
        [SerializeField] private float rotationSpeedY;
        [SerializeField] private float rotationSpeedX;

        private TurretWeaponController weaponController;
        private Transform currentTarget;
        private float targetFindTimer;
        private float lastFireCheckTime;

        private void Awake()
        {
            if (!TryGetComponent(out weaponController))
            {
                Debug.LogWarning("No EnemyWeaponController component on TurretController found");
            }
        }

        public void Update()
        {
            FindTarget();
            if (currentTarget == null)
            {
                return;
            }

            RotateToTarget(Time.deltaTime, rotationSpeedY, yawPivot, RotationAxis.YAxis);
            RotateToTarget(Time.deltaTime, rotationSpeedX, pitchPivot, RotationAxis.XAxis);
            weaponController.SpawnProjectile(currentTarget);

            lastFireCheckTime += Time.deltaTime;
            if (weaponController.TimeSinceLastShot >= FireTimeInterval && lastFireCheckTime >= FireTimeInterval)
            {
                lastFireCheckTime = 0;
                currentTarget = null;
            }
        }

        public void UpgradeFirepower()
        {
            weaponController.UpgradeFirepower();
        }

        public void UpgradeRateOfFire()
        {
            weaponController.UpgradeRateOfFire();
        }

        private void RotateToTarget(float deltaTime, float rotateSpeed, Transform pivotTransform, RotationAxis axis)
        {
            if (currentTarget == null)
            {
                return;
            }

            Vector3 directionToTarget = currentTarget.position - pivotTransform.position;
            directionToTarget = axis == RotationAxis.YAxis
                ? new(directionToTarget.x, 0, directionToTarget.z)
                : new(0, directionToTarget.y, directionToTarget.z);

            if (directionToTarget == Vector3.zero)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            float targetAngle = axis == RotationAxis.YAxis
                ? targetRotation.eulerAngles.y
                : targetRotation.eulerAngles.x;

            float currentAngle = axis == RotationAxis.YAxis
                ? pivotTransform.localEulerAngles.y
                : pivotTransform.localEulerAngles.x;

            float smoothAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotateSpeed * deltaTime);

            pivotTransform.localRotation = axis == RotationAxis.YAxis
                ? Quaternion.Euler(0, smoothAngle, 0)
                : Quaternion.Euler(smoothAngle, 0, 0);
        }

        private void FindTarget()
        {
            if (currentTarget != null)
            {
                return;
            }

            targetFindTimer += Time.deltaTime;
            if (!(targetFindTimer >= TargetFindTimeInterval))
            {
                return;
            }

            targetFindTimer = 0;
            EnemyController target = EnemyManager.Instance.FindNearestTarget(transform);
            if (target == null)
            {
                return;
            }

            target.Health.OnHealthDepleted += ClearTarget;
            currentTarget = target.transform;
        }

        private void ClearTarget(ControllerBase controller)
        {
            currentTarget = null;
        }
    }
}