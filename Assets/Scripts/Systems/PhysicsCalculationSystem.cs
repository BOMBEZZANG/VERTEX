using System.Collections.Generic;
using UnityEngine;
using VERTEX.Core;

namespace VERTEX.Systems
{
    public class PhysicsCalculationSystem : MonoBehaviour
    {
        [Header("Ground Stability Settings")]
        [SerializeField] private float supportPerSurfaceTile = 1000f;
        
        private WorldGrid worldGrid;
        private List<Vector2Int> pendingUpdates = new List<Vector2Int>();
        
        public void Initialize(WorldGrid grid)
        {
            worldGrid = grid;
        }
        
        public void TriggerStructureUpdate(Vector2Int position)
        {
            if (!pendingUpdates.Contains(position))
            {
                pendingUpdates.Add(position);
            }
        }

        [System.Obsolete]

        void Update()
        {
            if (pendingUpdates.Count > 0)
            {
                ProcessPendingUpdates();
                pendingUpdates.Clear();
            }
        }

        [System.Obsolete]

        private void ProcessPendingUpdates()
        {
            HashSet<Vector2Int> affectedTiles = new HashSet<Vector2Int>();
            
            foreach (Vector2Int position in pendingUpdates)
            {
                IdentifyAffectedArea(position, affectedTiles);
            }
            
            ResetLoads(affectedTiles);
            RecalculateLoads(affectedTiles);
            CheckForCollapses(affectedTiles);
        }
        
        private void IdentifyAffectedArea(Vector2Int changedPosition, HashSet<Vector2Int> affectedTiles)
        {
            int maxHeight = worldGrid.MaxHeight;
            
            for (int y = changedPosition.y; y <= maxHeight; y++)
            {
                Vector2Int checkPos = new Vector2Int(changedPosition.x, y);
                Tile tile = worldGrid.GetTile(checkPos);
                
                if (tile != null && tile.IsStructural())
                {
                    affectedTiles.Add(checkPos);
                }
            }
        }
        
        private void ResetLoads(HashSet<Vector2Int> affectedTiles)
        {
            foreach (Vector2Int pos in affectedTiles)
            {
                Tile tile = worldGrid.GetTile(pos);
                if (tile != null)
                {
                    tile.currentLoad = 0f;
                }
            }
        }
        
        private void RecalculateLoads(HashSet<Vector2Int> affectedTiles)
        {
            var sortedTiles = new List<Vector2Int>(affectedTiles);
            sortedTiles.Sort((a, b) => b.y.CompareTo(a.y)); // Sort by Y descending (top to bottom)
            
            foreach (Vector2Int pos in sortedTiles)
            {
                Tile tile = worldGrid.GetTile(pos);
                if (tile == null || !tile.IsStructural()) continue;
                
                // Add own mass to current load
                tile.currentLoad += tile.mass;
                
                // Transfer load to tile below
                Vector2Int belowPos = new Vector2Int(pos.x, pos.y - 1);
                Tile belowTile = worldGrid.GetTile(belowPos);
                
                if (belowTile != null && belowTile.IsStructural())
                {
                    belowTile.currentLoad += tile.currentLoad;
                    tile.isSupported = true;
                }
                else
                {
                    tile.isSupported = false;
                }
            }
        }

        [System.Obsolete]

        private void CheckForCollapses(HashSet<Vector2Int> affectedTiles)
        {
            List<Vector2Int> collapsedTiles = new List<Vector2Int>();
            
            // Check individual tile collapses
            foreach (Vector2Int pos in affectedTiles)
            {
                Tile tile = worldGrid.GetTile(pos);
                if (tile != null && tile.WillCollapse())
                {
                    collapsedTiles.Add(pos);
                }
            }
            
            // Check ground stability
            CheckGroundStability(collapsedTiles);
            
            // Process collapses
            foreach (Vector2Int pos in collapsedTiles)
            {
                CollapseTile(pos);
            }
        }
        
        private void CheckGroundStability(List<Vector2Int> collapsedTiles)
        {
            float totalGroundLoad = 0f;
            int foundationCount = 0;
            
            foreach (Tile tile in worldGrid.GetAllTiles())
            {
                if (tile.isFoundation)
                {
                    totalGroundLoad += tile.currentLoad;
                    foundationCount++;
                }
            }
            
            float maxGroundSupport = foundationCount * supportPerSurfaceTile;
            
            if (totalGroundLoad > maxGroundSupport)
            {
                // Trigger sinkhole event - collapse all foundation blocks
                foreach (Tile tile in worldGrid.GetAllTiles())
                {
                    if (tile.isFoundation && !collapsedTiles.Contains(tile.position))
                    {
                        collapsedTiles.Add(tile.position);
                    }
                }
                
                Debug.Log($"SINKHOLE EVENT! Ground load ({totalGroundLoad}) exceeds support ({maxGroundSupport})");
            }
        }

        [System.Obsolete]

        private void CollapseTile(Vector2Int position)
        {
            Tile tile = worldGrid.GetTile(position);
            if (tile == null) return;
            
            Debug.Log($"Tile collapsed at {position}: Load {tile.currentLoad} > Support {tile.supportValue}");
            
            // Convert to item drop or falling block
            CreateItemDrop(position, tile.materialType);
            
            // Remove tile from world
            worldGrid.SetTile(position, new Tile(MaterialType.Air, position));
            
            // Trigger another structure update due to removal
            TriggerStructureUpdate(position);
        }

        [System.Obsolete]

        private void CreateItemDrop(Vector2Int position, MaterialType materialType)
        {
            // This would create a dropped item or falling block
            // For now, just add to resource inventory
            var resourceManager = FindObjectOfType<ResourceManager>();
            if (resourceManager != null)
            {
                resourceManager.AddResource(materialType, 1);
            }
        }
        
        public float GetGroundStabilityPercentage()
        {
            float totalGroundLoad = 0f;
            int foundationCount = 0;
            
            foreach (Tile tile in worldGrid.GetAllTiles())
            {
                if (tile.isFoundation)
                {
                    totalGroundLoad += tile.currentLoad;
                    foundationCount++;
                }
            }
            
            float maxGroundSupport = foundationCount * supportPerSurfaceTile;
            
            if (maxGroundSupport == 0) return 100f;
            return Mathf.Clamp01(1f - (totalGroundLoad / maxGroundSupport)) * 100f;
        }
    }
}