using System.Collections.Generic;
using Character;
using UI;
using UnityEngine;

namespace Characters.Player
{
    [System.Serializable]
    public struct EngineColor
    {
        public Color Main;
        public Color Highlight;
    }

    public class FlightController : ControllerBase
    {
        public float CurrentSpeed { get; private set; }

        [Header("Controls")] [SerializeField] private bool invertYAxis = true;

        [Header("Speed")] [SerializeField] private List<float> baseThrusts = new()
        {
            25f,
            30f,
            35f
        };
        [SerializeField] private float afterburnerSpeedModifier = 20f;
        [SerializeField] private float boostTimeToMaxSpeed = .25f;
        [SerializeField] private List<float> afterburnerDuration = new()
        {
            3f,
            6f,
            10f
        };

        [Header("Turn")] [SerializeField] private float turnRateMax = 45f;
        [SerializeField] private float turnLerpSpeed = 2f;

        [Header("Pitch")] [SerializeField] private float pitchAngleMax = 45f;
        [SerializeField] private float pitchLerpSpeed = 1f;
        [SerializeField] private float altitudeFloor = 2f;
        [SerializeField] private float altitudeCeiling = 40f;

        [Header("Roll")] [SerializeField] private float rollAngleMax = 45f; // Max bank angle for tilt

        [Header("Visuals")] [SerializeField] private Transform aircraftParent;
        [SerializeField] private ParticleSystem engineParticles;
        [SerializeField] private EngineColor engineColor;
        [SerializeField] private EngineColor boostColor;

        [Header("Crosshair")] [SerializeField] private Transform crosshairTargetNear;
        [SerializeField] private Transform crosshairTargetFar;

        private float TargetBoostSpeed => baseThrusts[afterburnerIndex] + afterburnerSpeedModifier;
        
        private PlayerHub hub;
        private Vector3 currentPitch;
        private Vector3 moveDirection;
        private float currentVerticalSpeed;
        private float currentTurnRate;
        private float currentRoll;
        private float currentBoostDuration;
        private float boostRate;
        private bool hasEnoughBoost;
        private bool isBoosting;
        private float boostLerp;

        private int thrustIndex;
        private int afterburnerIndex;
        
        private void Start()
        {
            currentPitch = transform.forward;
            currentBoostDuration = afterburnerDuration[afterburnerIndex];
            CurrentSpeed = baseThrusts[thrustIndex];
            boostRate = afterburnerSpeedModifier / boostTimeToMaxSpeed;

            PlayerCanvas.Instance.SetCrosshairTargetFar(crosshairTargetFar);
            PlayerCanvas.Instance.SetCrosshairTargetNear(crosshairTargetNear);
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
            if (!isBoosting && currentBoostDuration < afterburnerDuration[afterburnerIndex])
            {
                currentBoostDuration = Mathf.Min(currentBoostDuration + deltaTime, afterburnerDuration[afterburnerIndex]);
            }

            if (Input.GetKey(KeyCode.Space) && ((isBoosting && currentBoostDuration > 0) || currentBoostDuration >= afterburnerDuration[afterburnerIndex]))
            {
                isBoosting = true;
                currentBoostDuration -= deltaTime;

                CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, TargetBoostSpeed, boostRate * deltaTime);
            }
            else
            {
                isBoosting = false;
                CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, baseThrusts[thrustIndex], boostRate * deltaTime);
            }

            float boostProgress = Mathf.InverseLerp(baseThrusts[thrustIndex], TargetBoostSpeed, CurrentSpeed);
            SetThrusterColor(boostProgress);

            PlayerCanvas.Instance.SetBoost(currentBoostDuration / afterburnerDuration[afterburnerIndex]);
        }

        private void SetThrusterColor(float boostProgress)
        {
            Color currentMainColor = Color.Lerp(engineColor.Main, boostColor.Main, boostProgress);
            Color currentHighlightColor = Color.Lerp(engineColor.Highlight, boostColor.Highlight, boostProgress);
            ParticleSystem.MainModule mainModule = engineParticles.main;
            mainModule.startColor = new(currentMainColor, currentHighlightColor);
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
            float targetVerticalSpeed = baseThrusts[thrustIndex] * Mathf.Sin(targetClimbAngle * Mathf.Deg2Rad);
            currentVerticalSpeed = Mathf.Lerp(currentVerticalSpeed, targetVerticalSpeed, pitchLerpSpeed * deltaTime);
        }

        public void SetPlayerHub(PlayerHub playerHub)
        {
            hub = playerHub;
        }

        public void UpgradeThrust()
        {
            if (thrustIndex < baseThrusts.Count - 1)
            {
                thrustIndex++;
            }
        }

        public void UpgradeAfterburner()
        {
            if (afterburnerIndex < afterburnerDuration.Count - 1)
            {
                afterburnerIndex++;
            }
        }
    }
}