using Movement;

namespace Characters.Enemy
{
    public abstract class EnemyState
    {
        protected EnemyController Controller;
        
        public abstract void OnEnter(EnemyController controller);
        public abstract void OnUpdate(float deltaTime);
        public abstract void OnExit();
    }
}