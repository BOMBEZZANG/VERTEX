using UnityEngine;
using System.Collections.Generic;
using VERTEX.Core;
using VERTEX.Systems;

namespace VERTEX.Visualization
{
    public class StructuralVisualization : MonoBehaviour
    {
        [Header("Visualization Settings")]
        [SerializeField] private bool showStructuralView = false;
        [SerializeField] private GameObject loadIndicatorPrefab;
        
        [Header("Load Status Colors")]
        [SerializeField] private Color safeColor = Color.green;
        [SerializeField] private Color moderateColor = Color.yellow;
        [SerializeField] private Color stressedColor = new Color(1f, 0.5f, 0f); // Orange
        [SerializeField] private Color criticalColor = Color.red;
        
        private WorldGrid worldGrid;
        private Dictionary<Vector2Int, GameObject> loadIndicators = new Dictionary<Vector2Int, GameObject>();
        
        void Start()
        {
            worldGrid = FindObjectOfType<WorldGrid>();
            CreateLoadIndicatorPrefab();
        }
        
        private void CreateLoadIndicatorPrefab()
        {
            if (loadIndicatorPrefab == null)
            {
                // Create a simple quad prefab for load indicators
                loadIndicatorPrefab = new GameObject("LoadIndicator");
                loadIndicatorPrefab.AddComponent<MeshRenderer>();
                loadIndicatorPrefab.AddComponent<MeshFilter>();
                
                // Create a simple quad mesh
                Mesh quadMesh = new Mesh();
                quadMesh.vertices = new Vector3[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(0, 1, 0)
                };
                quadMesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
                quadMesh.uv = new Vector2[]
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1)
                };
                
                loadIndicatorPrefab.GetComponent<MeshFilter>().mesh = quadMesh;
                
                // Create material
                Material indicatorMaterial = new Material(Shader.Find("Sprites/Default"));
                indicatorMaterial.color = Color.white;
                loadIndicatorPrefab.GetComponent<MeshRenderer>().material = indicatorMaterial;
                
                loadIndicatorPrefab.SetActive(false);
            }
        }
        
        void Update()
        {
            if (showStructuralView)
            {
                UpdateStructuralVisualization();
            }
        }
        
        public void ToggleStructuralView()
        {
            showStructuralView = !showStructuralView;
            
            if (showStructuralView)
            {
                ShowStructuralView();
            }
            else
            {
                HideStructuralView();
            }
            
            Debug.Log($"Structural view: {(showStructuralView ? "ON" : "OFF")}");
        }
        
        private void ShowStructuralView()
        {
            UpdateStructuralVisualization();
        }
        
        private void HideStructuralView()
        {
            foreach (var indicator in loadIndicators.Values)
            {
                if (indicator != null)
                {
                    indicator.SetActive(false);
                }
            }
        }
        
        private void UpdateStructuralVisualization()
        {
            if (worldGrid == null) return;
            
            foreach (Tile tile in worldGrid.GetAllTiles())
            {
                if (tile.IsStructural())
                {
                    UpdateLoadIndicator(tile);
                }
            }
        }
        
        private void UpdateLoadIndicator(Tile tile)
        {
            if (!loadIndicators.ContainsKey(tile.position))
            {
                CreateLoadIndicator(tile.position);
            }
            
            GameObject indicator = loadIndicators[tile.position];
            if (indicator == null) return;
            
            indicator.SetActive(showStructuralView);
            
            if (showStructuralView)
            {
                // Update color based on load status
                LoadStatus status = tile.GetLoadStatus();
                Color indicatorColor = GetColorForLoadStatus(status);
                
                MeshRenderer renderer = indicator.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material.color = indicatorColor;
                }
                
                // Update transparency based on load ratio
                float alpha = 0.7f; // Base transparency
                if (tile.supportValue > 0)
                {
                    float loadRatio = tile.currentLoad / tile.supportValue;
                    alpha = Mathf.Lerp(0.3f, 0.9f, loadRatio);
                }
                
                Color finalColor = indicatorColor;
                finalColor.a = alpha;
                renderer.material.color = finalColor;
            }
        }
        
        private void CreateLoadIndicator(Vector2Int position)
        {
            if (loadIndicatorPrefab == null) return;
            
            Vector3 worldPosition = worldGrid.GridToWorldPosition(position);
            GameObject indicator = Instantiate(loadIndicatorPrefab, worldPosition, Quaternion.identity);
            indicator.transform.SetParent(transform);
            indicator.SetActive(false);
            
            loadIndicators[position] = indicator;
        }
        
        private Color GetColorForLoadStatus(LoadStatus status)
        {
            switch (status)
            {
                case LoadStatus.Safe:
                    return safeColor;
                case LoadStatus.Moderate:
                    return moderateColor;
                case LoadStatus.Stressed:
                    return stressedColor;
                case LoadStatus.Critical:
                    return criticalColor;
                default:
                    return Color.white;
            }
        }
        
        public void SetVisualizationColors(Color safe, Color moderate, Color stressed, Color critical)
        {
            safeColor = safe;
            moderateColor = moderate;
            stressedColor = stressed;
            criticalColor = critical;
        }
        
        void OnDestroy()
        {
            foreach (var indicator in loadIndicators.Values)
            {
                if (indicator != null)
                {
                    Destroy(indicator);
                }
            }
            loadIndicators.Clear();
        }
    }
}