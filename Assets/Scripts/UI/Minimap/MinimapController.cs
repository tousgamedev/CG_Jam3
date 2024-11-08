using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public enum MinimapMode
    {
        Mini,
        Fullscreen
    }

    public class MinimapController : MonoBehaviour
    {
        public static MinimapController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<MinimapController>(FindObjectsInactive.Include);
                }

                return instance;
            }
        }

        private static MinimapController instance;

        [SerializeField] private List<Vector2> worldSize = new()
        {
            new(300, 300),
            new(600, 600),
            new(1300, 1300)
        };

        [SerializeField] private List<float> updateDelays = new()
        {
            2f,
            1f,
            0
        };

        [SerializeField] private Vector2 fullScreenDimensions = new(1000, 1000);
        [SerializeField] private float zoomSpeed = 0.1f;
        [SerializeField] private float maxZoom = 10f;
        [SerializeField] private float minZoom = 1f;
        [SerializeField] private RectTransform scrollViewRectTransform;
        [SerializeField] private RectTransform contentRectTransform;
        [SerializeField] private MinimapIcon minimapIconPrefab;

        Matrix4x4 transformationMatrix;

        private MinimapMode currentMiniMapMode = MinimapMode.Mini;
        private MinimapIcon followIcon;
        private Vector2 scrollViewDefaultSize;
        private Vector2 scrollViewDefaultPosition;
        private Vector2 halfVector2 = new(0.5f, 0.5f);
        private readonly Dictionary<MinimapWorldObject, MinimapIcon> miniMapWorldObjectsLookup = new();

        private int sizeIndex;
        private int updateIndex;
        private float updateTimer;

        private void Awake()
        {
            scrollViewDefaultSize = scrollViewRectTransform.sizeDelta;
            scrollViewDefaultPosition = scrollViewRectTransform.anchoredPosition;
        }

        private void Start()
        {
            CalculateTransformationMatrix();
        }

        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.M))
            // {
            //     SetMinimapMode(currentMiniMapMode == MinimapMode.Mini ? MinimapMode.Fullscreen : MinimapMode.Mini);
            // }

            //float zoom = Input.GetAxis("Mouse ScrollWheel");
            //ZoomMap(zoom);
            updateTimer += Time.deltaTime;
            if (updateTimer >= updateDelays[updateIndex])
            {
                UpdateMiniMapIcons();
                updateTimer = 0;
            }

            CenterMapOnIcon();
        }

        public void RegisterMinimapWorldObject(MinimapWorldObject miniMapWorldObject, bool followObject = false)
        {
            MinimapIcon minimapIcon = Instantiate(minimapIconPrefab);
            minimapIcon.transform.SetParent(contentRectTransform);
            minimapIcon.transform.SetParent(contentRectTransform);
            minimapIcon.Image.sprite = miniMapWorldObject.MinimapIcon;
            miniMapWorldObjectsLookup[miniMapWorldObject] = minimapIcon;

            if (followObject)
            {
                followIcon = minimapIcon;
            }
        }

        public void RemoveMinimapWorldObject(MinimapWorldObject minimapWorldObject)
        {
            if (!miniMapWorldObjectsLookup.Remove(minimapWorldObject, out MinimapIcon icon))
            {
                return;
            }

            if (icon == null || icon.gameObject == null)
            {
                return;
            }

            Destroy(icon.gameObject);
        }

        public void SetMinimapMode(MinimapMode mode)
        {
            const float defaultScaleWhenFullScreen = 1.3f; // 1.3f looks good here but it could be anything

            if (mode == currentMiniMapMode)
            {
                return;
            }

            switch (mode)
            {
                case MinimapMode.Mini:
                    scrollViewRectTransform.sizeDelta = scrollViewDefaultSize;
                    scrollViewRectTransform.anchorMin = Vector2.one;
                    scrollViewRectTransform.anchorMax = Vector2.one;
                    scrollViewRectTransform.pivot = Vector2.one;
                    scrollViewRectTransform.anchoredPosition = scrollViewDefaultPosition;
                    currentMiniMapMode = MinimapMode.Mini;
                    break;
                case MinimapMode.Fullscreen:
                    scrollViewRectTransform.sizeDelta = fullScreenDimensions;
                    scrollViewRectTransform.anchorMin = halfVector2;
                    scrollViewRectTransform.anchorMax = halfVector2;
                    scrollViewRectTransform.pivot = halfVector2;
                    scrollViewRectTransform.anchoredPosition = Vector2.zero;
                    currentMiniMapMode = MinimapMode.Fullscreen;
                    contentRectTransform.transform.localScale = Vector3.one * defaultScaleWhenFullScreen;
                    break;
            }
        }

        private void ZoomMap(float zoom)
        {
            if (zoom == 0)
            {
                return;
            }

            float currentMapScale = contentRectTransform.localScale.x;
            // we need to scale the zoom speed by the current map scale to keep the zooming linear
            float zoomAmount = (zoom > 0 ? zoomSpeed : -zoomSpeed) * currentMapScale;
            float newScale = currentMapScale + zoomAmount;
            float clampedScale = Mathf.Clamp(newScale, minZoom, maxZoom);
            contentRectTransform.localScale = Vector3.one * clampedScale;
        }

        private void CenterMapOnIcon()
        {
            if (followIcon != null)
            {
                float mapScale = contentRectTransform.transform.localScale.x;
                // we simply move the map in the opposite direction the player moved, scaled by the mapscale
                contentRectTransform.anchoredPosition = (-followIcon.RectTransform.anchoredPosition * mapScale);
            }
        }

        private void UpdateMiniMapIcons()
        {
            // scale icons by the inverse of the mapscale to keep them a consitent size
            float iconScale = 1 / contentRectTransform.transform.localScale.x;
            foreach (var kvp in miniMapWorldObjectsLookup)
            {
                var miniMapWorldObject = kvp.Key;
                var miniMapIcon = kvp.Value;
                var mapPosition = WorldPositionToMapPosition(miniMapWorldObject.transform.position);

                miniMapIcon.RectTransform.anchoredPosition = mapPosition;
                var rotation = miniMapWorldObject.transform.rotation.eulerAngles;
                miniMapIcon.IconRectTransform.localRotation = Quaternion.AngleAxis(-rotation.y, Vector3.forward);
                miniMapIcon.IconRectTransform.localScale = Vector3.one * iconScale;
            }
        }

        private Vector2 WorldPositionToMapPosition(Vector3 worldPos)
        {
            var pos = new Vector2(worldPos.x, worldPos.z);
            return transformationMatrix.MultiplyPoint3x4(pos);
        }

        private void CalculateTransformationMatrix()
        {
            var minimapSize = contentRectTransform.rect.size;
            var worldSize = new Vector2(this.worldSize[sizeIndex].x, this.worldSize[sizeIndex].y);

            var translation = -minimapSize / 2;
            var scaleRatio = minimapSize / worldSize;

            transformationMatrix = Matrix4x4.TRS(translation, Quaternion.identity, scaleRatio);

            //  {scaleRatio.x,   0,           0,   translation.x},
            //  {  0,        scaleRatio.y,    0,   translation.y},
            //  {  0,            0,           1,            0},
            //  {  0,            0,           0,            1}
        }

        public void UpgradeRange()
        {
            sizeIndex = Mathf.Clamp(sizeIndex + 1, 0, worldSize.Count - 1);
            Debug.LogError(sizeIndex);
            CalculateTransformationMatrix();
        }

        public void UpgradeScanRate()
        {
            updateIndex = Mathf.Clamp(updateIndex + 1, 0, updateDelays.Count - 1);
            Debug.LogError(updateIndex);
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}