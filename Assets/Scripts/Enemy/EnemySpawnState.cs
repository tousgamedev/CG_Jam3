using UnityEngine;

namespace Enemy
{
    public class EnemySpawnState : EnemyState
    {
        private const float DropSpeed = 100f;
    
        private Transform enemyTransform;
        private float dropAltitude;

        public override void OnEnter(EnemyController enemy)
        {
            Controller = enemy;
            enemyTransform = enemy.transform;
            dropAltitude = Random.Range(10f, 35f);
        }

        public override void OnUpdate(float deltaTime)
        {
            if (enemyTransform.position.y > dropAltitude)
            {
                enemyTransform.Translate(Vector3.down * deltaTime * DropSpeed);
                return;
            }

            Controller.OnFinishDropEntry();
        }

        public override void OnExit()
        {
        }
    }
}