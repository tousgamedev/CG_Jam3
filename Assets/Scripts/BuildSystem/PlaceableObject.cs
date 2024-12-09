using UnityEngine;

namespace BuildSystem
{
    public class PlaceableObject : MonoBehaviour
    {
        public bool IsPlaced { get; private set; }
        public Vector3Int Size { get; private set; }

        private Vector3[] vertexLocalPositions = new Vector3[4];
        private BoxCollider box;

        private void Awake()
        {
            if (!TryGetComponent(out box))
            {
                Debug.LogError($"No BoxCollider found on {gameObject.name}. PlaceableObject requires a BoxCollider.");
                enabled = false;
            }
        }

        private void Start()
        {
            Initialize();
        }
        
        public Vector3 GetStartPosition()
        {
            return transform.TransformPoint(vertexLocalPositions[0]);
        }
        
        public void RotateClockwise()
        {
            RotateObject(90);
        }

        public void RotateCounterclockwise()
        {
            RotateObject(-90);
        }
        
        public virtual void Place()
        {
            if (TryGetComponent(out ObjectDrag drag))
            {
                Destroy(drag);
            }

            IsPlaced = true;

            // Invoke placement events here
        }

        private void Initialize()
        {
            GetColliderVertexPositionLocal();
            CalculateSizeInCells();
        }

        private void GetColliderVertexPositionLocal()
        {
            Vector3 halfSize = box.bounds.extents;
            vertexLocalPositions[0] = box.center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
            vertexLocalPositions[1] = box.center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
            vertexLocalPositions[2] = box.center + new Vector3(halfSize.x, -halfSize.y, halfSize.z);
            vertexLocalPositions[3] = box.center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
        }

        private void CalculateSizeInCells()
        {
            if (BuildingSystem.Instance == null)
            {
                Debug.LogError("BuildingSystem.Instance is not available.");
                return;
            }
            
            var cellPositions = new Vector3Int[vertexLocalPositions.Length];

            for (int i = 0; i < vertexLocalPositions.Length; i++)
            {
                Vector3 worldPosition = transform.TransformPoint(vertexLocalPositions[i]);
                cellPositions[i] = BuildingSystem.Instance.GridLayout.WorldToCell(worldPosition);
            }

            Size = new(
                Mathf.Abs((cellPositions[0] - cellPositions[1]).x),
                Mathf.Abs((cellPositions[0] - cellPositions[3]).y),
                1
            );
        }

        private void RotateObject(float angle)
        {
            if (angle % 90 != 0)
            {
                Debug.LogError($"Angle {angle} is not a valid angle.");
                return;
            }
            
            transform.Rotate(0, angle, 0, Space.World);
            Size = new(Size.y, Size.x, 1);
            Initialize();
        }
    }
}