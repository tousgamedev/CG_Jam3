using System;
using Character;
using Characters.Player;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<GameManager>();
                }

                return instance;
            }
        }

        private static GameManager instance;


        public Action OnPause;
        public Action OnResume;
        public Action OnGameOver;
        public Action<int> OnScoreUpdated;

        public int Score { get; private set; }
        public bool GameOver { get; private set; }

        [SerializeField] private PlayerHub player;

        private bool isPaused;

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.M))
            {
                Score += 50000;
                OnScoreUpdated?.Invoke(Score);
            }
#endif
            
            TogglePause();
        }

        public void AddScore(int scoreToAdd)
        {
            Score += scoreToAdd;
            OnScoreUpdated?.Invoke(Score);
        }

        public void SubtractScore(int scoreToSubtract)
        {
            Score -= scoreToSubtract;
            OnScoreUpdated?.Invoke(Score);
        }

        private void TogglePause()
        {
            if (!Input.GetKeyDown(KeyCode.P))
            {
                return;
            }

            isPaused = !isPaused;
            if (isPaused)
            {
                LockMouse(false);
                Time.timeScale = 0f;
                OnPause?.Invoke();
            }
            else
            {
                LockMouse(true);
                Time.timeScale = 1;
                OnResume?.Invoke();
            }
        }

        public void OnPlayerDeath(ControllerBase controller)
        {
            GameOver = true;
            OnGameOver?.Invoke();
        }

        public void OnBaseDeath(ControllerBase controller)
        {
            GameOver = true;
            OnGameOver?.Invoke();
        }

        public void UpgradeFirepower()
        {
            player.WeaponController.UpgradeFirepower();
        }

        public void UpgradeRateOfFire()
        {
            player.WeaponController.UpgradeRateOfFire();
        }

        public void UpgradeThrust()
        {
            player.FlightController.UpgradeThrust();
        }

        public void UpgradeAfterburner()
        {
            player.FlightController.UpgradeAfterburner();
        }

        private static void LockMouse(bool isLocked)
        {
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isLocked;
        }
    }
}