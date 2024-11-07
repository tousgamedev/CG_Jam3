using Player;
using UI;
using UnityEngine;

namespace Aircraft
{
    public class FlightController : MonoBehaviour
    {
        public float CurrentSpeed { get; private set; }
        
        [Header("Controls")] [SerializeField] private bool invertYAxis = true;

        [Header("Speed")] [SerializeField] private float baseForwardSpeed = 20f;
        [SerializeField] private float boostSpeedModifier = 20f;
        [SerializeField] private float boostTimeToMaxSpeed = .25f;
        [SerializeField] private float boostDuration = 3f;

        [Header("Turn")] [SerializeField] private float turnRateMax = 45f;
        [SerializeField] private float turnLerpSpeed = 2f;

        [Header("Pitch")]
        [SerializeField] private float pitchAngleMax = 45f;
        [SerializeField] private float pitchLerpSpeed = 1f;
        [SerializeField] private float altitudeFloor = 2f;
        [SerializeField] private float altitudeCeiling = 40f;
        
        [Header("Roll")] [SerializeField] private float rollAngleMax = 45f; // Max bank angle for tilt

        [Header("Visuals")]
        [SerializeField] private Transform aircraftParent;

        [Header("Crosshair")]
        [SerializeField] private Transform crosshairTargetNear;
        [SerializeField] private Transform crosshairTargetFar;
        
        private PlayerHub hub;
        private Vector3 currentPitch;
        private Vector3 moveDirection;
        private float currentVerticalSpeed;
        private float currentTurnRate;
        private float currentRoll;
        private float currentBoostDuration;
        private float targetBoostSpeed;
        private float boostRate;
        private bool hasEnoughBoost;
        private bool isBoosting;

        private void Start()
        {
            currentPitch = transform.forward;
            currentBoostDuration = boostDuration;
            CurrentSpeed = baseForwardSpeed;
            targetBoostSpeed = baseForwardSpeed + boostSpeedModifier;
            boostRate = boostSpeedModifier / boostTimeToMaxSpeed;
            
            PlayerHud.Instance.SetCrosshairTargetFar(crosshairTargetFar);
            PlayerHud.Instance.SetCrosshairTargetNear(crosshairTargetNear);
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            UpdateBoost(deltaTime);
            HandlePitch(deltaTime);
            HandleTurning(deltaTime);
            UpdateMovement(deltaTime);
            UpdateRotation();
        }

        private void UpdateBoost(float deltaTime)
        {
            if (!isBoosting && currentBoostDuration < boostDuration)
            {
                currentBoostDuration = Mathf.Min(currentBoostDuration + deltaTime, boostDuration);
            }

            if (Input.GetKey(KeyCode.Space) && ((isBoosting && currentBoostDuration > 0) || currentBoostDuration >= boostDuration))
            {
                isBoosting = true;
                currentBoostDuration -= deltaTime;
                CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, targetBoostSpeed, boostRate * deltaTime);
            }
            else
            {
                isBoosting = false;
                CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, baseForwardSpeed, boostRate * deltaTime);
            }
            
            PlayerHud.Instance.SetBoost(currentBoostDuration / boostDuration);
        }

        private void UpdateRotation()
        {
            // Combine pitch and roll rotations
            Quaternion pitchRotation = Quaternion.LookRotation(currentPitch);
            Quaternion rollRotation = Quaternion.Euler(0, 0, currentRoll);
            aircraftParent.rotation = pitchRotation * rollRotation;
        }

        private void UpdateMovement(float deltaTime)
        {
            moveDirection = (transform.forward * CurrentSpeed + transform.up * currentVerticalSpeed) * deltaTime;
            transform.Translate(moveDirection, Space.World);

            if (moveDirection.sqrMagnitude > 0.0001f)
            {
                currentPitch = moveDirection.normalized;
            }
            else if (Vector3.Dot(currentPitch, transform.forward) < 0.999f)
            {
                currentPitch = Vector3.Slerp(currentPitch, transform.forward, pitchLerpSpeed * deltaTime);
            }
        }

        private void HandleTurning(float deltaTime)
        {
            float horizontalInput = Mathf.Clamp(Input.GetAxis("Horizontal"), -1f, 1f);
            float targetTurnRate = horizontalInput * turnRateMax;

            // Gradually lerp towards the target turn rate for a slower response
            currentTurnRate = Mathf.Lerp(currentTurnRate, targetTurnRate, turnLerpSpeed * deltaTime);

            // Apply rotation based on the current interpolated turn rate
            float rotationAmount = currentTurnRate * deltaTime;
            transform.Rotate(0, rotationAmount, 0);

            // Tie roll angle directly to current turn rate
            currentRoll = currentTurnRate / turnRateMax * -rollAngleMax;
        }

        private void HandlePitch(float deltaTime)
        {
            float verticalInput = Input.GetAxis("Vertical") * (invertYAxis ? -1f : 1f);
            bool canClimb = verticalInput > 0 && transform.position.y < altitudeCeiling;
            bool canDescend = verticalInput < 0 && transform.position.y > altitudeFloor;

            // Determine target pitch angle based on input and altitude constraints
            var targetClimbAngle = 0f;
            if (canClimb)
            {
                targetClimbAngle = pitchAngleMax;
            }
            else if (canDescend)
            {
                targetClimbAngle = -pitchAngleMax;
            }

            // Calculate vertical speed using max forward speed and climb angle
            float targetVerticalSpeed = baseForwardSpeed * Mathf.Sin(targetClimbAngle * Mathf.Deg2Rad);
            currentVerticalSpeed = Mathf.Lerp(currentVerticalSpeed, targetVerticalSpeed, pitchLerpSpeed * deltaTime);
        }

        public void SetPlayerHub(PlayerHub playerHub)
        {
            hub = playerHub;
        }
    }
}