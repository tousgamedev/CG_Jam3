using Player;
using UnityEngine;

namespace Weapons
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField, Tooltip("Shots per minute")]
        private float rateOfFire;

        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform bulletSpawn;

        private PlayerHub hub;
        private float timeBetweenShots;
        private float shotTimer;
        private float spawnAdjust;

        private void Start()
        {
            timeBetweenShots = 60f / rateOfFire;
            shotTimer = timeBetweenShots;
        }

        private void Update()
        {
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
            var laser = Instantiate(bulletPrefab, adjustedPosition, bulletSpawn.rotation).GetComponent<Laser>();
            if (laser != null)
            {
                laser.SetFaction(hub.Health.Faction);
            }
        }

        public void SetPlayerHub(PlayerHub playerHub)
        {
            hub = playerHub;
        }

        private Vector3 CalculateSpawnPosition()
        {
            float playerSpeed = hub.FlightController.CurrentSpeed;
            float additionalOffset = playerSpeed * Time.deltaTime;
            return bulletSpawn.position + bulletSpawn.forward * additionalOffset;
        }
    }
}