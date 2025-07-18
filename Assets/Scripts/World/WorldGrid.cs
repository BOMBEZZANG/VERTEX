using System.Collections.Generic;
using UnityEngine;
using VERTEX.Core;

namespace VERTEX.Systems
{
    public class WorldGrid : MonoBehaviour
    {
        [Header("World Settings")]
        [SerializeField] private int worldWidth = 50;
        [SerializeField] private int initialHeight = 100;
        [SerializeField] private int surfaceLevel = 50;
        
        private Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();
        private int maxHeight;
        private int minHeight;
        
        public int WorldWidth => worldWidth;
        public int SurfaceLevel => surfaceLevel;
        public int MaxHeight => maxHeight;
        public int MinHeight => minHeight;
        
        void Awake()
        {
            InitializeWorld();
        }
        
        private void InitializeWorld()
        {
            maxHeight = surfaceLevel + initialHeight / 2;
            minHeight = surfaceLevel - initialHeight / 2;
            
            // Generate initial world
            GenerateInitialTerrain();
        }
        
        private void GenerateInitialTerrain()
        {
            // Create surface layer
            for (int x = 0; x < worldWidth; x++)
            {
                Vector2Int pos = new Vector2Int(x, surfaceLevel);
                Tile surfaceTile = new Tile(MaterialType.Dirt, pos);
                surfaceTile.isFoundation = true;
                SetTile(pos, surfaceTile);
            }
            
            // Create shallow underground with resources
            for (int x = 0; x < worldWidth; x++)
            {
                for (int y = surfaceLevel - 1; y >= minHeight; y--)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    MaterialType materialType = GetUndergroundMaterial(pos);
                    SetTile(pos, new Tile(materialType, pos));
                }
            }
            
            // Fill air above surface
            for (int x = 0; x < worldWidth; x++)
            {
                for (int y = surfaceLevel + 1; y <= maxHeight; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    SetTile(pos, new Tile(MaterialType.Air, pos));
                }
            }
        }
        
        private MaterialType GetUndergroundMaterial(Vector2Int position)
        {
            // Simple resource generation
            float noiseValue = Mathf.PerlinNoise(position.x * 0.1f, position.y * 0.1f);
            
            if (noiseValue > 0.7f) return MaterialType.Stone;
            if (noiseValue > 0.5f) return MaterialType.Coal;
            if (noiseValue > 0.3f) return MaterialType.Iron;
            return MaterialType.Dirt;
        }
        
        public Tile GetTile(Vector2Int position)
        {
            tiles.TryGetValue(position, out Tile tile);
            return tile;
        }
        
        public void SetTile(Vector2Int position, Tile tile)
        {
            tiles[position] = tile;
            
            // Update world bounds if necessary
            if (position.y > maxHeight)
            {
                maxHeight = position.y;
            }
            if (position.y < minHeight)
            {
                minHeight = position.y;
            }
        }
        
        public bool IsValidPosition(Vector2Int position)
        {
            return position.x >= 0 && position.x < worldWidth;
        }
        
        public IEnumerable<Tile> GetAllTiles()
        {
            return tiles.Values;
        }
        
        public List<Vector2Int> GetNeighbors(Vector2Int position)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            
            Vector2Int[] directions = {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };
            
            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbor = position + dir;
                if (IsValidPosition(neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }
            
            return neighbors;
        }
        
        public bool CanPlaceTile(Vector2Int position, MaterialType materialType)
        {
            if (!IsValidPosition(position)) return false;
            
            Tile currentTile = GetTile(position);
            if (currentTile != null && currentTile.materialType != MaterialType.Air)
            {
                return false; // Position occupied
            }
            
            // Check if there's support below (for structural tiles)
            if (materialType != MaterialType.Air)
            {
                Vector2Int belowPos = position + Vector2Int.down;
                Tile belowTile = GetTile(belowPos);
                
                if (belowTile == null || !belowTile.IsStructural())
                {
                    return false; // No support below
                }
            }
            
            return true;
        }
        
        public Vector2Int WorldToGridPosition(Vector3 worldPosition)
        {
            return new Vector2Int(
                Mathf.FloorToInt(worldPosition.x),
                Mathf.FloorToInt(worldPosition.y)
            );
        }
        
        public Vector3 GridToWorldPosition(Vector2Int gridPosition)
        {
            return new Vector3(gridPosition.x + 0.5f, gridPosition.y + 0.5f, 0f);
        }
    }
}