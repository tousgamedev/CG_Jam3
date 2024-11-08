namespace Characters.Enemy
{
    public class EnemyDieState : EnemyState
    {
        public override void OnEnter(EnemyController controller)
        {
            Controller = controller;
            Controller.PlayDeathEffects();
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnExit()
        {
        }
    }
}