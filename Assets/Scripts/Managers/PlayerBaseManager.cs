using System;
using Aircraft;
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

        public Action OnRadarActivate;
        public Action OnBaseHealthChange;
        
        public bool RadarActive { get; private set; }

        [SerializeField] private Health baseHealth;
        [SerializeField] private Slider baseHealthBar;
        [SerializeField] private GameObject radar;

        private void Awake()
        {
            radar.SetActive(false);
            baseHealth.OnHealthDepleted += GameManager.Instance.OnBaseDeath;
        }

        public void UpgradeRadar()
        {
            if (!RadarActive)
            {
                RadarActive = true;
                radar.SetActive(true);
                OnRadarActivate?.Invoke();
                return;
            }
            
            // Do other upgrade stuff
        }
    }
}