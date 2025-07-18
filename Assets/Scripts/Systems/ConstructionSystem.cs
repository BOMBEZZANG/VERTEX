using UnityEngine;
using VERTEX.Core;
using VERTEX.Units;

namespace VERTEX.Systems
{
    public class ConstructionSystem : MonoBehaviour
    {
        [Header("Construction Settings")]
        [SerializeField] private LayerMask groundLayer = 1;
        
        private WorldGrid worldGrid;
        private ResourceManager resourceManager;
        private UnitManager unitManager;
        private PhysicsCalculationSystem physicsSystem;
        
        private MaterialType selectedMaterial = MaterialType.Wood;
        private bool isConstructionMode = false;
        
        public MaterialType SelectedMaterial => selectedMaterial;
        public bool IsConstructionMode => isConstructionMode;

        [System.Obsolete]

        void Start()
        {
            worldGrid = FindObjectOfType<WorldGrid>();
            resourceManager = FindObjectOfType<ResourceManager>();
            unitManager = FindObjectOfType<UnitManager>();
            physicsSystem = FindObjectOfType<PhysicsCalculationSystem>();
            
            // Debug missing components
            if (worldGrid == null) Debug.LogError("WorldGrid not found!");
            if (resourceManager == null) Debug.LogError("ResourceManager not found!");
            if (unitManager == null) Debug.LogError("UnitManager not found!");
            if (physicsSystem == null) Debug.LogError("PhysicsCalculationSystem not found!");
        }
        
        void Update()
        {
            HandleInput();
        }
        
        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                ToggleConstructionMode();
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetSelectedMaterial(MaterialType.Wood);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetSelectedMaterial(MaterialType.Stone);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetSelectedMaterial(MaterialType.Steel);
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseClick();
            }
        }
        
        private void HandleMouseClick()
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPos = worldGrid.WorldToGridPosition(mouseWorldPos);
            
            if (isConstructionMode)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    // Dig mode
                    CommandDig(gridPos);
                }
                else
                {
                    // Build mode
                    CommandBuild(gridPos);
                }
            }
            else
            {
                // Unit selection mode
                HandleUnitSelection(mouseWorldPos);
            }
        }
        
        private void HandleUnitSelection(Vector3 worldPosition)
        {
            Unit clickedUnit = GetUnitAtPosition(worldPosition);
            
            if (clickedUnit != null)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    // Multi-select
                    if (clickedUnit.IsSelected)
                    {
                        unitManager.DeselectUnit(clickedUnit);
                    }
                    else
                    {
                        unitManager.SelectUnit(clickedUnit);
                    }
                }
                else
                {
                    // Single select
                    unitManager.ClearSelection();
                    unitManager.SelectUnit(clickedUnit);
                }
            }
            else
            {
                // Move command
                if (unitManager.SelectedUnits.Count > 0)
                {
                    Vector2Int gridPos = worldGrid.WorldToGridPosition(worldPosition);
                    unitManager.CommandSelectedUnits(UnitCommand.Move, gridPos);
                }
            }
        }
        
        private Unit GetUnitAtPosition(Vector3 worldPosition)
        {
            Collider2D collider = Physics2D.OverlapPoint(worldPosition);
            if (collider != null)
            {
                return collider.GetComponent<Unit>();
            }
            return null;
        }
        
        private void CommandBuild(Vector2Int position)
        {
            if (!worldGrid.CanPlaceTile(position, selectedMaterial))
            {
                Debug.Log($"Cannot build {selectedMaterial} at {position}");
                return;
            }
            
            if (!resourceManager.HasResource(selectedMaterial))
            {
                Debug.Log($"Not enough {selectedMaterial} to build");
                return;
            }
            
            unitManager.CommandSelectedUnits(UnitCommand.Build, position, selectedMaterial);
        }
        
        private void CommandDig(Vector2Int position)
        {
            Tile tile = worldGrid.GetTile(position);
            if (tile == null || !tile.IsStructural())
            {
                Debug.Log($"Cannot dig at {position} - no structural tile");
                return;
            }
            
            unitManager.CommandSelectedUnits(UnitCommand.Dig, position);
        }
        
        public void ToggleConstructionMode()
        {
            isConstructionMode = !isConstructionMode;
            Debug.Log($"Construction mode: {(isConstructionMode ? "ON" : "OFF")}");
        }
        
        public void SetSelectedMaterial(MaterialType materialType)
        {
            selectedMaterial = materialType;
            Debug.Log($"Selected material: {materialType}");
        }
        
        public bool CanBuildWith(MaterialType materialType)
        {
            return resourceManager.HasResource(materialType) && 
                   worldGrid.CanPlaceTile(GetMouseGridPosition(), materialType);
        }
        
        private Vector2Int GetMouseGridPosition()
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return worldGrid.WorldToGridPosition(mouseWorldPos);
        }
        
        // Construction recipes for different structures
        public bool CanBuildPillar(MaterialType materialType)
        {
            if (resourceManager == null)
            {
                Debug.LogWarning("ResourceManager is null in CanBuildPillar");
                return false;
            }
            
            switch (materialType)
            {
                case MaterialType.Wood:
                    return resourceManager.HasResource(MaterialType.Wood, 3);
                case MaterialType.Stone:
                    return resourceManager.HasResource(MaterialType.Stone, 2);
                default:
                    return false;
            }
        }
        
        public void BuildPillar(Vector2Int position, MaterialType materialType)
        {
            if (!CanBuildPillar(materialType)) return;
            
            int height = 0;
            int resourceCost;

            switch (materialType)
            {
                case MaterialType.Wood:
                    height = 3;
                    resourceCost = 3;
                    break;
                case MaterialType.Stone:
                    height = 2;
                    resourceCost = 2;
                    break;
            }
            
            // Build pillar vertically
            for (int i = 0; i < height; i++)
            {
                Vector2Int pillarPos = position + Vector2Int.up * i;
                if (worldGrid.CanPlaceTile(pillarPos, materialType))
                {
                    unitManager.CommandSelectedUnits(UnitCommand.Build, pillarPos, materialType);
                }
            }
        }
    }
}