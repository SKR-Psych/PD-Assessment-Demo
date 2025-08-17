using UnityEngine;
using SortingBoardGame.Data;

namespace SortingBoardGame.Managers
{
    public class SceneSetup : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private GameObject boardPrefab;
        [SerializeField] private GameObject cameraPrefab;
        
        void Start()
        {
            SetupScene();
        }
        
        private void SetupScene()
        {
            // Create main camera if not exists
            Camera mainCamera = FindObjectOfType<Camera>();
            if (mainCamera == null)
            {
                GameObject cameraObj = new GameObject("Main Camera");
                mainCamera = cameraObj.AddComponent<Camera>();
                cameraObj.AddComponent<CameraController>();
                cameraObj.tag = "MainCamera";
            }
            
            // Ensure camera has CameraController
            CameraController cameraController = mainCamera.GetComponent<CameraController>();
            if (cameraController == null)
            {
                cameraController = mainCamera.gameObject.AddComponent<CameraController>();
            }
            
            // Create basic lighting
            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.0f;
            light.shadows = LightShadows.Soft;
            lightObj.transform.rotation = Quaternion.Euler(45f, 45f, 0f);
            
            Debug.Log("Scene setup complete - Camera and lighting configured");
        }
    }
}