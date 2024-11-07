using UnityEngine;

namespace Enemy
{
    public class EnemySpawnState : EnemyState
    {
        private const float DropSpeed = 200f;

        private Transform enemyTransform;
        private float dropAltitude;
        private float startAltitude;
        private float easingExponent = 1f;

        public override void OnEnter(EnemyController enemy)
        {
            Controller = enemy;
            enemyTransform = enemy.transform;
            startAltitude = enemyTransform.position.y;
            dropAltitude = Random.Range(10f, 35f);
        }

        public override void OnUpdate(float deltaTime)
        {
            if (enemyTransform.position.y > dropAltitude)
            {
                // Calculate the remaining distance as a fraction between 0 and 1
                float remainingDistance = (enemyTransform.position.y - dropAltitude) / (startAltitude - dropAltitude);

                // Apply an easing effect using the square of the remaining distance
                float speedMultiplier = Mathf.Max(Mathf.Pow(remainingDistance, easingExponent), .1f);

                // Move downward based on the calculated speed multiplier
                enemyTransform.Translate(Vector3.down * (deltaTime * DropSpeed * speedMultiplier));

                return;
            }

            Controller.OnFinishDropEntry();
        }

        public override void OnExit()
        {
        }
    }
}