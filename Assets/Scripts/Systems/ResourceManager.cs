using System.Collections.Generic;
using UnityEngine;
using VERTEX.Core;

namespace VERTEX.Systems
{
    public class ResourceManager : MonoBehaviour
    {
        [Header("Starting Resources")]
        [SerializeField] private int startingWood = 50;
        [SerializeField] private int startingStone = 20;
        [SerializeField] private int startingCoal = 10;
        [SerializeField] private int startingIron = 5;
        
        private Dictionary<MaterialType, int> resources = new Dictionary<MaterialType, int>();
        
        public System.Action<MaterialType, int> OnResourceChanged;
        
        void Start()
        {
            InitializeResources();
        }
        
        private void InitializeResources()
        {
            resources[MaterialType.Wood] = startingWood;
            resources[MaterialType.Stone] = startingStone;
            resources[MaterialType.Coal] = startingCoal;
            resources[MaterialType.Iron] = startingIron;
            
            // Notify UI of initial values
            foreach (var kvp in resources)
            {
                OnResourceChanged?.Invoke(kvp.Key, kvp.Value);
            }
        }
        
        public int GetResourceCount(MaterialType materialType)
        {
            return resources.ContainsKey(materialType) ? resources[materialType] : 0;
        }
        
        public bool HasResource(MaterialType materialType, int amount = 1)
        {
            return GetResourceCount(materialType) >= amount;
        }
        
        public bool ConsumeResource(MaterialType materialType, int amount = 1)
        {
            if (!HasResource(materialType, amount))
            {
                return false;
            }
            
            resources[materialType] -= amount;
            OnResourceChanged?.Invoke(materialType, resources[materialType]);
            return true;
        }
        
        public void AddResource(MaterialType materialType, int amount = 1)
        {
            if (!resources.ContainsKey(materialType))
            {
                resources[materialType] = 0;
            }
            
            resources[materialType] += amount;
            OnResourceChanged?.Invoke(materialType, resources[materialType]);
        }
        
        public ResourceTier GetResourceTier(MaterialType materialType)
        {
            switch (materialType)
            {
                case MaterialType.Wood:
                    return ResourceTier.Tier1;
                case MaterialType.Stone:
                case MaterialType.Coal:
                case MaterialType.Iron:
                    return ResourceTier.Tier2;
                default:
                    return ResourceTier.Tier1;
            }
        }
        
        public List<MaterialType> GetAvailableConstructionMaterials()
        {
            List<MaterialType> availableMaterials = new List<MaterialType>();
            
            if (HasResource(MaterialType.Wood))
                availableMaterials.Add(MaterialType.Wood);
            if (HasResource(MaterialType.Stone))
                availableMaterials.Add(MaterialType.Stone);
            if (HasResource(MaterialType.Steel))
                availableMaterials.Add(MaterialType.Steel);
            
            return availableMaterials;
        }
        
        public Dictionary<MaterialType, int> GetAllResources()
        {
            return new Dictionary<MaterialType, int>(resources);
        }
    }
}