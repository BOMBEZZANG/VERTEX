using UnityEngine;
using VERTEX.Systems;
using VERTEX.Units;
using VERTEX.Crafting;
using VERTEX.Visualization;
using VERTEX.UI;

namespace VERTEX.Systems
{
    public class SystemVerification : MonoBehaviour
    {
        [Header("System Verification")]
        [SerializeField] private bool runOnStart = true;
        
        void Start()
        {
            if (runOnStart)
            {
                StartCoroutine(VerifySystemsDelayed());
            }
        }
        
        private System.Collections.IEnumerator VerifySystemsDelayed()
        {
            yield return new WaitForSeconds(0.5f); // Wait for all systems to initialize
            
            VerifyAllSystems();
        }
        
        [ContextMenu("Verify All Systems")]
        public void VerifyAllSystems()
        {
            Debug.Log("=== VERTEX SYSTEM VERIFICATION ===");
            
            // Core Systems
            CheckSystem<GameManager>("GameManager");
            CheckSystem<WorldGrid>("WorldGrid");
            CheckSystem<PhysicsCalculationSystem>("PhysicsCalculationSystem");
            CheckSystem<ResourceManager>("ResourceManager");
            CheckSystem<ConstructionSystem>("ConstructionSystem");
            CheckSystem<UnitManager>("UnitManager");
            CheckSystem<CraftingSystem>("CraftingSystem");
            CheckSystem<StructuralVisualization>("StructuralVisualization");
            
            // Check UI System
            var uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                Debug.Log("✅ UIManager found");
            }
            else
            {
                Debug.LogError("❌ UIManager not found!");
            }
            
            // Check Crafting Facilities
            var facilities = FindObjectsOfType<CraftingFacility>();
            Debug.Log($"Found {facilities.Length} crafting facilities:");
            foreach (var facility in facilities)
            {
                Debug.Log($"  - {facility.GetFacilityType()} at {facility.transform.position}");
            }
            
            // Check Units
            var units = FindObjectsOfType<Unit>();
            Debug.Log($"Found {units.Length} units in scene");
            
            Debug.Log("=== VERIFICATION COMPLETE ===");
        }
        
        private void CheckSystem<T>(string systemName) where T : MonoBehaviour
        {
            var system = FindObjectOfType<T>();
            if (system != null)
            {
                Debug.Log($"✅ {systemName} found");
            }
            else
            {
                Debug.LogError($"❌ {systemName} not found!");
            }
        }
    }
}