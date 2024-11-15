using UnityEngine;

namespace UI
{
    public class CrosshairController : MonoBehaviour
    {
        [SerializeField] private Transform nearTarget;
        [SerializeField] private Transform farTarget;

        private void Start()
        {
            PlayerCanvas.Instance.SetCrosshairTargetFar(farTarget);
            PlayerCanvas.Instance.SetCrosshairTargetNear(nearTarget);
        }
    }
}
