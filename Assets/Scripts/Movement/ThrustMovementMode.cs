using UnityEngine;
using UnityEngine.Serialization;
using VFX;

namespace Movement
{
    public class ThrustMovementMode : BaseMovementMode
    {
        // Define floor and ceiling altitude constraints
        private const float FloorAltitude = 5f;
        private const float CeilingAltitude = 40f;
        
        [Header("Visuals"), SerializeField]
        private Transform modelParent;

        [SerializeField]
        private EngineEffects engineEffects;
        
        public override void Awake()
        {
            base.Awake();
            UpdateEngineParticles();
        }

        public override void Update()
        {
            base.Update();
            UpdateEngineParticles();
        }

        protected override void UpdateSpeed(float deltaTime)
        {
            switch (MovementState)
            {
                case MovementState.Restoring:
                    CurrentForwardSpeed = Mathf.MoveTowards(CurrentForwardSpeed, TargetBaseSpeed, BaseReturnRate * deltaTime);

                    if (CurrentBoostDuration < TargetBoostDuration)
                    {
                        OnBoostDurationChange?.Invoke(CurrentBoostDuration / TargetBoostDuration);
                    }
                    else if (CurrentBrakeDuration < TargetBrakeDuration)
                    {
                        // Currently does nothing
                    }
                    else
                    {
                        MovementState = MovementState.Default;
                        OnBoostDurationChange?.Invoke(1f);
                    }

                    break;
                case MovementState.Boost:
                    CurrentBoostDuration -= deltaTime;
                    CurrentForwardSpeed = Mathf.MoveTowards(CurrentForwardSpeed, TargetBoostSpeed, BoostRate * deltaTime);
                    if (CurrentBoostDuration <= 0)
                    {
                        MovementState = MovementState.Restoring;
                    }

                    OnBoostDurationChange?.Invoke(CurrentBoostDuration / boostDurations[BoostDurationIndex]);
                    
                    break;
                case MovementState.Brake:
                    CurrentBrakeDuration -= deltaTime;
                    CurrentForwardSpeed = Mathf.MoveTowards(CurrentForwardSpeed, TargetBrakeSpeed, BrakeRate * deltaTime);
                    if (CurrentBrakeDuration <= 0)
                    {
                        MovementState = MovementState.Restoring;
                    }

                    break;
                case MovementState.Default:
                    break;
            }
        }

        protected override void UpdateAltitude(float deltaTime)
        {
            float targetPitchAngle = GetAdjustedPitchAngle();

            // Calculate vertical speed based on climb angle and max forward speed
            float targetVerticalSpeed = TargetBaseSpeed * Mathf.Sin(targetPitchAngle * Mathf.Deg2Rad);
            CurrentVerticalSpeed = Mathf.Lerp(CurrentVerticalSpeed, targetVerticalSpeed, pitchLerpSpeed * deltaTime);
        }

        private float GetAdjustedPitchAngle()
        {
            const float boundaryBuffer = 5f; // Range within which to begin leveling out
            float currentAltitude = transform.position.y;

            // Check if the object is close to the ceiling and needs to level out
            if (MovementInput.y > 0 && currentAltitude >= CeilingAltitude - boundaryBuffer)
            {
                float distanceToCeiling = CeilingAltitude - currentAltitude;
                float ceilingFactor = Mathf.Clamp01(distanceToCeiling / boundaryBuffer);
                return pitchAngles[PitchAngleIndex] * ceilingFactor;
            }

            // Check if the object is close to the floor and needs to level out
            if (MovementInput.y < 0 && currentAltitude <= FloorAltitude + boundaryBuffer)
            {
                float distanceToFloor = currentAltitude - FloorAltitude;
                float floorFactor = Mathf.Clamp01(distanceToFloor / boundaryBuffer);
                return -pitchAngles[PitchAngleIndex] * floorFactor;
            }

            // Use the full pitch angle if not near the boundary
            return MovementInput.y > 0 ? pitchAngles[PitchAngleIndex] : MovementInput.y < 0 ? -pitchAngles[PitchAngleIndex] : 0f;
        }

        protected override void UpdateYawAndRoll(float deltaTime)
        {
            float targetYaw = MovementInput.x * turnAngles[TurnAngleIndex].yaw;

            // Gradually lerp towards the target turn rate for a slower response
            CurrentYaw = Mathf.Lerp(CurrentYaw, targetYaw, turnLerpSpeed * deltaTime);

            // Apply rotation based on the current interpolated turn rate
            float rotationAmount = CurrentYaw * deltaTime;
            transform.Rotate(0, rotationAmount, 0);

            // Tie roll angle directly to current turn rate
            CurrentRoll = CurrentYaw / turnAngles[TurnAngleIndex].yaw * -turnAngles[TurnAngleIndex].roll;

            Quaternion pitchRotation = Quaternion.LookRotation(CurrentPitch);
            Quaternion rollRotation = Quaternion.Euler(0, 0, CurrentRoll);
            modelParent.rotation = pitchRotation * rollRotation;
        }

        protected override void Move(float deltaTime)
        {
            CurrentHeading = transform.forward * CurrentForwardSpeed + transform.up * CurrentVerticalSpeed;
            transform.Translate(CurrentHeading * deltaTime, Space.World);

            if (CurrentHeading.sqrMagnitude > 0.0001f)
            {
                CurrentPitch = CurrentHeading.normalized;
            }
            else if (Vector3.Dot(CurrentPitch, transform.forward) < 0.999f)
            {
                CurrentPitch = Vector3.Slerp(CurrentPitch, transform.forward, pitchLerpSpeed * deltaTime);
            }
        }

        private void UpdateEngineParticles()
        {
            if (MovementState == MovementState.Default)
            {
                return;
            }
            
            float progress = CalculateProgress(out EngineEffectsConfig effectsConfig);
            engineEffects.ApplyEffects(effectsConfig, progress);
        }

        private float CalculateProgress(out EngineEffectsConfig engineEffectsConfig)
        {
            engineEffectsConfig = MovementState switch
            {
                MovementState.Boost => engineEffects.BoostConfig,
                MovementState.Brake => engineEffects.BrakeConfig,
                MovementState.Restoring when CurrentSpeed < TargetBaseSpeed => engineEffects.BrakeConfig,
                MovementState.Restoring => engineEffects.BoostConfig,
                _ => engineEffects.BaseConfig
            };
            
            return MovementState switch
            {
                MovementState.Boost => SetProgress(TargetBaseSpeed, TargetBoostSpeed),
                MovementState.Brake => SetProgress(TargetBaseSpeed, TargetBrakeSpeed),
                MovementState.Restoring => SetProgress(TargetBaseSpeed, CurrentSpeed < TargetBaseSpeed ? TargetBrakeSpeed : TargetBoostSpeed),
                _ => 0f
            };
        }
        
        private float SetProgress(float minSpeed, float maxSpeed)
        {
            return Mathf.InverseLerp(minSpeed, maxSpeed, CurrentSpeed);
        }
    }
}