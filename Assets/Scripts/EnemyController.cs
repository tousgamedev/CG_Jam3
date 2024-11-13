using System.Collections.Generic;
using Aircraft;
using Characters;
using Characters.Enemy;
using Managers;
using UnityEngine;

namespace Movement
{
    public enum EnemyStates
    {
        Spawn,
        Track,
        Attack,
        Die,
        Idle
    }

    public enum Target
    {
        None,
        Base,
        Player,
        Turret
    }

    public class EnemyController : BaseCharacterController
    {
        public Health Health => health;
        public float CurrentSpeed { get; set; }
        public EnemyWeaponController WeaponController => weaponController;
        public Target Target => target;
        public float EngageRange => engageRange;
        public float FireRange => fireRange;
        public float MoveSpeed => moveSpeed;
        public float RotationSpeed => rotationSpeed;

        [Header("Movement")] [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("Combat")] [SerializeField] private Target target;
        [SerializeField] private float engageRange = 200f;
        [SerializeField] private float fireRange = 400f;
        [SerializeField] private Transform shotSpawnPoint;

        [Header("Misc")] [SerializeField] private int scoreValue = 100;
        [SerializeField] private GameObject deathPrefab;

        private Health health;
        private Animator animator;
        private EnemyState currentState;
        private EnemyWeaponController weaponController;
        private float currentSpeed;

        private readonly Dictionary<EnemyStates, EnemyState> states = new()
        {
            { EnemyStates.Spawn, new EnemySpawnState() },
            { EnemyStates.Track, new EnemyTrackState() },
            { EnemyStates.Attack, new EnemyAttackState() },
            { EnemyStates.Die, new EnemyDieState() },
            { EnemyStates.Idle, new EnemyIdleState() }
        };

        private void Awake()
        {
            if (TryGetComponent(out health))
            {
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
            GameManager.Instance.OnGameOver += OnGameOver;
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

        public void OnDie(BaseCharacterController controller)
        {
            GameManager.Instance.AddScore(scoreValue);
            ChangeState(EnemyStates.Die);
        }

        public void PlayDeathEffects()
        {
            Instantiate(deathPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        private void ChangeState(EnemyStates newState)
        {
            currentState?.OnExit();
            if (states.TryGetValue(newState, out currentState))
            {
                currentState.OnEnter(this);
            }
        }

        private void OnGameOver()
        {
            ChangeState(EnemyStates.Idle);
        }
    }
}