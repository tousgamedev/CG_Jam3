using UnityEngine;

namespace BuildSystem
{
    public class ObjectDrag : MonoBehaviour
    {
        private Vector3 offset;

        private void OnMouseDown()
        {
            offset = transform.position - Utils.GetMouseWorldPosition();
        }

        private void OnMouseDrag()
        {
            Vector3 position = Utils.GetMouseWorldPosition();
            transform.position = BuildingSystem.Instance.GetGridSnapPosition(position);
        }
    }
}
