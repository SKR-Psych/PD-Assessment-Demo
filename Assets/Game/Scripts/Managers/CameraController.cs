using UnityEngine;
using SortingBoardGame.Data;

namespace SortingBoardGame.Managers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private Transform boardTransform;
        
        private Camera mainCamera;
        
        void Start()
        {
            mainCamera = GetComponent<Camera>();
            SetupCamera();
        }
        
        private void SetupCamera()
        {
            if (gameConfig == null)
            {
                Debug.LogError("GameConfig not assigned to CameraController!");
                return;
            }
            
            // Set camera FOV
            mainCamera.fieldOfView = gameConfig.cameraFOV;
            
            // Position camera at 45° angle to view the board optimally
            Vector3 boardCenter = boardTransform != null ? boardTransform.position : Vector3.zero;
            boardCenter.y = gameConfig.boardHeight;
            
            // Calculate camera position for 45° angle view
            float distance = 2.0f; // Distance from board center
            Vector3 cameraPosition = boardCenter + new Vector3(0, distance * Mathf.Sin(gameConfig.cameraAngle * Mathf.Deg2Rad), 
                                                              -distance * Mathf.Cos(gameConfig.cameraAngle * Mathf.Deg2Rad));
            
            transform.position = cameraPosition;
            transform.LookAt(boardCenter);
            
            // Ensure camera is completely static (no player control)
            GetComponent<Camera>().enabled = true;
        }
        
        void Update()
        {
            // Camera remains completely static - no player control
        }
    }
}