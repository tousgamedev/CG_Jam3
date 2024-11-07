using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Graphic))]
    public class ChangeGradient : MonoBehaviour
    {
        public Color startColor;
        public Color endColor;
        private Graphic graphic;

        private Graphic Graphic
        {
            get
            {
                if (graphic == null)
                {
                    graphic = GetComponent<Graphic>();
                }

                return graphic;
            }
        }

        public void OnValueChanged(float value)
        {
            Graphic.color = Color.Lerp(startColor, endColor, value);
        }
    }
}