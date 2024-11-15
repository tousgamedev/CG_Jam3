using Aircraft;
using Characters.Player;
using Movement;
using UnityEngine;

namespace UI
{
    public class FollowTransform : MonoBehaviour
    {
        private Transform target;
        private RectTransform rectTransform;
        private Camera mainCam;
        
        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            mainCam = Camera.main;
        }

        private void Update()
        {
            if (target == null)
            {
                return;
            }

            Vector2 viewportPoint = mainCam.WorldToViewportPoint(target.position);
            // set MIN and MAX Anchor values(positions) to the same position (ViewportPoint)
            rectTransform.anchorMin = viewportPoint;
            rectTransform.anchorMax = viewportPoint;
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}