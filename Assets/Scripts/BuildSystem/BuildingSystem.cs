using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BuildSystem
{
    public class BuildingSystem : MonoBehaviour
    {
        public static BuildingSystem Instance;

        public GridLayout GridLayout => gridLayout;

        [SerializeField]
        private GridLayout gridLayout;

        [SerializeField]
        private Tilemap tilemap;

        [SerializeField]
        private TileBase occupiedTile;

        [SerializeField]
        private GameObject prefab1;

        private Grid grid;
        private PlaceableObject objectToPlace;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (gridLayout == null)
            {
                Debug.LogError("Grid Layout is null!");
                return;
            }

            if (!gridLayout.TryGetComponent(out grid))
            {
                Debug.LogError("Grid is null!");
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                InitializeWithObject(prefab1);
            }

            if (objectToPlace == null)
            {
                return;
            }

            if (!Input.GetKeyDown(KeyCode.E))
                return;

            if (!CanBePlaced(objectToPlace))
                return;
            
            objectToPlace.Place();
            Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
            FillArea(start, objectToPlace.Size);

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Destroy(objectToPlace);
            }
        }

        public Vector3 GetGridSnapPosition(Vector3 position)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(position);
            position = grid.GetCellCenterWorld(cellPosition);
            return position;
        }

        public void InitializeWithObject(GameObject prefab)
        {
            if (prefab == null)
            {
                return;
            }

            Vector3 position = GetGridSnapPosition(Vector3.zero);
            GameObject obj = Instantiate(prefab, position, Quaternion.identity);
            if (!obj.TryGetComponent(out objectToPlace))
            {
                Debug.LogError("Prefab is missing PlaceableObject component!");
            }

            obj.AddComponent<ObjectDrag>();
        }

        private bool CanBePlaced(PlaceableObject placeableObject)
        {
            BoundsInt area = new()
            {
                position = gridLayout.WorldToCell(objectToPlace.GetStartPosition()),
                size = placeableObject.Size
            };

            TileBase[] baseArray = tilemap.GetTilesBlock(area);
            foreach (TileBase tile in baseArray)
            {
                if (tile != null)
                {
                    return false;
                }
            }

            return true;
        }

        public void FillArea(Vector3Int start, Vector3Int size)
        {
            tilemap.BoxFill(start, occupiedTile, start.x, start.y, start.x + size.x, start.y + size.y);
        }
    }
}