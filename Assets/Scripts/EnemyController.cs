using Aircraft;
using Characters;
using Characters.Enemy;
using Managers;
using System.Collections.Generic;
using Player;
using UnityEngine;
using Weapons.Controllers;

namespace Movement
{
    public enum EnemyStates
    {
        Spawn,
        Track,
        Attack,
        Evade,
        Die,
        Idle
    }

    public enum TargetTypes
    {
        None,
        Base,
        Player,
        Turret
    }

    public class EnemyController : BaseCharacterController
    {
        public Health Health => health;
        public EnemyWeaponController WeaponController => weaponController;
        public BaseMovementMode MovementMode => movementMode;
        public TargetTypes TargetType => targetType;
        public Transform CurrentTarget { get; private set; }
        public float EvasionRange => evasionRange;
        public float EngageRange => engageRange;
        public float FireRange => fireRange;

        [Header("Combat"), SerializeField]
        private TargetTypes targetType;

        [SerializeField]
        private float evasionRange = 75f;
        
        [SerializeField]
        private float engageRange = 200f;

        [SerializeField]
        private float fireRange = 400f;

        [SerializeField]
        private float attackArc = 20f;

        [SerializeField]
        private Transform shotSpawnPoint;

        [Header("Misc"), SerializeField]
        private int scoreValue = 100;

        [SerializeField]
        private GameObject deathPrefab;

        private Health health;
        private Animator animator;
        private EnemyState currentState;
        private EnemyWeaponController weaponController;
        private BaseMovementMode movementMode;
        private BaseMovementMode targetMovementMode;

        private readonly Dictionary<EnemyStates, EnemyState> states = new()
        {
            { EnemyStates.Spawn, new EnemySpawnState() },
            { EnemyStates.Track, new EnemyTrackingState() },
            { EnemyStates.Attack, new EnemyAttackState() },
            { EnemyStates.Evade, new EnemyEvadeState() },
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

            if (TryGetComponent(out movementMode))
            {
            }

            if (targetType == TargetTypes.None)
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
        
        public void OnEvade()
        {
            ChangeState(EnemyStates.Evade);
        }

        public void RotateToTarget()
        {
            if (CurrentTarget == null)
            {
                MovementMode.SetMovementInput(Vector2.zero);
                return;
            }

            Vector3 directionToTarget = transform.DirectionTo(CurrentTarget);
            directionToTarget.y = 0;
            if (directionToTarget == Vector3.zero)
            {
                MovementMode.SetMovementInput(Vector2.zero);
                return;
            }

            directionToTarget.Normalize();

            float angleToTarget = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
            float normalizedYaw = Mathf.Clamp(angleToTarget / MovementMode.TurnAngles.yaw, -1f, 1f);

            MovementMode.SetMovementInput(new(normalizedYaw, 0));
        }

        public void GetTarget()
        {
            CurrentTarget = GameManager.Instance.GetTarget(transform.position, targetType);
            if (targetType != TargetTypes.Player || CurrentTarget == null)
            {
                targetMovementMode = null;
                return;
            }

            CurrentTarget.gameObject.TryGetComponent(out targetMovementMode);
        }

        public bool IsTargetInEvasionRange()
        {
            if (CurrentTarget == null)
            {
                return false;
            }
            
            return Vector3.Distance(transform.position, CurrentTarget.position) <= EvasionRange;
        }
        
        public bool IsTargetInEngagementRange()
        {
            if (CurrentTarget == null)
            {
                return false;
            }

            return Vector3.Distance(CurrentTarget.position, transform.position) < EngageRange;
        }

        public bool IsTargetInFiringRange()
        {
            if (CurrentTarget == null)
            {
                return false;
            }

            return Vector3.Distance(CurrentTarget.position, transform.position) < FireRange;
        }

        public bool IsTargetWithinFiringArc()
        {
            // Calculate the direction from the enemy to the target
            Vector3 directionToTarget = (CurrentTarget.position - transform.position).normalized;

            // Calculate the dot product between the enemy's forward direction and the direction to the target
            float dotProduct = Vector3.Dot(transform.forward, directionToTarget);

            // Calculate the cosine of half the arc angle
            float cosAngleThreshold = Mathf.Cos(attackArc * 0.5f * Mathf.Deg2Rad);

            // Check if the dot product meets or exceeds the threshold
            return dotProduct >= cosAngleThreshold;
        }

        public bool IsTargetWithinTurnArc(float multiplier)
        {
            // Calculate the direction vector to the target
            Vector3 toTarget = transform.DirectionTo(CurrentTarget).normalized;

            // Calculate the angle between the vehicle's forward direction and the direction to the target
            float angleToTarget = Vector3.Angle(transform.forward, toTarget);

            // Check if the angle is within twice the turning arc
            return angleToTarget <= movementMode.TurnAngles.yaw * multiplier;
        }

        public int GetEvasionDirection()
        {
            Vector3 toEnemy = CurrentTarget.DirectionTo(transform).normalized;

            // Check if the enemy is within the cone
            float dotProduct = Vector3.Dot(CurrentTarget.forward, toEnemy);
            float cosConeThreshold = Mathf.Cos(60 * 0.5f * Mathf.Deg2Rad);

            if (dotProduct > cosConeThreshold) // Inside the cone
            {
                // Determine the angle between the enemy's forward direction and the direction out of the cone
                float angleToTarget = Vector3.SignedAngle(CurrentTarget.forward, -toEnemy, Vector3.up);

                // Check if the enemy is already facing roughly in the direction to leave the cone
                float angleToEnemyForward = Mathf.DeltaAngle(Vector3.SignedAngle(transform.forward, -toEnemy, Vector3.up), 0f);

                // If the angle is small (within a threshold), go straight
                const float alignmentThreshold = 5f; // Degrees
                if (Mathf.Abs(angleToEnemyForward) < alignmentThreshold)
                {
                    return 0; // Keep moving straight
                }

                // Otherwise, turn left or right based on the angle to the target
                return angleToTarget > 0 ? 1 : -1;
            }

            // Enemy is outside the cone, no evasion needed
            return 0;
        }

        private void OnDie(BaseCharacterController controller)
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
            Debug.Log($"Changing to state: {newState}");
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