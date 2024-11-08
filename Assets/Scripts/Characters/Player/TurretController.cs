using UnityEngine;

namespace Characters.Player
{
    public class TurretController : MonoBehaviour
    {
        [SerializeField] private Transform pitchPivot;
        [SerializeField] private float rotationSpeedY;
        [SerializeField] private float rotationSpeedX;


        private Transform currentTarget;

        private void RotateToTarget(float deltaTime, float rotateSpeed, Transform pivotTransform, bool lockToYAxis)
        {
            if (currentTarget == null)
            {
                return;
            }

            // Calculate the direction to the target on the horizontal plane
            Vector3 directionToTarget = currentTarget.position - pivotTransform.position;
            directionToTarget = lockToYAxis
                ? new(directionToTarget.x, 0, directionToTarget.z)
                : new Vector3(0, directionToTarget.y, directionToTarget.z);

            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            Quaternion newRotation = Quaternion.RotateTowards(
                pivotTransform.rotation,
                targetRotation,
                rotateSpeed * deltaTime
            );

            // Lock rotation to one axis only
            pivotTransform.rotation = lockToYAxis
                ? Quaternion.Euler(0, newRotation.eulerAngles.y, 0)
                : Quaternion.Euler(newRotation.eulerAngles.x, 0, 0);
        }
    }
}