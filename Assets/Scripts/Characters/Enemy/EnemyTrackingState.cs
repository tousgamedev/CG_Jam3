using Movement;
using UnityEngine;

namespace Characters.Enemy
{
    public class EnemyTrackingState : EnemyState
    {
        private float extendedTurnTimer;
        
        public override void OnEnter(EnemyController controller)
        {
            Controller = controller;
            Controller.GetTarget();
            Controller.MovementMode.SetMovementState(MovementState.Default);
        }

        public override void OnUpdate(float deltaTime)
        {
            Debug.Log(extendedTurnTimer);
            if (!Controller.IsTargetInEngagementRange())
            {
                extendedTurnTimer = 0;
                Controller.RotateToTarget();
                return;
            }
            
            if(Controller.IsTargetWithinFiringArc())
            {
                extendedTurnTimer = 0;
                Controller.OnEngage();
                return;
            }

            if (Controller.IsTargetWithinTurnArc(1))
            {
                extendedTurnTimer = 0;
                Controller.RotateToTarget();
                return;
            }

            if (Controller.IsTargetWithinTurnArc(2) && extendedTurnTimer < .5f)
            {
                Controller.RotateToTarget();
                extendedTurnTimer += deltaTime;
            }
        }

        public override void OnExit()
        {
        }
    }
}