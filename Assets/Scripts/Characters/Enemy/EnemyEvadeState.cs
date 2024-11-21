using Movement;

namespace Characters.Enemy
{
    public class EnemyEvadeState : EnemyState
    {
        private float turnTimer = 0f;
        private float boostTimer = 0f;
        
        public override void OnEnter(EnemyController controller)
        {
            Controller = controller;
            Controller.GetTarget();
            Controller.MovementMode.SetMovementState(MovementState.Default);
        }

        public override void OnUpdate(float deltaTime)
        {
            turnTimer += deltaTime;
            if (turnTimer >= 2f)
            {
                if (Controller.MovementMode.CanBoost)
                {
                    Controller.MovementMode.StartBoosting();
                }
                
                boostTimer += deltaTime;
                return;
            }
            
            int direction = Controller.GetEvasionDirection();
            if (direction == 0)
            {
                Controller.OnDisengage();
                return;
            }

            Controller.MovementMode.SetMovementInput(new(direction, 0));
        }

        public override void OnExit()
        {
        }
    }
}