using System.Collections;
using UnityEngine;
using VERTEX.Core;
using VERTEX.Systems;

namespace VERTEX.Units
{
    public class Unit : MonoBehaviour
    {
        [Header("Unit Settings")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float workTime = 1f;
        
        private bool isSelected = false;
        private bool isBusy = false;
        private WorldGrid worldGrid;
        private ResourceManager resourceManager;
        private PhysicsCalculationSystem physicsSystem;
        
        public bool IsSelected => isSelected;
        public bool IsBusy => isBusy;

        [System.Obsolete]

        void Start()
        {
            worldGrid = FindObjectOfType<WorldGrid>();
            resourceManager = FindObjectOfType<ResourceManager>();
            physicsSystem = FindObjectOfType<PhysicsCalculationSystem>();
        }
        
        public void SetSelected(bool selected)
        {
            isSelected = selected;
            
            // Visual feedback for selection
            GetComponent<SpriteRenderer>().color = selected ? Color.yellow : Color.white;
        }
        
        public void MoveTo(Vector2Int targetPosition)
        {
            if (isBusy) return;
            
            Vector3 targetWorldPos = worldGrid.GridToWorldPosition(targetPosition);
            StartCoroutine(MoveToPosition(targetWorldPos));
        }
        
        public void DigAt(Vector2Int position)
        {
            if (isBusy) return;
            
            Tile tile = worldGrid.GetTile(position);
            if (tile == null || !tile.IsStructural()) return;
            
            StartCoroutine(PerformDigAction(position));
        }
        
        public void BuildAt(Vector2Int position, MaterialType materialType)
        {
            if (isBusy) return;
            
            if (!worldGrid.CanPlaceTile(position, materialType)) return;
            if (!resourceManager.HasResource(materialType)) return;
            
            StartCoroutine(PerformBuildAction(position, materialType));
        }
        
        private IEnumerator MoveToPosition(Vector3 targetPosition)
        {
            isBusy = true;
            
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            
            transform.position = targetPosition;
            isBusy = false;
        }
        
        private IEnumerator PerformDigAction(Vector2Int position)
        {
            isBusy = true;
            
            // Move to adjacent position if not already there
            Vector2Int unitPos = worldGrid.WorldToGridPosition(transform.position);
            if (Vector2Int.Distance(unitPos, position) > 1.5f)
            {
                Vector2Int adjacentPos = FindAdjacentPosition(position);
                if (adjacentPos != Vector2Int.zero)
                {
                    yield return StartCoroutine(MoveToPosition(worldGrid.GridToWorldPosition(adjacentPos)));
                }
            }
            
            // Perform dig work
            yield return new WaitForSeconds(workTime);
            
            // Remove tile and add resource
            Tile tile = worldGrid.GetTile(position);
            if (tile != null && tile.IsStructural())
            {
                resourceManager.AddResource(tile.materialType, 1);
                worldGrid.SetTile(position, new Tile(MaterialType.Air, position));
                physicsSystem.TriggerStructureUpdate(position);
            }
            
            isBusy = false;
        }
        
        private IEnumerator PerformBuildAction(Vector2Int position, MaterialType materialType)
        {
            isBusy = true;
            
            // Move to adjacent position if not already there
            Vector2Int unitPos = worldGrid.WorldToGridPosition(transform.position);
            if (Vector2Int.Distance(unitPos, position) > 1.5f)
            {
                Vector2Int adjacentPos = FindAdjacentPosition(position);
                if (adjacentPos != Vector2Int.zero)
                {
                    yield return StartCoroutine(MoveToPosition(worldGrid.GridToWorldPosition(adjacentPos)));
                }
            }
            
            // Perform build work
            yield return new WaitForSeconds(workTime);
            
            // Place tile and consume resource
            if (resourceManager.ConsumeResource(materialType, 1))
            {
                worldGrid.SetTile(position, new Tile(materialType, position));
                physicsSystem.TriggerStructureUpdate(position);
            }
            
            isBusy = false;
        }
        
        private Vector2Int FindAdjacentPosition(Vector2Int targetPosition)
        {
            Vector2Int[] directions = {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };
            
            foreach (Vector2Int dir in directions)
            {
                Vector2Int adjacentPos = targetPosition + dir;
                Tile tile = worldGrid.GetTile(adjacentPos);
                
                if (tile != null && tile.materialType == MaterialType.Air)
                {
                    return adjacentPos;
                }
            }
            
            return Vector2Int.zero; // No valid adjacent position found
        }
    }
}