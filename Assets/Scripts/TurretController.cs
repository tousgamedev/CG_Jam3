using System;
using Characters;
using Managers;
using Turrets;
using UnityEngine;
using Weapons.Controllers;

namespace Movement
{
    public class TurretController : MonoBehaviour
    {
        public Action<EnemyController> OnTargetAcquire;
        public Action OnTargetClear;
        
        private const float FindTargetTimeInterval = 3f;
        private const float FireTimeInterval = 5f;

        private TurretWeaponController weaponController;
        private TurretRotator rotator;
        private EnemyController target;
        private float findTargetTimer;
        private float lastFireCheckTime;

        private void Awake()
        {
            if (!TryGetComponent(out weaponController))
            {
                Debug.LogWarning("No EnemyWeaponController component on TurretController found");
            }

            if (TryGetComponent(out rotator))
            {
                OnTargetAcquire += rotator.SetTarget;
                OnTargetClear += rotator.ClearTarget;
                rotator.SetProjectileSpeed(1000);
            }
        }

        public void Update()
        {
            FindTarget();
            if (target == null)
            {
                return;
            }

            weaponController.SpawnProjectile(target.transform);

            lastFireCheckTime += Time.deltaTime;
            if (weaponController.TimeSinceLastShot >= FireTimeInterval && lastFireCheckTime >= FireTimeInterval)
            {
                lastFireCheckTime = 0;
                target = null;
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

        private void FindTarget()
        {
            if (target != null)
            {
                return;
            }

            findTargetTimer += Time.deltaTime;
            if (findTargetTimer < FindTargetTimeInterval)
            {
                return;
            }

            findTargetTimer = 0;
            EnemyController enemy = EnemyManager.Instance.FindNearestTarget(transform);
            if (enemy == null)
            {
                return;
            }

            target = enemy;
            target.Health.OnHealthDepleted += ClearTarget;
            OnTargetAcquire?.Invoke(target);
        }

        private void ClearTarget(BaseCharacterController controller)
        {
            target = null;
            OnTargetClear?.Invoke();
        }
    }
}