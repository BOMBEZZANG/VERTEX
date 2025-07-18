using System.Collections.Generic;
using UnityEngine;
using VERTEX.Core;
using VERTEX.Systems;

namespace VERTEX.Crafting
{
    public class CraftingSystem : MonoBehaviour
    {
        [Header("Crafting Settings")]
        [SerializeField] private float craftingTime = 2f;
        
        private ResourceManager resourceManager;
        private Dictionary<string, CraftingRecipe> recipes;
        private List<CraftingFacility> facilities;

        [System.Obsolete]

        void Awake()
        {
            facilities = new List<CraftingFacility>();
            InitializeRecipes();
        }
        
        void Start()
        {
            resourceManager = FindObjectOfType<ResourceManager>();
            if (resourceManager == null)
            {
                Debug.LogError("ResourceManager not found! Make sure it exists in the scene.");
            }
        }
        
        private void InitializeRecipes()
        {
            recipes = new Dictionary<string, CraftingRecipe>();
            
            // Steel recipe (Coal + Iron = Steel)
            recipes["steel"] = new CraftingRecipe(
                "Steel",
                new Dictionary<MaterialType, int>
                {
                    { MaterialType.Coal, 1 },
                    { MaterialType.Iron, 2 }
                },
                new Dictionary<MaterialType, int>
                {
                    { MaterialType.Steel, 1 }
                },
                craftingTime,
                FacilityType.Furnace
            );
            
            // Wood planks recipe (more efficient building material)
            recipes["wood_planks"] = new CraftingRecipe(
                "Wood Planks",
                new Dictionary<MaterialType, int>
                {
                    { MaterialType.Wood, 2 }
                },
                new Dictionary<MaterialType, int>
                {
                    { MaterialType.Wood, 3 } // More efficient
                },
                craftingTime * 0.5f,
                FacilityType.Workbench
            );
        }
        
        public void RegisterFacility(CraftingFacility facility)
        {
            if (facilities == null)
            {
                Debug.LogError("Facilities list is null! CraftingSystem may not be properly initialized.");
                return;
            }
            
            if (facility == null)
            {
                Debug.LogError("Attempting to register null facility!");
                return;
            }
            
            if (!facilities.Contains(facility))
            {
                facilities.Add(facility);
                Debug.Log($"Facility {facility.GetFacilityType()} registered successfully. Total facilities: {facilities.Count}");
            }
        }
        
        public void UnregisterFacility(CraftingFacility facility)
        {
            facilities.Remove(facility);
        }
        
        public bool CanCraft(string recipeId)
        {
            if (!recipes.ContainsKey(recipeId))
                return false;
            
            CraftingRecipe recipe = recipes[recipeId];
            
            // Check if we have required facility
            bool hasFacility = false;
            foreach (var facility in facilities)
            {
                if (facility.GetFacilityType() == recipe.RequiredFacility && !facility.IsBusy())
                {
                    hasFacility = true;
                    break;
                }
            }
            
            if (!hasFacility)
                return false;
            
            // Check if we have required resources
            foreach (var ingredient in recipe.Ingredients)
            {
                if (!resourceManager.HasResource(ingredient.Key, ingredient.Value))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        public void StartCrafting(string recipeId)
        {
            if (!CanCraft(recipeId))
            {
                Debug.Log($"Cannot craft {recipeId}");
                return;
            }
            
            CraftingRecipe recipe = recipes[recipeId];
            
            // Find available facility
            CraftingFacility facility = null;
            foreach (var f in facilities)
            {
                if (f.GetFacilityType() == recipe.RequiredFacility && !f.IsBusy())
                {
                    facility = f;
                    break;
                }
            }
            
            if (facility == null)
            {
                Debug.Log($"No available {recipe.RequiredFacility} for crafting {recipeId}");
                return;
            }
            
            // Consume ingredients
            foreach (var ingredient in recipe.Ingredients)
            {
                resourceManager.ConsumeResource(ingredient.Key, ingredient.Value);
            }
            
            // Start crafting at facility
            facility.StartCrafting(recipe);
        }
        
        public void OnCraftingComplete(CraftingRecipe recipe)
        {
            // Add output resources
            foreach (var output in recipe.Outputs)
            {
                resourceManager.AddResource(output.Key, output.Value);
            }
            
            Debug.Log($"Crafting completed: {recipe.Name}");
        }
        
        public List<string> GetAvailableRecipes()
        {
            List<string> availableRecipes = new List<string>();
            
            foreach (var kvp in recipes)
            {
                if (CanCraft(kvp.Key))
                {
                    availableRecipes.Add(kvp.Key);
                }
            }
            
            return availableRecipes;
        }
        
        public CraftingRecipe GetRecipe(string recipeId)
        {
            return recipes.ContainsKey(recipeId) ? recipes[recipeId] : null;
        }
        
        public Dictionary<string, CraftingRecipe> GetAllRecipes()
        {
            return new Dictionary<string, CraftingRecipe>(recipes);
        }
    }
    
    [System.Serializable]
    public class CraftingRecipe
    {
        public string Name;
        public Dictionary<MaterialType, int> Ingredients;
        public Dictionary<MaterialType, int> Outputs;
        public float CraftingTime;
        public FacilityType RequiredFacility;
        
        public CraftingRecipe(string name, Dictionary<MaterialType, int> ingredients, 
                             Dictionary<MaterialType, int> outputs, float craftingTime, 
                             FacilityType requiredFacility)
        {
            Name = name;
            Ingredients = ingredients;
            Outputs = outputs;
            CraftingTime = craftingTime;
            RequiredFacility = requiredFacility;
        }
    }
    
    public enum FacilityType
    {
        Workbench,
        Furnace
    }
}