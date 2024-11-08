using System;
using Aircraft;
using Character;
using Player;
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
        public Action OnScoreUpdated;

        public int Score { get; private set; }

        [SerializeField] private PlayerHub player;

        private bool isPaused;

        private void Update()
        {
            TogglePause();
        }

        public void AddScore(int scoreToAdd)
        {
            Score += scoreToAdd;
        }

        public void SubtractScore(int scoreToSubtract)
        {
            Score -= scoreToSubtract;
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
                Time.timeScale = 0;
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
            OnGameOver?.Invoke();
        }

        private void LockMouse(bool isLocked)
        {
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isLocked;
        }
    }
}