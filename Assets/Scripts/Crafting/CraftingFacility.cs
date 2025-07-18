using System.Collections;
using UnityEngine;
using VERTEX.Core;

namespace VERTEX.Crafting
{
    public class CraftingFacility : MonoBehaviour
    {
        [Header("Facility Settings")]
        [SerializeField] private FacilityType facilityType;
        [SerializeField] private float efficiencyMultiplier = 1f;
        
        private bool isBusy = false;
        private CraftingSystem craftingSystem;
        private CraftingRecipe currentRecipe;
        
        public FacilityType GetFacilityType() => facilityType;
        public bool IsBusy() => isBusy;
        
        void Start()
        {
            StartCoroutine(DelayedRegistration());
        }
        
        private System.Collections.IEnumerator DelayedRegistration()
        {
            yield return null; // Wait one frame
            
            craftingSystem = FindObjectOfType<CraftingSystem>();
            if (craftingSystem != null)
            {
                craftingSystem.RegisterFacility(this);
                Debug.Log($"{facilityType} facility registered successfully");
            }
            else
            {
                Debug.LogError($"CraftingSystem not found! {facilityType} facility cannot register.");
            }
        }
        
        public void StartCrafting(CraftingRecipe recipe)
        {
            if (isBusy)
            {
                Debug.Log($"{facilityType} is already busy!");
                return;
            }
            
            currentRecipe = recipe;
            StartCoroutine(CraftingProcess());
        }
        
        private IEnumerator CraftingProcess()
        {
            isBusy = true;
            
            Debug.Log($"Starting to craft {currentRecipe.Name} at {facilityType}");
            
            // Visual feedback - change color while crafting
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            Color originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
            
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.yellow; // Working color
            }
            
            // Wait for crafting time (adjusted by efficiency)
            float actualCraftingTime = currentRecipe.CraftingTime / efficiencyMultiplier;
            yield return new WaitForSeconds(actualCraftingTime);
            
            // Restore original color
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
            
            // Complete crafting
            if (craftingSystem != null)
            {
                craftingSystem.OnCraftingComplete(currentRecipe);
            }
            
            currentRecipe = null;
            isBusy = false;
        }
        
        public void SetEfficiencyMultiplier(float multiplier)
        {
            efficiencyMultiplier = Mathf.Max(0.1f, multiplier);
        }
        
        public float GetEfficiencyMultiplier()
        {
            return efficiencyMultiplier;
        }
        
        public string GetCurrentRecipeName()
        {
            return currentRecipe != null ? currentRecipe.Name : "Idle";
        }
        
        void OnDestroy()
        {
            if (craftingSystem != null)
            {
                craftingSystem.UnregisterFacility(this);
            }
        }
    }
}