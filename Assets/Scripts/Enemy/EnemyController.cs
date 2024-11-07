using System;
using System.Collections.Generic;
using Aircraft;
using UnityEngine;

namespace Enemy
{
    public enum EnemyStates
    {
        Spawn,
        Track,
        Attack,
        Die
    }

    public enum Target
    {
        None,
        Base,
        Player,
        Turret
    }

    public class EnemyController : MonoBehaviour
    {
        public Target Target => target;
        public float EngageRange => engageRange;
        public float FireRange => fireRange;
        public float MoveSpeed => moveSpeed;
        public float RotationSpeed => rotationSpeed;
        public Transform ShotSpawnPoint => shotSpawnPoint;

        [SerializeField] private Target target;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float engageRange = 200f;
        [SerializeField] private float fireRange = 400f;
        [SerializeField] private Transform shotSpawnPoint;

        private Health health;
        private Animator animator;
        private EnemyState currentState;

        private readonly Dictionary<EnemyStates, EnemyState> states = new()
        {
            { EnemyStates.Spawn, new EnemySpawnState() },
            { EnemyStates.Track, new EnemyTrackState() },
        };

        private void Awake()
        {
            if (!TryGetComponent(out health))
            {
                Debug.LogError("No Health component found!");
            }

            if (target == Target.None)
            {
                Debug.LogError("No valid Target found!");
            }

            ChangeState(EnemyStates.Spawn);
        }

        private void Update()
        {
            currentState.OnUpdate(Time.deltaTime);
        }

        public void OnFinishDropEntry()
        {
            ChangeState(EnemyStates.Track);
        }

        private void ChangeState(EnemyStates newState)
        {
            currentState?.OnExit();
            if (states.TryGetValue(newState, out currentState))
            {
                currentState.OnEnter(this);
            }
        }
    }
}