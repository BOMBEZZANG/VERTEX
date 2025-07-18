using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VERTEX.Core;
using VERTEX.Systems;
using VERTEX.Visualization;
using VERTEX.Crafting;

namespace VERTEX.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Resource Display")]
        [SerializeField] private TextMeshProUGUI woodCountText;
        [SerializeField] private TextMeshProUGUI stoneCountText;
        [SerializeField] private TextMeshProUGUI coalCountText;
        [SerializeField] private TextMeshProUGUI ironCountText;
        [SerializeField] private TextMeshProUGUI groundStabilityText;
        
        [Header("Construction Menu")]
        [SerializeField] private Button woodBlockButton;
        [SerializeField] private Button stoneBlockButton;
        [SerializeField] private Button steelBlockButton;
        [SerializeField] private Button woodPillarButton;
        [SerializeField] private Button stonePillarButton;
        
        [Header("Mode Display")]
        [SerializeField] private TextMeshProUGUI modeText;
        [SerializeField] private TextMeshProUGUI selectedMaterialText;
        
        [Header("Instructions")]
        [SerializeField] private TextMeshProUGUI instructionsText;
        
        [Header("Crafting Buttons")]
        [SerializeField] private Button craftSteelButton;
        [SerializeField] private Button craftWoodButton;
        
        private ResourceManager resourceManager;
        private ConstructionSystem constructionSystem;
        private PhysicsCalculationSystem physicsSystem;
        private StructuralVisualization structuralVisualization;
        private CraftingSystem craftingSystem;
        
        void Start()
        {
            resourceManager = FindObjectOfType<ResourceManager>();
            constructionSystem = FindObjectOfType<ConstructionSystem>();
            physicsSystem = FindObjectOfType<PhysicsCalculationSystem>();
            structuralVisualization = FindObjectOfType<StructuralVisualization>();
            craftingSystem = FindObjectOfType<CraftingSystem>();
            
            InitializeUI();
            SetupEventListeners();
        }
        
        private void InitializeUI()
        {
            // Set up instructions
            if (instructionsText != null)
            {
                instructionsText.text = "Controls:\n" +
                                      "B - Toggle Build Mode\n" +
                                      "1/2/3 - Select Material\n" +
                                      "Shift+Click - Dig\n" +
                                      "Click - Build/Select\n" +
                                      "Ctrl+Click - Multi-select\n" +
                                      "V - Toggle Structural View\n" +
                                      "\nCrafting:\n" +
                                      "Steel: 1 Coal + 2 Iron\n" +
                                      "Wood+: 2 Wood → 3 Wood";
            }
            
            UpdateUI();
        }
        
        private void SetupEventListeners()
        {
            if (resourceManager != null)
            {
                resourceManager.OnResourceChanged += UpdateResourceDisplay;
            }
            
            // Construction menu buttons
            if (woodBlockButton != null)
                woodBlockButton.onClick.AddListener(() => constructionSystem.SetSelectedMaterial(MaterialType.Wood));
            if (stoneBlockButton != null)
                stoneBlockButton.onClick.AddListener(() => constructionSystem.SetSelectedMaterial(MaterialType.Stone));
            if (steelBlockButton != null)
                steelBlockButton.onClick.AddListener(() => constructionSystem.SetSelectedMaterial(MaterialType.Steel));
            
            if (woodPillarButton != null)
                woodPillarButton.onClick.AddListener(() => BuildPillar(MaterialType.Wood));
            if (stonePillarButton != null)
                stonePillarButton.onClick.AddListener(() => BuildPillar(MaterialType.Stone));
            
            // Crafting buttons
            if (craftSteelButton != null)
                craftSteelButton.onClick.AddListener(() => CraftItem("steel"));
            if (craftWoodButton != null)
                craftWoodButton.onClick.AddListener(() => CraftItem("wood_planks"));
        }
        
        void Update()
        {
            UpdateUI();
            HandleInput();
        }
        
        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                if (structuralVisualization != null)
                {
                    structuralVisualization.ToggleStructuralView();
                }
            }
        }
        
        private void UpdateUI()
        {
            UpdateModeDisplay();
            UpdateGroundStability();
            UpdateConstructionButtons();
        }
        
        private void UpdateResourceDisplay(MaterialType materialType, int amount)
        {
            switch (materialType)
            {
                case MaterialType.Wood:
                    if (woodCountText != null)
                        woodCountText.text = $"Wood: {amount}";
                    else
                        Debug.LogWarning("Wood Count Text is not assigned in UIManager");
                    break;
                case MaterialType.Stone:
                    if (stoneCountText != null)
                        stoneCountText.text = $"Stone: {amount}";
                    else
                        Debug.LogWarning("Stone Count Text is not assigned in UIManager");
                    break;
                case MaterialType.Coal:
                    if (coalCountText != null)
                        coalCountText.text = $"Coal: {amount}";
                    else
                        Debug.LogWarning("Coal Count Text is not assigned in UIManager");
                    break;
                case MaterialType.Iron:
                    if (ironCountText != null)
                        ironCountText.text = $"Iron: {amount}";
                    else
                        Debug.LogWarning("Iron Count Text is not assigned in UIManager");
                    break;
            }
        }
        
        private void UpdateModeDisplay()
        {
            if (modeText != null && constructionSystem != null)
            {
                modeText.text = constructionSystem.IsConstructionMode ? "BUILD MODE" : "UNIT MODE";
            }
            
            if (selectedMaterialText != null && constructionSystem != null)
            {
                selectedMaterialText.text = $"Material: {constructionSystem.SelectedMaterial}";
            }
        }
        
        private void UpdateGroundStability()
        {
            if (groundStabilityText != null && physicsSystem != null)
            {
                float stability = physicsSystem.GetGroundStabilityPercentage();
                groundStabilityText.text = $"Ground Stability: {stability:F1}%";
                
                // Color coding
                if (stability < 25f)
                    groundStabilityText.color = Color.red;
                else if (stability < 50f)
                    groundStabilityText.color = Color.yellow;
                else
                    groundStabilityText.color = Color.green;
            }
        }
        
        private void UpdateConstructionButtons()
        {
            if (constructionSystem == null)
            {
                Debug.LogWarning("ConstructionSystem is null in UpdateConstructionButtons");
                return;
            }
            
            if (resourceManager == null)
            {
                Debug.LogWarning("ResourceManager is null in UpdateConstructionButtons");
                return;
            }
            
            // Update button interactability based on available resources
            if (woodBlockButton != null)
                woodBlockButton.interactable = resourceManager.HasResource(MaterialType.Wood);
            if (stoneBlockButton != null)
                stoneBlockButton.interactable = resourceManager.HasResource(MaterialType.Stone);
            if (steelBlockButton != null)
                steelBlockButton.interactable = resourceManager.HasResource(MaterialType.Steel);
            
            if (woodPillarButton != null)
                woodPillarButton.interactable = constructionSystem.CanBuildPillar(MaterialType.Wood);
            if (stonePillarButton != null)
                stonePillarButton.interactable = constructionSystem.CanBuildPillar(MaterialType.Stone);
            
            // Update crafting buttons
            if (craftSteelButton != null && craftingSystem != null)
                craftSteelButton.interactable = craftingSystem.CanCraft("steel");
            if (craftWoodButton != null && craftingSystem != null)
                craftWoodButton.interactable = craftingSystem.CanCraft("wood_planks");
        }
        
        private void BuildPillar(MaterialType materialType)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            WorldGrid worldGrid = FindObjectOfType<WorldGrid>();
            
            if (worldGrid != null)
            {
                Vector2Int gridPos = worldGrid.WorldToGridPosition(mouseWorldPos);
                constructionSystem.BuildPillar(gridPos, materialType);
            }
        }
        
        void OnDestroy()
        {
            if (resourceManager != null)
            {
                resourceManager.OnResourceChanged -= UpdateResourceDisplay;
            }
        }
        
        private void CraftItem(string recipeId)
        {
            if (craftingSystem != null)
            {
                craftingSystem.StartCrafting(recipeId);
                Debug.Log($"Attempting to craft: {recipeId}");
            }
            else
            {
                Debug.LogError("CraftingSystem not found!");
            }
        }
    }
}