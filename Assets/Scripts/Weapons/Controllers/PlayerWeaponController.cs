using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Weapons.Controllers
{
    public class PlayerWeaponController : BaseWeaponController
    {
        private PlayerController controller;

        private void Update()
        {
            if (Time.timeScale == 0)
            {
                return;
            }
            
            if (Input.GetMouseButton(0) && ShotTimer > FireInterval)
            {
                SpawnProjectile();
            }

            ShotTimer += Time.deltaTime;
        }

        private void SpawnProjectile()
        {
            Vector3 adjustedPosition = CalculateSpawnPosition();
            ShotTimer = 0;
            var laser = Instantiate(bulletPrefabs[PrefabIndex], adjustedPosition, bulletSpawn.rotation).GetComponent<Laser>();
            if (laser != null)
            {
                laser.SetFaction(controller.Health.Faction);
            }
        }

        public void SetPlayerHub(PlayerController playerController)
        {
            controller = playerController;
        }

        protected override Vector3 CalculateSpawnPosition()
        {
            float playerSpeed = controller.MovementMode.Speed;
            float additionalOffset = playerSpeed * Time.deltaTime;
            return bulletSpawn.position + bulletSpawn.forward * additionalOffset;
        }
    }
}