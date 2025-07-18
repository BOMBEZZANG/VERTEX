using UnityEngine;
using VERTEX.Units;
using VERTEX.Visualization;

namespace VERTEX.Systems
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private bool pauseOnStart = false;
        
        private WorldGrid worldGrid;
        private PhysicsCalculationSystem physicsSystem;
        private ResourceManager resourceManager;
        private UnitManager unitManager;
        private ConstructionSystem constructionSystem;
        private StructuralVisualization structuralVisualization;
        private CameraController cameraController;
        
        private bool isPaused = false;
        
        public bool IsPaused => isPaused;
        
        void Awake()
        {
            // Initialize systems in correct order
            InitializeSystems();
        }
        
        void Start()
        {
            // Wait one frame to ensure all systems are initialized
            StartCoroutine(DelayedStart());
        }
        
        private System.Collections.IEnumerator DelayedStart()
        {
            yield return null; // Wait one frame
            
            // Start the game
            StartGame();
            
            if (pauseOnStart)
            {
                PauseGame();
            }
        }
        
        private void InitializeSystems()
        {
            // Find or create core systems
            worldGrid = FindObjectOfType<WorldGrid>();
            if (worldGrid == null)
            {
                GameObject worldGridObj = new GameObject("WorldGrid");
                worldGrid = worldGridObj.AddComponent<WorldGrid>();
            }
            
            physicsSystem = FindObjectOfType<PhysicsCalculationSystem>();
            if (physicsSystem == null)
            {
                GameObject physicsObj = new GameObject("PhysicsSystem");
                physicsSystem = physicsObj.AddComponent<PhysicsCalculationSystem>();
            }
            
            resourceManager = FindObjectOfType<ResourceManager>();
            if (resourceManager == null)
            {
                GameObject resourceObj = new GameObject("ResourceManager");
                resourceManager = resourceObj.AddComponent<ResourceManager>();
            }
            
            unitManager = FindObjectOfType<UnitManager>();
            if (unitManager == null)
            {
                GameObject unitObj = new GameObject("UnitManager");
                unitManager = unitObj.AddComponent<UnitManager>();
            }
            
            constructionSystem = FindObjectOfType<ConstructionSystem>();
            if (constructionSystem == null)
            {
                GameObject constructionObj = new GameObject("ConstructionSystem");
                constructionSystem = constructionObj.AddComponent<ConstructionSystem>();
            }
            
            structuralVisualization = FindObjectOfType<StructuralVisualization>();
            if (structuralVisualization == null)
            {
                GameObject visualObj = new GameObject("StructuralVisualization");
                structuralVisualization = visualObj.AddComponent<StructuralVisualization>();
            }
            
            cameraController = FindObjectOfType<CameraController>();
            if (cameraController == null && Camera.main != null)
            {
                cameraController = Camera.main.gameObject.AddComponent<CameraController>();
            }
            
            // Initialize connections between systems
            physicsSystem.Initialize(worldGrid);
        }
        
        void Update()
        {
            HandleInput();
        }
        
        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
            
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ShowHelp();
            }
        }
        
        private void StartGame()
        {
            Debug.Log("VERTEX Game Started!");
            Debug.Log("Build upwards while reinforcing the ground below!");
            
            // Focus camera on spawn area
            if (cameraController != null)
            {
                cameraController.FocusOnPosition(new Vector3(25f, 51f, 0f));
            }
        }
        
        public void PauseGame()
        {
            isPaused = true;
            Time.timeScale = 0f;
            Debug.Log("Game Paused");
        }
        
        public void ResumeGame()
        {
            isPaused = false;
            Time.timeScale = 1f;
            Debug.Log("Game Resumed");
        }
        
        public void TogglePause()
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        
        public void RestartGame()
        {
            Debug.Log("Restarting game...");
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
        
        private void ShowHelp()
        {
            Debug.Log("=== VERTEX GAME HELP ===");
            Debug.Log("GOAL: Build a tower upwards while managing ground stability");
            Debug.Log("CONTROLS:");
            Debug.Log("- B: Toggle Build Mode");
            Debug.Log("- 1/2/3: Select Material (Wood/Stone/Steel)");
            Debug.Log("- Click: Build block or Select unit");
            Debug.Log("- Shift+Click: Dig/Remove block");
            Debug.Log("- Ctrl+Click: Multi-select units");
            Debug.Log("- V: Toggle Structural View");
            Debug.Log("- WASD/Arrow Keys: Move camera");
            Debug.Log("- Mouse Wheel: Zoom");
            Debug.Log("- ESC: Pause/Resume");
            Debug.Log("- R: Restart");
            Debug.Log("- F1: Show this help");
            Debug.Log("========================");
        }
        
        public void OnGameOver(string reason)
        {
            Debug.Log($"GAME OVER: {reason}");
            PauseGame();
        }
        
        public void OnVictory()
        {
            Debug.Log("VICTORY! Tower construction successful!");
            PauseGame();
        }
        
        // Getters for other systems
        public WorldGrid GetWorldGrid() => worldGrid;
        public PhysicsCalculationSystem GetPhysicsSystem() => physicsSystem;
        public ResourceManager GetResourceManager() => resourceManager;
        public UnitManager GetUnitManager() => unitManager;
        public ConstructionSystem GetConstructionSystem() => constructionSystem;
        public StructuralVisualization GetStructuralVisualization() => structuralVisualization;
        public CameraController GetCameraController() => cameraController;
    }
}