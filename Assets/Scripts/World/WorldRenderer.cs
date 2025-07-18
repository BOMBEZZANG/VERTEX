using UnityEngine;
using System.Collections.Generic;
using VERTEX.Core;
using VERTEX.Systems;

namespace VERTEX.World
{
    public class WorldRenderer : MonoBehaviour
    {
        [Header("Rendering Settings")]
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Material woodMaterial;
        [SerializeField] private Material stoneMaterial;
        [SerializeField] private Material steelMaterial;
        [SerializeField] private Material dirtMaterial;
        [SerializeField] private Material coalMaterial;
        [SerializeField] private Material ironMaterial;
        
        private WorldGrid worldGrid;
        private Dictionary<Vector2Int, GameObject> renderedTiles = new Dictionary<Vector2Int, GameObject>();
        private Dictionary<Vector2Int, Tile> lastRenderedState = new Dictionary<Vector2Int, Tile>();
        
        void Start()
        {
            worldGrid = FindObjectOfType<WorldGrid>();
            CreateTilePrefab();
            InitialRender();
        }
        
        void Update()
        {
            UpdateChangedTiles();
        }
        
        private void CreateTilePrefab()
        {
            if (tilePrefab == null)
            {
                // Create a simple cube prefab for tiles
                tilePrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tilePrefab.name = "TilePrefab";
                
                // Remove the collider as we don't need it for rendering
                Destroy(tilePrefab.GetComponent<Collider>());
                
                tilePrefab.SetActive(false);
            }
        }
        
        private void InitialRender()
        {
            if (worldGrid == null) return;
            
            foreach (Tile tile in worldGrid.GetAllTiles())
            {
                if (tile.IsStructural())
                {
                    RenderTile(tile);
                }
            }
        }
        
        private void UpdateChangedTiles()
        {
            if (worldGrid == null) return;
            
            foreach (Tile tile in worldGrid.GetAllTiles())
            {
                bool needsUpdate = false;
                
                if (!lastRenderedState.ContainsKey(tile.position))
                {
                    needsUpdate = true;
                }
                else
                {
                    Tile lastState = lastRenderedState[tile.position];
                    if (lastState.materialType != tile.materialType)
                    {
                        needsUpdate = true;
                    }
                }
                
                if (needsUpdate)
                {
                    UpdateTileRendering(tile);
                    lastRenderedState[tile.position] = new Tile(tile.materialType, tile.position);
                }
            }
        }
        
        private void UpdateTileRendering(Tile tile)
        {
            if (tile.IsStructural())
            {
                RenderTile(tile);
            }
            else
            {
                RemoveTileRendering(tile.position);
            }
        }
        
        private void RenderTile(Tile tile)
        {
            if (renderedTiles.ContainsKey(tile.position))
            {
                // Update existing tile
                UpdateTileAppearance(renderedTiles[tile.position], tile);
            }
            else
            {
                // Create new tile
                CreateTileObject(tile);
            }
        }
        
        private void CreateTileObject(Tile tile)
        {
            Vector3 worldPosition = worldGrid.GridToWorldPosition(tile.position);
            GameObject tileObj = Instantiate(tilePrefab, worldPosition, Quaternion.identity);
            tileObj.SetActive(true);
            tileObj.transform.SetParent(transform);
            tileObj.name = $"Tile_{tile.position.x}_{tile.position.y}";
            
            UpdateTileAppearance(tileObj, tile);
            renderedTiles[tile.position] = tileObj;
        }
        
        private void UpdateTileAppearance(GameObject tileObj, Tile tile)
        {
            MeshRenderer renderer = tileObj.GetComponent<MeshRenderer>();
            if (renderer == null) return;
            
            Material material = GetMaterialForTile(tile.materialType);
            if (material != null)
            {
                renderer.material = material;
            }
            else
            {
                // Fallback to color-based rendering
                renderer.material.color = GetColorForMaterial(tile.materialType);
            }
        }
        
        private Material GetMaterialForTile(MaterialType materialType)
        {
            switch (materialType)
            {
                case MaterialType.Wood:
                    return woodMaterial;
                case MaterialType.Stone:
                    return stoneMaterial;
                case MaterialType.Steel:
                    return steelMaterial;
                case MaterialType.Dirt:
                    return dirtMaterial;
                case MaterialType.Coal:
                    return coalMaterial;
                case MaterialType.Iron:
                    return ironMaterial;
                default:
                    return null;
            }
        }
        
        private Color GetColorForMaterial(MaterialType materialType)
        {
            switch (materialType)
            {
                case MaterialType.Wood:
                    return new Color(0.6f, 0.4f, 0.2f); // Brown
                case MaterialType.Stone:
                    return Color.gray;
                case MaterialType.Steel:
                    return new Color(0.8f, 0.8f, 0.9f); // Light gray
                case MaterialType.Dirt:
                    return new Color(0.4f, 0.3f, 0.2f); // Dark brown
                case MaterialType.Coal:
                    return new Color(0.2f, 0.2f, 0.2f); // Very dark gray
                case MaterialType.Iron:
                    return new Color(0.6f, 0.4f, 0.3f); // Rust color
                default:
                    return Color.white;
            }
        }
        
        private void RemoveTileRendering(Vector2Int position)
        {
            if (renderedTiles.ContainsKey(position))
            {
                GameObject tileObj = renderedTiles[position];
                if (tileObj != null)
                {
                    Destroy(tileObj);
                }
                renderedTiles.Remove(position);
            }
        }
        
        public void SetTileMaterial(MaterialType materialType, Material material)
        {
            switch (materialType)
            {
                case MaterialType.Wood:
                    woodMaterial = material;
                    break;
                case MaterialType.Stone:
                    stoneMaterial = material;
                    break;
                case MaterialType.Steel:
                    steelMaterial = material;
                    break;
                case MaterialType.Dirt:
                    dirtMaterial = material;
                    break;
                case MaterialType.Coal:
                    coalMaterial = material;
                    break;
                case MaterialType.Iron:
                    ironMaterial = material;
                    break;
            }
        }
        
        void OnDestroy()
        {
            foreach (var tileObj in renderedTiles.Values)
            {
                if (tileObj != null)
                {
                    Destroy(tileObj);
                }
            }
            renderedTiles.Clear();
        }
    }
}