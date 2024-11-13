using Movement;
using UnityEngine;

namespace Characters.Enemy
{
    public class EnemyIdleState : EnemyState
    {
        public override void OnEnter(EnemyController controller)
        {
            Controller = controller;
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnExit()
        {
        }
    }
}