using System.Collections.Generic;
using UnityEngine;

namespace Weapons.Controllers
{
    public abstract class BaseWeaponController : MonoBehaviour
    {
        [SerializeField, Tooltip("Shots per minute")]
        protected List<float> ratesOfFire = new()
        {
            120,
            180,
            240
        };

        [SerializeField]
        protected List<GameObject> bulletPrefabs;

        [SerializeField]
        protected Transform bulletSpawn;
        
        protected float FireInterval;
        protected float ShotTimer;
        protected float SpawnAdjust;
        
        protected int PrefabIndex;
        protected int RateOfFireIndex;

        protected virtual void Awake()
        {
            if (bulletPrefabs == null || bulletPrefabs.Count == 0)
            {
                Debug.LogError("No bullet prefabs assigned. Please assign at least one prefab in the inspector.");
                enabled = false;
                return;
            }

            if (bulletSpawn == null)
            {
                Debug.LogError("Bullet spawn transform not assigned. Please assign it in the inspector.");
                enabled = false;
                return;
            }
            
            FireInterval = 60f / ratesOfFire[RateOfFireIndex];
            ShotTimer = FireInterval;
        }
        
        public virtual void UpgradeFirepower()
        {
            PrefabIndex = Mathf.Clamp(PrefabIndex + 1, 0, bulletPrefabs.Count - 1);
        }
        
        public virtual void UpgradeRateOfFire()
        {
            RateOfFireIndex = Mathf.Clamp(RateOfFireIndex + 1, 0, ratesOfFire.Count - 1);
        }

        protected abstract Vector3 CalculateSpawnPosition();
    }
}