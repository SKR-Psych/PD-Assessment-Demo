using UnityEngine;
using SortingBoardGame.Managers;
using SortingBoardGame.Gameplay;
using SortingBoardGame.UI;

namespace SortingBoardGame.Setup
{
    public class QuickGameSetup : MonoBehaviour
    {
        [Header("Quick Setup Options")]
        [SerializeField] private bool setupOnStart = true;
        [SerializeField] private bool enableTesting = false;
        
        void Start()
        {
            if (setupOnStart)
            {
                SetupGame();
            }
        }
        
        public void SetupGame()
        {
            Debug.Log("Quick Game Setup: Starting...");
            
            // Create all essential game objects
            CreateEssentialManagers();
            CreateGameplayComponents();
            CreateUIComponents();
            
            if (enableTesting)
            {
                EnableTestingComponents();
            }
            
            Debug.Log("Quick Game Setup: Complete!");
        }
        
        private void CreateEssentialManagers()
        {
            // GameManager (singleton)
            if (GameManager.Instance == null)
            {
                GameObject gameManagerObj = new GameObject("GameManager");
                gameManagerObj.AddComponent<GameManager>();
            }
            
            // AudioManager (singleton)
            if (AudioManager.Instance == null)
            {
                GameObject audioManagerObj = new GameObject("AudioManager");
                audioManagerObj.AddComponent<AudioManager>();
            }
            
            // StatisticsManager (singleton)
            if (StatisticsManager.Instance == null)
            {
                GameObject statsManagerObj = new GameObject("StatisticsManager");
                statsManagerObj.AddComponent<StatisticsManager>();
            }
            
            // DataLogger (singleton)
            if (DataLogger.Instance == null)
            {
                GameObject dataLoggerObj = new GameObject("DataLogger");
                dataLoggerObj.AddComponent<DataLogger>();
            }
            
            // PerformanceManager (singleton)
            if (PerformanceManager.Instance == null)
            {
                GameObject perfManagerObj = new GameObject("PerformanceManager");
                perfManagerObj.AddComponent<PerformanceManager>();
            }
        }
        
        private void CreateGameplayComponents()
        {
            // GameBoard
            if (FindObjectOfType<GameBoard>() == null)
            {
                GameObject gameBoardObj = new GameObject("GameBoard");
                gameBoardObj.AddComponent<GameBoard>();
            }
            
            // HoleManager
            if (FindObjectOfType<HoleManager>() == null)
            {
                GameObject holeManagerObj = new GameObject("HoleManager");
                holeManagerObj.AddComponent<HoleManager>();
            }
            
            // BallSpawner
            if (FindObjectOfType<BallSpawner>() == null)
            {
                GameObject ballSpawnerObj = new GameObject("BallSpawner");
                ballSpawnerObj.AddComponent<BallSpawner>();
            }
            
            // MouseInteractionSystem
            if (FindObjectOfType<MouseInteractionSystem>() == null)
            {
                GameObject mouseSystemObj = new GameObject("MouseInteractionSystem");
                mouseSystemObj.AddComponent<MouseInteractionSystem>();
            }
            
            // BallInteractionHandler
            if (FindObjectOfType<BallInteractionHandler>() == null)
            {
                GameObject interactionHandlerObj = new GameObject("BallInteractionHandler");
                interactionHandlerObj.AddComponent<BallInteractionHandler>();
            }
            
            // VisualEffectsManager
            if (FindObjectOfType<VisualEffectsManager>() == null)
            {
                GameObject visualEffectsObj = new GameObject("VisualEffectsManager");
                visualEffectsObj.AddComponent<VisualEffectsManager>();
            }
            
            // LevelController
            if (FindObjectOfType<LevelController>() == null)
            {
                GameObject levelControllerObj = new GameObject("LevelController");
                levelControllerObj.AddComponent<LevelController>();
            }
        }
        
        private void CreateUIComponents()
        {
            // HUDManager
            if (FindObjectOfType<HUDManager>() == null)
            {
                GameObject hudManagerObj = new GameObject("HUDManager");
                hudManagerObj.AddComponent<HUDManager>();
            }
            
            // EndLevelUI
            if (FindObjectOfType<EndLevelUI>() == null)
            {
                GameObject endLevelUIObj = new GameObject("EndLevelUI");
                endLevelUIObj.AddComponent<EndLevelUI>();
            }
        }
        
        private void EnableTestingComponents()
        {
            // GameTester
            if (FindObjectOfType<GameTester>() == null)
            {
                GameObject gameTesterObj = new GameObject("GameTester");
                gameTesterObj.AddComponent<GameTester>();
            }
            
            // SceneOrchestrator
            if (FindObjectOfType<SceneOrchestrator>() == null)
            {
                GameObject sceneOrchestratorObj = new GameObject("SceneOrchestrator");
                sceneOrchestratorObj.AddComponent<SceneOrchestrator>();
            }
        }
        
        // Create main camera if it doesn't exist
        private void CreateMainCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                GameObject cameraObj = new GameObject("Main Camera");
                mainCamera = cameraObj.AddComponent<Camera>();
                cameraObj.AddComponent<AudioListener>();
                cameraObj.tag = "MainCamera";
                
                // Position camera for optimal view
                cameraObj.transform.position = new Vector3(0, 2, -2);
                cameraObj.transform.rotation = Quaternion.Euler(45, 0, 0);
                mainCamera.fieldOfView = 45f;
            }
        }
        
        // Create basic lighting
        private void CreateLighting()
        {
            Light[] lights = FindObjectsOfType<Light>();
            if (lights.Length == 0)
            {
                GameObject lightObj = new GameObject("Directional Light");
                Light light = lightObj.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1.0f;
                light.shadows = LightShadows.Soft;
                lightObj.transform.rotation = Quaternion.Euler(45f, 45f, 0f);
            }
        }
        
        [ContextMenu("Setup Game Now")]
        public void SetupGameFromMenu()
        {
            SetupGame();
        }
        
        [ContextMenu("Test All Systems")]
        public void TestAllSystems()
        {
            GameTester tester = FindObjectOfType<GameTester>();
            if (tester != null)
            {
                tester.RunQuickTest();
            }
            else
            {
                Debug.LogWarning("GameTester not found - enable testing components first");
            }
        }
    }
}