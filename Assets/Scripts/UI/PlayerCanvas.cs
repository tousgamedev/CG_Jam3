using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerCanvas : MonoBehaviour
    {
        public static PlayerCanvas Instance;

        public PlayerStatus Status => playerStatus;
        
        [SerializeField] private PlayerStatus playerStatus;

        [Header("Crosshairs")]
        [SerializeField] private FollowTransform crosshairTargetNear;
        [SerializeField] private FollowTransform crosshairTargetFar;
        
        [Header("Misc")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private GameObject pauseImage;
        [SerializeField] private GameObject minimap;
        [SerializeField] private GameObject shop;
        [SerializeField] private GameObject gameOverScreen;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                RegisterEvents();
                ToggleMenu(isPaused: false);
                minimap.gameObject.SetActive(false);
                gameOverScreen.SetActive(false);
                return;
            }
            
            Destroy(gameObject);
        }

        private void RegisterEvents()
        {
            GameManager.Instance.OnScoreUpdated += UpdateScore;
            GameManager.Instance.OnPause += OnPause;
            GameManager.Instance.OnResume += OnResume;
            GameManager.Instance.OnGameOver += OnGameOver;
        }



        public void SetCrosshairTargetNear(Transform crosshairTarget)
        {
            crosshairTargetNear.SetTarget(crosshairTarget);
        }

        public void SetCrosshairTargetFar(Transform crosshairTarget)
        {
            crosshairTargetFar.SetTarget(crosshairTarget);
        }

        private void UpdateScore(int score)
        {
            scoreText.text = $"${score}";
        }

        private void OnPause()
        {
            ToggleMenu(isPaused: true);
        }

        private void OnResume()
        {
            ToggleMenu(isPaused: false);
        }
        
        private void ToggleMenu(bool isPaused)
        {
            pauseImage.SetActive(isPaused);
            shop.SetActive(isPaused);
            crosshairTargetNear.gameObject.SetActive(!isPaused);
            crosshairTargetFar.gameObject.SetActive(!isPaused);
            playerStatus.gameObject.SetActive(!isPaused);
            if (PlayerBaseManager.Instance.RadarActive)
            {
                minimap.gameObject.SetActive(!isPaused);
            }
        }
        
        private void OnGameOver()
        {
            gameOverScreen.SetActive(true);
            pauseImage.SetActive(false);
            shop.SetActive(false);
            crosshairTargetNear.gameObject.SetActive(false);
            crosshairTargetFar.gameObject.SetActive(false);
            playerStatus.gameObject.SetActive(false);
            if (PlayerBaseManager.Instance.RadarActive)
            {
                minimap.gameObject.SetActive(false);
            }
        }
    }
}