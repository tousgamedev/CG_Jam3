using System;
using Characters;
using Movement;
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
        public Action<int> OnScoreUpdated;

        public int Score { get; private set; }
        public bool GameOver { get; private set; }

        [SerializeField] private PlayerController player;

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
        
        public Transform GetTarget(Vector3 currentPosition, TargetTypes targetType)
        {
            switch (targetType)
            {
                case TargetTypes.Base:
                    return GameObject.FindGameObjectWithTag("Base").transform;
                case TargetTypes.Player:
                    return GameObject.FindGameObjectWithTag("Player").transform;
                case TargetTypes.Turret:
                    GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
                    if (turrets is not { Length: > 0 })
                    {
                        return null;
                    }

                    Transform closestTurret = null;
                    float closestTurretDistance = 0;
                    if (turrets.Length > 0)
                    {
                        foreach (GameObject turret in turrets)
                        {
                            if (closestTurret == null)
                            {
                                closestTurret = turret.transform;
                                closestTurretDistance = Vector3.Distance(currentPosition, closestTurret.position);
                                continue;
                            }

                            float distance = Vector3.Distance(currentPosition, turret.transform.position);
                            if (distance >= closestTurretDistance)
                            {
                                continue;
                            }

                            closestTurret = turret.transform;
                            closestTurretDistance = distance;
                        }
                    }

                    break;
                case TargetTypes.None:
                default:
                    Debug.LogError("No valid target set!");
                    break;
            }

            return null;
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

        public void OnPlayerDeath(BaseCharacterController controller)
        {
            GameOver = true;
            OnGameOver?.Invoke();
        }

        public void OnBaseDeath(BaseCharacterController controller)
        {
            GameOver = true;
            OnGameOver?.Invoke();
        }

        public void UpgradeFirepower()
        {
            player.PlayerWeaponController.UpgradeFirepower();
        }

        public void UpgradeRateOfFire()
        {
            player.PlayerWeaponController.UpgradeRateOfFire();
        }

        public void UpgradeBaseSpeed()
        {
            player.MovementMode.IncrementIndex(MovementAttribute.BaseSpeed);
        }

        public void UpgradeBoost()
        {
            player.MovementMode.IncrementIndex(MovementAttribute.BoostSpeed);
        }

        private static void LockMouse(bool isLocked)
        {
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isLocked;
        }
    }
}