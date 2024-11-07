using System;
using UnityEngine;

namespace Enemy
{
    public class EnemyTrackState : EnemyState
    {
        private Transform enemyTransform;
        private Transform currentTarget;
        
        public override void OnEnter(EnemyController controller)
        {
            Controller = controller;
            enemyTransform = controller.transform;
            currentTarget = FindTarget(controller.Target);
        }

        public override void OnUpdate(float deltaTime)
        {
            TrackTarget(deltaTime);
        }

        public override void OnExit()
        {
        }

        private void TrackTarget(float deltaTime)
        {
            // Calculate the direction to the target on the horizontal plane
            Vector3 directionToTarget = currentTarget.position - enemyTransform.position;
            directionToTarget.y = 0; // Lock movement direction to the horizontal plane
            directionToTarget.Normalize();

            // Smoothly rotate towards the target direction on the Y-axis only
            if (directionToTarget != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                Quaternion newRotation = Quaternion.RotateTowards(
                    enemyTransform.rotation, 
                    targetRotation, 
                    Controller.RotationSpeed * deltaTime
                );

                // Lock rotation to Y-axis only
                enemyTransform.rotation = Quaternion.Euler(0, newRotation.eulerAngles.y, 0);
            }

            // Move forward in the direction the enemy is currently facing
            Vector3 moveDirection = enemyTransform.forward * (Controller.MoveSpeed * deltaTime);
            enemyTransform.Translate(moveDirection, Space.World);
        }
        
        private Transform FindTarget(Target target)
        {
            switch(target)
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
            };

            return null;
        }
    }
}