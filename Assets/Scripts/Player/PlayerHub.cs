using Aircraft;
using UI;
using UnityEngine;
using Weapons;

namespace Player
{
    public class PlayerHub : MonoBehaviour
    {
        public FlightController FlightController => flightController;
        public WeaponController WeaponController => weaponController;
        public Health Health => health;

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
                health.OnHealthChange += UpdateHealth;
            }
        }

        private void UpdateHealth()
        {
            PlayerHud.Instance.SetHealth(health.Percentage);
        }
    }
}