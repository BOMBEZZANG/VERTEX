using UnityEngine;

namespace VERTEX.Core
{
    [System.Serializable]
    public class Tile
    {
        public MaterialType materialType;
        public float mass;
        public float supportValue;
        public float currentLoad;
        public bool isFoundation;
        public bool isSupported;
        public Vector2Int position;
        
        public Tile(MaterialType type, Vector2Int pos)
        {
            materialType = type;
            position = pos;
            SetMaterialProperties();
            currentLoad = 0f;
            isFoundation = false;
            isSupported = false;
        }
        
        private void SetMaterialProperties()
        {
            switch (materialType)
            {
                case MaterialType.Wood:
                    mass = 5f;
                    supportValue = 100f;
                    break;
                case MaterialType.Stone:
                    mass = 20f;
                    supportValue = 500f;
                    break;
                case MaterialType.Steel:
                    mass = 15f;
                    supportValue = 1500f;
                    break;
                case MaterialType.Dirt:
                    mass = 10f;
                    supportValue = 50f;
                    break;
                case MaterialType.Air:
                    mass = 0f;
                    supportValue = 0f;
                    break;
                default:
                    mass = 1f;
                    supportValue = 10f;
                    break;
            }
        }
        
        public bool IsStructural()
        {
            return materialType != MaterialType.Air;
        }
        
        public bool WillCollapse()
        {
            return currentLoad > supportValue;
        }
        
        public LoadStatus GetLoadStatus()
        {
            if (supportValue == 0) return LoadStatus.Safe;
            
            float ratio = currentLoad / supportValue;
            if (ratio >= 1.0f) return LoadStatus.Critical;
            if (ratio >= 0.75f) return LoadStatus.Stressed;
            if (ratio >= 0.5f) return LoadStatus.Moderate;
            return LoadStatus.Safe;
        }
    }
}