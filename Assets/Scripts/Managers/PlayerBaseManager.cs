using Aircraft;
using System;
using System.Collections.Generic;
using Movement;
using UI.Minimap;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class PlayerBaseManager : MonoBehaviour
    {
        public static PlayerBaseManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<PlayerBaseManager>();
                }

                return instance;
            }
        }

        private static PlayerBaseManager instance;

        public Action OnRadarUnlock;

        public bool RadarActive { get; private set; }

        [SerializeField] private Health baseHealth;
        [SerializeField] private Slider baseHealthBar;
        [SerializeField] private List<TurretController> turrets;
        [SerializeField] private GameObject radar;
        [SerializeField] private MinimapController controller;

        private void Awake()
        {
            radar.SetActive(false);
            baseHealth.OnHealthChange += OnHealthChange;
            baseHealth.OnHealthDepleted += GameManager.Instance.OnBaseDeath;

            foreach (TurretController turret in turrets)
            {
                turret.gameObject.SetActive(false);
            }
        }

        public void UnlockRadar()
        {
            RadarActive = true;
            radar.SetActive(true);
            OnRadarUnlock?.Invoke();
        }
        
        public void UpgradeRadarRange()
        {
            controller.UpgradeRange();
        }
        
        public void UpgradeRadarScanRate()
        {
            controller.UpgradeScanRate();
        }

        public void ActivateTurret(int index)
        {
            if (!IsValidTurretIndex(index))
            {
                return;
            }

            if (turrets[index] != null)
            {
                turrets[index].gameObject.SetActive(true);
            }
        }

        public void UpgradeTurretFirepower(int index)
        {
            if (!IsValidTurretIndex(index))
            {
                return;
            }

            if (turrets[index] != null)
            {
                turrets[index].UpgradeFirepower();
            }
        }

        public void UpgradeTurretRateOfFire(int index)
        {
            if (!IsValidTurretIndex(index))
            {
                return;
            }

            if (turrets[index] != null)
            {
                turrets[index].UpgradeRateOfFire();
            }
        }

        private bool IsValidTurretIndex(int index)
        {
            return index >= 0 && index < turrets.Count;
        }

        private void OnHealthChange(float percent)
        {
            baseHealthBar.value = percent;
        }
    }
}