using Aircraft;
using Characters;
using Managers;
using Movement;
using UI;
using UnityEngine;
using Weapons;
using Weapons.Controllers;

namespace Player
{
    public class PlayerController : BaseCharacterController
    {
        public BaseMovementMode MovementMode => movementMode;
        public PlayerWeaponController PlayerWeaponController => playerWeaponController;
        public Health Health => health;

        [SerializeField] private GameObject destroyPrefab;
        
        private BaseMovementMode movementMode;
        private PlayerWeaponController playerWeaponController;
        private Health health;

        private void Awake()
        {
            if (TryGetComponent(out movementMode))
            {
                movementMode.OnBoostDurationChange += PlayerCanvas.Instance.Status.SetBoost;
            }
            
            if(TryGetComponent(out playerWeaponController))
            {
                playerWeaponController.SetPlayerHub(this);
            }

            if (TryGetComponent(out health))
            {
                health.OnHealthDepleted += GameManager.Instance.OnPlayerDeath;
                health.OnHealthDepleted += OnDeath;
                health.OnHealthChange += UpdateHealth;
                health.SetController(this);
            }
        }

        private static void UpdateHealth(float percentage)
        {
            PlayerCanvas.Instance.Status.SetHealth(percentage);
        }
        
        private void OnDeath(BaseCharacterController controllerBase)
        {
            MovementMode.enabled = false;
            PlayerWeaponController.enabled = false;
            movementMode.OnBoostDurationChange -= PlayerCanvas.Instance.Status.SetBoost;
            health.OnHealthDepleted -= OnDeath;
            health.OnHealthDepleted -= GameManager.Instance.OnPlayerDeath;
            health.OnHealthChange -= UpdateHealth;
            GameObject effect = Instantiate(destroyPrefab);
            Destroy(effect, 3);
            Destroy(gameObject);
        }
    }
}