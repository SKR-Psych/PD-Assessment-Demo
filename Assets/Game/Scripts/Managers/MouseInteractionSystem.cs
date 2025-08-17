using UnityEngine;
using SortingBoardGame.Gameplay;
using SortingBoardGame.Data;

namespace SortingBoardGame.Managers
{
    public class MouseInteractionSystem : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask ballLayerMask = -1;
        [SerializeField] private float dragHeight = 1.0f; // Height to maintain balls during drag
        
        private Ball currentDraggedBall = null;
        private Ball hoveredBall = null;
        private Vector3 dragOffset = Vector3.zero;
        private bool isDragging = false;
        
        // Mouse interaction events
        public System.Action<Ball> OnBallHoverStart;
        public System.Action<Ball> OnBallHoverEnd;
        public System.Action<Ball> OnBallPickup;
        public System.Action<Ball, Vector3> OnBallDrag;
        public System.Action<Ball, Vector3> OnBallRelease;
        
        void Start()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    mainCamera = FindObjectOfType<Camera>();
                }
            }
            
            if (mainCamera == null)
            {
                Debug.LogError("No camera found for MouseInteractionSystem!");
            }
        }
        
        void Update()
        {
            HandleMouseInput();
        }
        
        private void HandleMouseInput()
        {
            if (mainCamera == null) return;
            
            // Handle mouse hover detection
            if (!isDragging)
            {
                HandleMouseHover();
            }
            
            // Handle mouse click and drag
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseDown();
            }
            else if (Input.GetMouseButton(0) && isDragging)
            {
                HandleMouseDrag();
            }
            else if (Input.GetMouseButtonUp(0) && isDragging)
            {
                HandleMouseUp();
            }
        }
        
        private void HandleMouseHover()
        {
            Ball ball = GetBallUnderMouse();
            
            if (ball != hoveredBall)
            {
                // End previous hover
                if (hoveredBall != null)
                {
                    hoveredBall.SetState(BallState.Idle);
                    OnBallHoverEnd?.Invoke(hoveredBall);
                }
                
                // Start new hover
                hoveredBall = ball;
                if (hoveredBall != null && hoveredBall.State == BallState.Idle)
                {
                    hoveredBall.SetState(BallState.Hovered);
                    OnBallHoverStart?.Invoke(hoveredBall);
                }
            }
        }
        
        private void HandleMouseDown()
        {
            Ball ball = GetBallUnderMouse();
            
            if (ball != null && (ball.State == BallState.Idle || ball.State == BallState.Hovered))
            {
                StartDragging(ball);
            }
        }
        
        private void HandleMouseDrag()
        {
            if (currentDraggedBall == null) return;
            
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            if (mouseWorldPos != Vector3.zero)
            {
                // Maintain ball at drag height
                mouseWorldPos.y = dragHeight;
                
                // Apply drag offset to maintain smooth dragging
                Vector3 targetPosition = mouseWorldPos + dragOffset;
                currentDraggedBall.transform.position = targetPosition;
                
                OnBallDrag?.Invoke(currentDraggedBall, targetPosition);
            }
        }
        
        private void HandleMouseUp()
        {
            if (currentDraggedBall != null)
            {
                Vector3 releasePosition = currentDraggedBall.transform.position;
                Ball releasedBall = currentDraggedBall;
                
                StopDragging();
                
                OnBallRelease?.Invoke(releasedBall, releasePosition);
            }
        }
        
        private void StartDragging(Ball ball)
        {
            currentDraggedBall = ball;
            isDragging = true;
            
            // Calculate drag offset to prevent ball jumping to mouse position
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            if (mouseWorldPos != Vector3.zero)
            {
                dragOffset = ball.transform.position - mouseWorldPos;
                dragOffset.y = 0; // Don't offset Y axis
            }
            
            // Set ball state to dragged
            ball.SetState(BallState.Dragged);
            
            // Clear hover state
            if (hoveredBall == ball)
            {
                hoveredBall = null;
            }
            
            OnBallPickup?.Invoke(ball);
            
            Debug.Log($"Started dragging ball {ball.BallId} ({ball.Color})");    
    }
        
        private void StopDragging()
        {
            if (currentDraggedBall != null)
            {
                Debug.Log($"Stopped dragging ball {currentDraggedBall.BallId} ({currentDraggedBall.Color})");
            }
            
            currentDraggedBall = null;
            isDragging = false;
            dragOffset = Vector3.zero;
        }
        
        private Ball GetBallUnderMouse()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ballLayerMask))
            {
                Ball ball = hit.collider.GetComponent<Ball>();
                return ball;
            }
            
            return null;
        }
        
        private Vector3 GetMouseWorldPosition()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            
            // Create a plane at the drag height to project mouse position onto
            Plane dragPlane = new Plane(Vector3.up, new Vector3(0, dragHeight, 0));
            
            float distance;
            if (dragPlane.Raycast(ray, out distance))
            {
                return ray.GetPoint(distance);
            }
            
            return Vector3.zero;
        }
        
        // Public methods for external systems
        public bool IsDragging => isDragging;
        public Ball CurrentDraggedBall => currentDraggedBall;
        public Ball HoveredBall => hoveredBall;
        
        public void ForceStopDragging()
        {
            if (isDragging && currentDraggedBall != null)
            {
                currentDraggedBall.SetState(BallState.Idle);
                StopDragging();
            }
        }
        
        public void SetDragHeight(float height)
        {
            dragHeight = height;
        }
        
        // Debug visualization
        void OnDrawGizmos()
        {
            if (isDragging && currentDraggedBall != null)
            {
                // Draw line from camera to dragged ball
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(mainCamera.transform.position, currentDraggedBall.transform.position);
                
                // Draw drag plane
                Gizmos.color = Color.cyan;
                Vector3 planeCenter = new Vector3(0, dragHeight, 0);
                Gizmos.DrawWireCube(planeCenter, new Vector3(5, 0.1f, 5));
            }
        }
    }
}