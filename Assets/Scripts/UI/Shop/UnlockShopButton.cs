using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Shop
{
    public class UnlockShopButton : MonoBehaviour
    {
        [SerializeField] private GameObject purchaseOptions;
        [SerializeField] private GameObject unlockPrompt;
        [SerializeField] private int unlockPrice;
        [SerializeField] private Button unlockShopButton;
        [SerializeField] private TextMeshProUGUI priceText;

        private void OnEnable()
        {
            priceText.text = $"{unlockPrice}";
            if (isActiveAndEnabled)
            {
                unlockShopButton.interactable = GameManager.Instance.Score >= unlockPrice;
            }
        }

        public void UnlockShop()
        {
            unlockPrompt.SetActive(false);
            purchaseOptions.SetActive(true);
        }

        public void BuildTurret(int turretID)
        {
            
        }

        public void BuildRadar()
        {
            
        }
    }
}