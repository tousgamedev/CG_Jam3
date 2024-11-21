using System;
using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
    [Serializable]
    public struct TurnAngle
    {
        public float yaw;
        public float roll;
    }

    public enum MovementAttribute
    {
        BaseSpeed,
        BoostSpeed,
        BoostDuration,
        BoostRestoreTime,
        BrakeSpeed,
        BrakeDuration,
        BrakeRestoreTime,
        PitchAngle,
        TurnAngle
    }

    public enum MovementState
    {
        Default,
        Boost,
        Brake,
        Restoring,
        Stop
    }

    public abstract class BaseMovementMode : MonoBehaviour
    {
        protected const float FloorAltitude = 10f;
        protected const float CeilingAltitude = 40f;
        
        public Action<float> OnBoostDurationChange;
        
        public float Speed => ForwardSpeed;
        public bool CanBoost => MovementState == MovementState.Default && CurrentBoostDuration == TargetBoostDuration;
        public Vector3 Velocity => CurrentVelocity;
        public TurnAngle TurnAngles => turnAngles[TurnAngleIndex];
        
        protected float TargetBoostSpeed => baseSpeeds[BaseSpeedIndex] + boostSpeeds[BoostSpeedIndex];
        protected float TargetBoostDuration => boostDurations[BoostDurationIndex];
        protected float TargetBrakeSpeed => brakeSpeeds[BrakeSpeedIndex];
        protected float TargetBrakeDuration => brakeDurations[BrakeDurationIndex];
        protected float TargetBaseSpeed => baseSpeeds[BaseSpeedIndex];

        [Header("Speed"), SerializeField]
        protected float[] baseSpeeds;

        [SerializeField]
        protected float[] boostSpeeds;

        [SerializeField]
        protected float[] boostDurations;

        [SerializeField]
        protected float[] boostRestoreTimes;

        [SerializeField]
        protected float startSpeedChangeTransitionTime = 0.25f;

        [SerializeField]
        protected float endSpeedChangeTransitionTime = 1f;

        [SerializeField]
        protected float[] brakeSpeeds;

        [SerializeField]
        protected float[] brakeDurations;

        [SerializeField]
        protected float[] brakeRestoreTimes;

        [Header("Pitch, Yaw, Roll"), SerializeField]
        protected float[] pitchAngles;

        [SerializeField]
        protected float pitchLerpSpeed = 1f;

        [SerializeField]
        protected TurnAngle[] turnAngles;

        [SerializeField]
        protected float turnLerpSpeed = 2f;

        protected int BaseSpeedIndex;
        protected int BoostSpeedIndex;
        protected int BoostDurationIndex;
        protected int BoostRestoreTimeIndex;
        protected int BrakeSpeedIndex;
        protected int BrakeDurationIndex;
        protected int BrakeRestoreTimeIndex;
        protected int PitchAngleIndex;
        protected int TurnAngleIndex;

        protected Vector2 MovementInput;

        protected Vector3 CurrentVelocity;
        protected Vector3 CurrentPitch;
        protected float CurrentYaw;
        protected float CurrentRoll;

        protected float ForwardSpeed;
        protected float CurrentVerticalSpeed;
        protected float CurrentBoostDuration;
        protected float CurrentBrakeDuration;
        protected float BoostRate;
        protected float BrakeRate;
        protected float BaseReturnRate = 1;

        protected float TurnLerp;
        protected float PitchLerp;
        protected float BoostLerp;
        protected float BrakeLerp;

        protected MovementState MovementState;

        public virtual void Awake()
        {
            CurrentPitch = transform.forward;
            CurrentBoostDuration = boostDurations[BoostDurationIndex];
            ForwardSpeed = baseSpeeds[BaseSpeedIndex];
            BoostRate = boostSpeeds[BoostSpeedIndex] / startSpeedChangeTransitionTime;
            BrakeRate = brakeSpeeds[BrakeSpeedIndex] / startSpeedChangeTransitionTime;
        }

        public virtual void Update()
        {
            float deltaTime = Time.deltaTime;

            UpdateMeters(deltaTime);
            UpdateAltitude(deltaTime);
            UpdateYawAndRoll(deltaTime);
            UpdateSpeed(deltaTime);
            Move(deltaTime);
        }

        protected virtual void UpdateMeters(float deltaTime)
        {
            if (MovementState != MovementState.Default && MovementState != MovementState.Restoring)
            {
                return;
            }

            if (CurrentBoostDuration < boostSpeeds[BoostSpeedIndex])
            {
                CurrentBoostDuration = Mathf.Min(CurrentBoostDuration + deltaTime, boostSpeeds[BoostSpeedIndex]);
            }

            if (CurrentBrakeDuration < brakeSpeeds[BrakeSpeedIndex])
            {
                CurrentBrakeDuration = Mathf.Min(CurrentBrakeDuration + deltaTime, brakeSpeeds[BrakeSpeedIndex]);
            }
        }

        protected abstract void UpdateSpeed(float deltaTime);
        protected abstract void UpdateAltitude(float deltaTime);
        protected abstract void UpdateYawAndRoll(float deltaTime);
        protected abstract void Move(float deltaTime);

        public virtual void StartBoosting()
        {
            if (MovementState == MovementState.Default)
            {
                MovementState = MovementState.Boost;
                BaseReturnRate = BoostRate;
            }
        }

        public virtual void StartBraking()
        {
            if (MovementState == MovementState.Default)
            {
                MovementState = MovementState.Brake;
                BaseReturnRate = BrakeRate;
            }
        }

        public virtual void SetMovementState(MovementState movementState)
        {
            MovementState = movementState;
        }

        public virtual void IncrementIndex(MovementAttribute index)
        {
            switch (index)
            {
                case MovementAttribute.BaseSpeed:
                    baseSpeeds.IncrementIndex(ref BaseSpeedIndex);
                    break;
                case MovementAttribute.BoostSpeed:
                    boostSpeeds.IncrementIndex(ref BoostSpeedIndex);
                    break;
                case MovementAttribute.BoostDuration:
                    boostDurations.IncrementIndex(ref BoostDurationIndex);
                    break;
                case MovementAttribute.BrakeSpeed:
                    brakeSpeeds.IncrementIndex(ref BrakeSpeedIndex);
                    break;
                case MovementAttribute.BrakeDuration:
                    brakeDurations.IncrementIndex(ref BrakeDurationIndex);
                    break;
                case MovementAttribute.PitchAngle:
                    pitchAngles.IncrementIndex(ref PitchAngleIndex);
                    break;
                case MovementAttribute.TurnAngle:
                    turnAngles.IncrementIndex(ref TurnAngleIndex);
                    break;
                case MovementAttribute.BoostRestoreTime:
                    turnAngles.IncrementIndex(ref BoostRestoreTimeIndex);
                    break;
                case MovementAttribute.BrakeRestoreTime:
                    turnAngles.IncrementIndex(ref BrakeRestoreTimeIndex);
                    break;
            }
        }

        public void SetMovementInput(Vector2 movementInput)
        {
            MovementInput = movementInput;
        }
    }
}