using UI.Minimap;
using UnityEngine;

namespace UI
{
    public class MinimapWorldObject : MonoBehaviour
    {
        [SerializeField] private bool followObject;
        [SerializeField] private Sprite minimapIcon;
        public Sprite MinimapIcon => minimapIcon;

        private void Start()
        {
            MinimapController.Instance.RegisterMinimapWorldObject(this, followObject);
        }

        private void OnDestroy()
        {
            if (minimapIcon != null && MinimapController.Instance != null)
            {
                MinimapController.Instance.RemoveMinimapWorldObject(this);
            }
        }
    }
}