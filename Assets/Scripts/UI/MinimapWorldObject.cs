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
            MinimapController.Instance.RemoveMinimapWorldObject(this);
        }
    }
}