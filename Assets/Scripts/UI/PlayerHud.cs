using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerHud : MonoBehaviour
    {
        public static PlayerHud Instance;

        [Header("Status Bars")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider shieldSlider;
        [SerializeField] private Slider boostSlider;

        [Header("Crosshairs")]
        [SerializeField] private FollowTransform crosshairTargetNear;
        [SerializeField] private FollowTransform crosshairTargetFar;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                return;
            }

            Destroy(gameObject);
        }

        public void SetHealth(float value)
        {
            healthSlider.value = ClampValue(value);
        }

        public void SetShield(float value)
        {
            shieldSlider.value = ClampValue(value);
        }

        public void SetBoost(float value)
        {
            boostSlider.value = ClampValue(value);
        }

        public void SetCrosshairTargetNear(Transform crosshairTarget)
        {
            crosshairTargetNear.SetTarget(crosshairTarget);
        }

        public void SetCrosshairTargetFar(Transform crosshairTarget)
        {
            crosshairTargetFar.SetTarget(crosshairTarget);
        }

        private static float ClampValue(float value)
        {
            return Mathf.Clamp(value, 0f, 1f);
        }
    }
}