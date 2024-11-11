using Aircraft;
using Character;
using Managers;
using UI;
using UnityEngine;
using Weapons;

namespace Characters.Player
{
    public class PlayerHub : MonoBehaviour
    {
        public FlightController FlightController => flightController;
        public WeaponController WeaponController => weaponController;
        public Health Health => health;

        [SerializeField] private GameObject destroyPrefab;
        
        private FlightController flightController;
        private WeaponController weaponController;
        private Health health;

        private void Awake()
        {
            if(TryGetComponent(out flightController))
            {
                flightController.SetPlayerHub(this);
            }
            
            if(TryGetComponent(out weaponController))
            {
                weaponController.SetPlayerHub(this);
            }

            if (TryGetComponent(out health))
            {
                health.OnHealthDepleted += GameManager.Instance.OnPlayerDeath;
                health.OnHealthDepleted += OnDeath;
                health.SetController(flightController);
                health.OnHealthChange += UpdateHealth;
            }
        }

        private void UpdateHealth(float percentage)
        {
            PlayerCanvas.Instance.SetHealth(percentage);
        }
        
        private void OnDeath(ControllerBase controllerBase)
        {
            FlightController.enabled = false;
            WeaponController.enabled = false;
            GameObject effect = Instantiate(destroyPrefab);
            Destroy(effect, 3);
            Destroy(gameObject);
        }
    }
}