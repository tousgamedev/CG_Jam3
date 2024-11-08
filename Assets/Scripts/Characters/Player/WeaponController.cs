using System.Collections.Generic;
using Characters.Player;
using UnityEngine;

namespace Weapons
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField, Tooltip("Shots per minute")]
        private List<float> ratesOfFire = new()
        {
            120,
            180,
            240
        };

        [SerializeField] private List<GameObject> bulletPrefabs;
        [SerializeField] private Transform bulletSpawn;

        private PlayerHub hub;
        private float timeBetweenShots;
        private float shotTimer;
        private float spawnAdjust;
        
        private int prefabIndex;
        private int rateOfFireIndex;
        
        private void Start()
        {
            timeBetweenShots = 60f / ratesOfFire[rateOfFireIndex];
            shotTimer = timeBetweenShots;
        }

        private void Update()
        {
            if (Time.timeScale == 0)
            {
                return;
            }
            
            if (Input.GetMouseButton(0) && shotTimer > timeBetweenShots)
            {
                SpawnProjectile();
            }

            shotTimer += Time.deltaTime;
        }

        private void SpawnProjectile()
        {
            Vector3 adjustedPosition = CalculateSpawnPosition();
            shotTimer = 0;
            var laser = Instantiate(bulletPrefabs[prefabIndex], adjustedPosition, bulletSpawn.rotation).GetComponent<Laser>();
            if (laser != null)
            {
                laser.SetFaction(hub.Health.Faction);
            }
        }

        public void SetPlayerHub(PlayerHub playerHub)
        {
            hub = playerHub;
        }

        public void UpgradeFirepower()
        {
            if (prefabIndex < bulletPrefabs.Count - 1)
            {
                prefabIndex++;
            }
        }
        
        public void UpgradeRateOfFire()
        {
            if (rateOfFireIndex < ratesOfFire.Count - 1)
            {
                rateOfFireIndex++;
                timeBetweenShots = 60f / ratesOfFire[rateOfFireIndex];
            }
        }

        private Vector3 CalculateSpawnPosition()
        {
            float playerSpeed = hub.FlightController.CurrentSpeed;
            float additionalOffset = playerSpeed * Time.deltaTime;
            return bulletSpawn.position + bulletSpawn.forward * additionalOffset;
        }
    }
}