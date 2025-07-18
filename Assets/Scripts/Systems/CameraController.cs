using UnityEngine;
using UnityEngine.EventSystems;

namespace VERTEX.Systems
{
    public class CameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float minZoom = 2f;
        [SerializeField] private float maxZoom = 20f;
        [SerializeField] private bool enableMouseEdgeScrolling = true;
        
        [Header("Boundaries")]
        [SerializeField] private float leftBoundary = -5f;
        [SerializeField] private float rightBoundary = 55f;
        [SerializeField] private float bottomBoundary = -50f;
        [SerializeField] private float topBoundary = 100f;
        
        private Camera cam;
        private WorldGrid worldGrid;
        
        void Start()
        {
            cam = GetComponent<Camera>();
            worldGrid = FindObjectOfType<WorldGrid>();
            
            if (worldGrid != null)
            {
                // Set boundaries based on world grid
                rightBoundary = worldGrid.WorldWidth + 5f;
            }
        }
        
        void Update()
        {
            HandleMovement();
            HandleZoom();
        }
        
        private void HandleMovement()
        {
            Vector3 movement = Vector3.zero;
            
            // Keyboard input
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                movement.y += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                movement.y -= 1f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                movement.x -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                movement.x += 1f;
            
            // Mouse edge scrolling (only when enabled and not over UI)
            if (enableMouseEdgeScrolling && !IsMouseOverUI())
            {
                Vector3 mousePos = Input.mousePosition;
                float screenWidth = Screen.width;
                float screenHeight = Screen.height;
                float edgeThreshold = 50f;
                
                if (mousePos.x < edgeThreshold)
                    movement.x -= 1f;
                if (mousePos.x > screenWidth - edgeThreshold)
                    movement.x += 1f;
                if (mousePos.y < edgeThreshold)
                    movement.y -= 1f;
                if (mousePos.y > screenHeight - edgeThreshold)
                    movement.y += 1f;
            }
            
            // Apply movement
            if (movement != Vector3.zero)
            {
                movement = movement.normalized * moveSpeed * Time.deltaTime;
                Vector3 newPosition = transform.position + movement;
                
                // Clamp to boundaries
                newPosition.x = Mathf.Clamp(newPosition.x, leftBoundary, rightBoundary);
                newPosition.y = Mathf.Clamp(newPosition.y, bottomBoundary, topBoundary);
                
                transform.position = newPosition;
            }
        }
        
        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            
            if (scroll != 0)
            {
                float newSize = cam.orthographicSize - scroll * zoomSpeed;
                cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
            }
        }
        
        public void FocusOnPosition(Vector3 position)
        {
            Vector3 targetPosition = new Vector3(position.x, position.y, transform.position.z);
            
            // Clamp to boundaries
            targetPosition.x = Mathf.Clamp(targetPosition.x, leftBoundary, rightBoundary);
            targetPosition.y = Mathf.Clamp(targetPosition.y, bottomBoundary, topBoundary);
            
            transform.position = targetPosition;
        }
        
        public void SetBoundaries(float left, float right, float bottom, float top)
        {
            leftBoundary = left;
            rightBoundary = right;
            bottomBoundary = bottom;
            topBoundary = top;
        }
        
        public Vector3 GetWorldPositionFromScreenPoint(Vector3 screenPoint)
        {
            return cam.ScreenToWorldPoint(screenPoint);
        }
        
        private bool IsMouseOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}