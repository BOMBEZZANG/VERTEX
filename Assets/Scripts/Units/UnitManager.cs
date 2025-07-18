using System.Collections.Generic;
using UnityEngine;
using VERTEX.Core;

namespace VERTEX.Units
{
    public class UnitManager : MonoBehaviour
    {
        [Header("Unit Management")]
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private int initialUnitCount = 3;
        
        private List<Unit> allUnits = new List<Unit>();
        private List<Unit> selectedUnits = new List<Unit>();
        
        public List<Unit> SelectedUnits => selectedUnits;
        
        void Start()
        {
            SpawnInitialUnits();
        }
        
        private void SpawnInitialUnits()
        {
            for (int i = 0; i < initialUnitCount; i++)
            {
                Vector3 spawnPosition = new Vector3(i * 2f, 51f, 0f); // Spawn above surface
                GameObject unitObj = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
                Unit unit = unitObj.GetComponent<Unit>();
                
                if (unit != null)
                {
                    allUnits.Add(unit);
                }
            }
        }
        
        public void SelectUnit(Unit unit)
        {
            if (!selectedUnits.Contains(unit))
            {
                selectedUnits.Add(unit);
                unit.SetSelected(true);
            }
        }
        
        public void DeselectUnit(Unit unit)
        {
            if (selectedUnits.Contains(unit))
            {
                selectedUnits.Remove(unit);
                unit.SetSelected(false);
            }
        }
        
        public void SelectAllUnits()
        {
            ClearSelection();
            foreach (Unit unit in allUnits)
            {
                SelectUnit(unit);
            }
        }
        
        public void ClearSelection()
        {
            foreach (Unit unit in selectedUnits)
            {
                unit.SetSelected(false);
            }
            selectedUnits.Clear();
        }
        
        public void CommandSelectedUnits(UnitCommand command, Vector2Int position, MaterialType materialType = MaterialType.Air)
        {
            foreach (Unit unit in selectedUnits)
            {
                if (unit.IsBusy) continue;
                
                switch (command)
                {
                    case UnitCommand.Move:
                        unit.MoveTo(position);
                        break;
                    case UnitCommand.Dig:
                        unit.DigAt(position);
                        break;
                    case UnitCommand.Build:
                        unit.BuildAt(position, materialType);
                        break;
                }
            }
        }
        
        public Unit GetNearestUnit(Vector3 position)
        {
            Unit nearestUnit = null;
            float nearestDistance = float.MaxValue;
            
            foreach (Unit unit in allUnits)
            {
                float distance = Vector3.Distance(unit.transform.position, position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestUnit = unit;
                }
            }
            
            return nearestUnit;
        }
        
        public List<Unit> GetAvailableUnits()
        {
            List<Unit> availableUnits = new List<Unit>();
            foreach (Unit unit in allUnits)
            {
                if (!unit.IsBusy)
                {
                    availableUnits.Add(unit);
                }
            }
            return availableUnits;
        }
    }
    
    public enum UnitCommand
    {
        Move,
        Dig,
        Build
    }
}