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
        public Health Health => health;
        public EnemyWeaponController WeaponController => weaponController;
        public Target Target => target;
        public float EngageRange => engageRange;
        public float FireRange => fireRange;
        public float MoveSpeed => moveSpeed;
        public float RotationSpeed => rotationSpeed;

        [SerializeField] private Target target;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float engageRange = 200f;
        [SerializeField] private float fireRange = 400f;
        [SerializeField] private Transform shotSpawnPoint;

        private Health health;
        private Animator animator;
        private EnemyState currentState;
        private EnemyWeaponController weaponController;

        private readonly Dictionary<EnemyStates, EnemyState> states = new()
        {
            { EnemyStates.Spawn, new EnemySpawnState() },
            { EnemyStates.Track, new EnemyTrackState() },
            { EnemyStates.Attack, new EnemyAttackState() },
        };

        private void Awake()
        {
            if (!TryGetComponent(out health))
            {
                Debug.LogError("No Health component found!");
                health.OnHealthDepleted += OnDie;
            }

            if (TryGetComponent(out weaponController))
            {
                weaponController.SetEnemyController(this);
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

        public void OnEngage()
        {
            ChangeState(EnemyStates.Attack);
        }

        public void OnDisengage()
        {
            ChangeState(EnemyStates.Track);
        }

        public void OnDie()
        {
            ChangeState(EnemyStates.Die);
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