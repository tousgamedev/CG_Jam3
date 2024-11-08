using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Shop
{
    public enum TurretShopTypes
    {
        None,
        Firepower,
        RateOfFire
    }

    public class TurretShopEntry : MonoBehaviour
    {
        [SerializeField] private Button buyButton;
        [SerializeField] private TurretShopTypes shopType;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private List<int> prices;
        [SerializeField] private int turretIndex;

        private int levelIndex;

        private void Awake()
        {
            if (buyButton == null)
            {
                Debug.LogError("BuyButton is null");
            }

            if (priceText == null)
            {
                Debug.LogError("PriceText is null");
            }

            if (levelText == null)
            {
                Debug.LogError("LevelText is null");
            }

            GameManager.Instance.OnScoreUpdated += OnScoreUpdated;

            priceText.text = $"${prices[levelIndex]}";
            levelText.text = $"Level {levelIndex + 1}";
        }

        private void OnEnable()
        {
            buyButton.interactable = CanPurchase();
        }

        private bool CanPurchase()
        {
            return GameManager.Instance.Score >= prices[levelIndex]
                   && levelIndex < prices.Count - 1;
        }

        private void OnScoreUpdated(int score)
        {
            buyButton.interactable = CanPurchase();
        }

        private void HandlePurchase()
        {
            GameManager.Instance.SubtractScore(prices[levelIndex]);

            levelIndex++;
            buyButton.interactable = CanPurchase();
            priceText.text = levelIndex >= prices.Count - 1
                ? "MAX"
                : $"${prices[levelIndex]}";

            levelText.text = $"Level {levelIndex + 1}";
        }

        public void UpgradeFirepower()
        {
            PlayerBaseManager.Instance.UpgradeTurretFirepower(turretIndex);
            HandlePurchase();
        }

        public void UpgradeRateOfFire()
        {
            PlayerBaseManager.Instance.UpgradeTurretRateOfFire(turretIndex);
            HandlePurchase();
        }
    }
}