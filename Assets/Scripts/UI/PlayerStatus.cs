using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerStatus : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider shieldSlider;
        [SerializeField] private Slider boostSlider;
        
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
        
        private static float ClampValue(float value)
        {
            return Mathf.Clamp(value, 0f, 1f);
        }
    }
}
