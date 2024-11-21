using Movement;
using UnityEngine;

namespace Characters.Enemy
{
    public class EnemyAttackState : EnemyState
    {
        public override void OnEnter(EnemyController controller)
        {
            Controller = controller;
            Controller.MovementMode.SetMovementState(MovementState.Stop);
        }

        public override void OnUpdate(float deltaTime)
        {
            if (!Controller.IsTargetInFiringRange())
            {
                Controller.OnDisengage();
                return;
            }

            Controller.RotateToTarget();
            if (!Controller.IsTargetWithinFiringArc())
            {
                Controller.OnDisengage();
                return;
            }

            if (Controller.IsTargetInEvasionRange())
            {
                Controller.OnEvade();
                return;
            }
            
            Controller.WeaponController.SpawnProjectile(Controller.CurrentTarget);
        }

        public override void OnExit()
        {
        }
    }
}