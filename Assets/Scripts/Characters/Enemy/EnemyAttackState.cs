using UnityEngine;

namespace Characters.Enemy
{
    public class EnemyAttackState : EnemyState
    {
        private const float RotateSpeedMultiplier = 1.25f;
        
        private Transform enemyTransform;
        private Transform currentTarget;
        
        public override void OnEnter(EnemyController controller)
        {
            Controller = controller;
            enemyTransform = controller.transform;
            currentTarget = FindTarget(controller.Target);
            controller.CurrentSpeed = 0;
        }

        public override void OnUpdate(float deltaTime)
        {
            if (!IsTargetInRange())
            {
                Controller.OnDisengage();
                return;
            }
            
            RotateToTarget(deltaTime);
            Controller.WeaponController.SpawnProjectile(currentTarget);
        }

        public override void OnExit()
        {
        }

        private bool IsTargetInRange()
        {
            if (currentTarget == null)
                return false;

            return Vector3.Distance(currentTarget.position, enemyTransform.position) < Controller.FireRange;
        }
        
        private void RotateToTarget(float deltaTime)
        {
            if (currentTarget == null)
            {
                return;
            }
            
            // Calculate the direction to the target on the horizontal plane
            Vector3 directionToTarget = currentTarget.position - enemyTransform.position;
            directionToTarget.y = 0; // Lock movement direction to the horizontal plane
            directionToTarget.Normalize();

            // Smoothly rotate towards the target direction on the Y-axis only
            if (directionToTarget == Vector3.zero)
            {
                return;
            }
            
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            Quaternion newRotation = Quaternion.RotateTowards(
                enemyTransform.rotation,
                targetRotation,
                Controller.RotationSpeed * RotateSpeedMultiplier * deltaTime
            );

            // Lock rotation to Y-axis only
            enemyTransform.rotation = Quaternion.Euler(0, newRotation.eulerAngles.y, 0);
        }
        
        private Transform FindTarget(Target target)
        {
            switch (target)
            {
                //TODO: Put this through game manager
                case Target.Base:
                    return GameObject.FindGameObjectWithTag("Base").transform;
                case Target.Player:
                    return GameObject.FindGameObjectWithTag("Player").transform;
                case Target.Turret:
                    GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
                    if (turrets is not { Length: > 0 })
                    {
                        return null;
                    }

                    Transform closestTurret = null;
                    float closestTurretDistance = 0;
                    if (turrets.Length > 0)
                    {
                        foreach (GameObject turret in turrets)
                        {
                            if (closestTurret == null)
                            {
                                closestTurret = turret.transform;
                                closestTurretDistance = Vector3.Distance(enemyTransform.position, closestTurret.position);
                                continue;
                            }

                            float distance = Vector3.Distance(enemyTransform.position, turret.transform.position);
                            if (distance >= closestTurretDistance)
                            {
                                continue;
                            }

                            closestTurret = turret.transform;
                            closestTurretDistance = distance;
                        }
                    }

                    break;
                case Target.None:
                default:
                    Debug.LogError("No valid target set!");
                    break;
            }

            return null;
        }
    }
}