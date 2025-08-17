using UnityEngine;
using System.Collections;
using SortingBoardGame.Gameplay;
using SortingBoardGame.UI;

namespace SortingBoardGame.Managers
{
    public class SceneOrchestrator : MonoBehaviour
    {
        [Header("Scene Components")]
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private GameBoard gameBoard;
        [SerializeField] private HoleManager holeManager;
        [SerializeField] private BallSpawner ballSpawner;
        [SerializeField] private MouseInteractionSystem mouseSystem;
        [SerializeField] private BallInteractionHandler interactionHandler;
        [SerializeField] private VisualEffectsManager visualEffects;
        [SerializeField] private HUDManager hudManager;
        [SerializeField] private EndLevelUI endLevelUI;
        [SerializeField] private LevelController levelController;
        
        [Header("Scene Setup")]
        [SerializeField] private bool autoSetupScene = true;
        [SerializeField] private bool validateOnStart = true;
        
        void Start()
        {
            StartCoroutine(InitializeSceneCoroutine());
        }
        
        private IEnumerator InitializeSceneCoroutine()
        {
            Debug.Log("Scene Orchestrator: Starting scene initialization...");
            
            // Wait for GameManager to be ready
            yield return new WaitUntil(() => GameManager.Instance != null);
            
            if (autoSetupScene)
            {
                yield return StartCoroutine(SetupSceneComponents());
            }
            
            if (validateOnStart)
            {
                yield return StartCoroutine(ValidateSceneSetup());
            }
            
            // Connect all systems
            yield return StartCoroutine(ConnectSystems());
            
            // Final validation
            bool sceneReady = ValidateAllSystems();
            
            if (sceneReady)
            {
                Debug.Log("✓ Scene initialization complete - all systems ready");
                
                // Notify GameManager that scene is ready
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.NotifySystemReady("Scene");
                }
            }
            else
            {
                Debug.LogError("✗ Scene initialization failed - some systems not ready");
            }
        }
        
        private IEnumerator SetupSceneComponents()
        {
            Debug.Log("Setting up scene components...");
            
            // Create GameConfig if not assigned
            if (gameConfig == null)
            {
                gameConfig = CreateDefaultGameConfig();
            }
            
            // Setup GameBoard
            if (gameBoard == null)
            {
                gameBoard = FindOrCreateGameBoard();
            }
            yield return null;
            
            // Setup HoleManager
            if (holeManager == null)
            {
                holeManager = FindOrCreateHoleManager();
            }
            yield return null;
            
            // Setup BallSpawner
            if (ballSpawner == null)
            {
                ballSpawner = FindOrCreateBallSpawner();
            }
            yield return null;
            
            // Setup MouseInteractionSystem
            if (mouseSystem == null)
            {
                mouseSystem = FindOrCreateMouseSystem();
            }
            yield return null;
            
            // Setup other components
            SetupRemainingComponents();
            yield return null;
            
            Debug.Log("Scene components setup complete");
        }
        
        private GameConfig CreateDefaultGameConfig()
        {
            GameObject configObj = new GameObject("GameConfig");
            configObj.transform.SetParent(transform);
            
            // Create ScriptableObject instance
            GameConfig config = ScriptableObject.CreateInstance<GameConfig>();
            
            // Set default values
            config.totalBalls = 20;
            config.spawnInterval = 2.0f;
            config.ballDiameter = 120f;
            config.boardSize = new Vector3(1.0f, 0.0f, 0.6f);
            config.boardHeight = 0.9f;
            config.holeDiameters = new float[] { 160f, 140f, 120f };
            config.holeColors = new Color[] { Color.red, Color.blue, Color.yellow };
            config.cameraAngle = 45f;
            config.cameraFOV = 45f;
            config.targetFPS = 60;
            config.targetResolutionWidth = 1920;
            config.targetResolutionHeight = 1080;
            
            Debug.Log("Created default GameConfig");
            return config;
        }
        
        private GameBoard FindOrCreateGameBoard()
        {
            GameBoard board = FindObjectOfType<GameBoard>();
            if (board == null)
            {
                GameObject boardObj = new GameObject("GameBoard");
                boardObj.transform.SetParent(transform);
                board = boardObj.AddComponent<GameBoard>();
            }
            return board;
        }
        
        private HoleManager FindOrCreateHoleManager()
        {
            HoleManager manager = FindObjectOfType<HoleManager>();
            if (manager == null)
            {
                GameObject managerObj = new GameObject("HoleManager");
                managerObj.transform.SetParent(transform);
                manager = managerObj.AddComponent<HoleManager>();
            }
            return manager;
        }
        
        private BallSpawner FindOrCreateBallSpawner()
        {
            BallSpawner spawner = FindObjectOfType<BallSpawner>();
            if (spawner == null)
            {
                GameObject spawnerObj = new GameObject("BallSpawner");
                spawnerObj.transform.SetParent(transform);
                spawner = spawnerObj.AddComponent<BallSpawner>();
            }
            return spawner;
        }
        
        private MouseInteractionSystem FindOrCreateMouseSystem()
        {
            MouseInteractionSystem mouse = FindObjectOfType<MouseInteractionSystem>();
            if (mouse == null)
            {
                GameObject mouseObj = new GameObject("MouseInteractionSystem");
                mouseObj.transform.SetParent(transform);
                mouse = mouseObj.AddComponent<MouseInteractionSystem>();
            }
            return mouse;
        }
        
        private void SetupRemainingComponents()
        {
            // Setup remaining components
            if (interactionHandler == null)
                interactionHandler = FindObjectOfType<BallInteractionHandler>();
            
            if (visualEffects == null)
                visualEffects = FindObjectOfType<VisualEffectsManager>();
            
            if (hudManager == null)
                hudManager = FindObjectOfType<HUDManager>();
            
            if (endLevelUI == null)
                endLevelUI = FindObjectOfType<EndLevelUI>();
            
            if (levelController == null)
                levelController = FindObjectOfType<LevelController>();
        }
        
        private IEnumerator ValidateSceneSetup()
        {
            Debug.Log("Validating scene setup...");
            
            bool validationPassed = true;
            
            // Validate core components
            if (gameConfig == null)
            {
                Debug.LogError("✗ GameConfig missing");
                validationPassed = false;
            }
            
            if (gameBoard == null)
            {
                Debug.LogError("✗ GameBoard missing");
                validationPassed = false;
            }
            
            if (holeManager == null)
            {
                Debug.LogError("✗ HoleManager missing");
                validationPassed = false;
            }
            
            if (ballSpawner == null)
            {
                Debug.LogError("✗ BallSpawner missing");
                validationPassed = false;
            }
            
            if (mouseSystem == null)
            {
                Debug.LogError("✗ MouseInteractionSystem missing");
                validationPassed = false;
            }
            
            yield return null;
            
            if (validationPassed)
            {
                Debug.Log("✓ Scene validation passed");
            }
            else
            {
                Debug.LogError("✗ Scene validation failed");
            }
        }
        
        private IEnumerator ConnectSystems()
        {
            Debug.Log("Connecting systems...");
            
            // Connect BallInteractionHandler to other systems
            if (interactionHandler != null)
            {
                // The systems will find each other automatically via FindObjectOfType
                // This is handled in their Start() methods
            }
            
            // Connect VisualEffectsManager
            if (visualEffects != null)
            {
                visualEffects.CreateDefaultParticleSystems();
            }
            
            yield return null;
            
            Debug.Log("Systems connected");
        }
        
        private bool ValidateAllSystems()
        {
            bool allValid = true;
            
            // Check singleton managers
            if (GameManager.Instance == null)
            {
                Debug.LogError("✗ GameManager not initialized");
                allValid = false;
            }
            
            if (AudioManager.Instance == null)
            {
                Debug.LogError("✗ AudioManager not initialized");
                allValid = false;
            }
            
            if (StatisticsManager.Instance == null)
            {
                Debug.LogError("✗ StatisticsManager not initialized");
                allValid = false;
            }
            
            if (DataLogger.Instance == null)
            {
                Debug.LogError("✗ DataLogger not initialized");
                allValid = false;
            }
            
            // Check scene components
            if (gameBoard == null || holeManager == null || ballSpawner == null || mouseSystem == null)
            {
                Debug.LogError("✗ Core scene components missing");
                allValid = false;
            }
            
            return allValid;
        }
        
        // Public methods for external access
        public bool IsSceneReady()
        {
            return ValidateAllSystems();
        }
        
        public void RestartScene()
        {
            StartCoroutine(RestartSceneCoroutine());
        }
        
        private IEnumerator RestartSceneCoroutine()
        {
            Debug.Log("Restarting scene...");
            
            // Reset all systems
            if (ballSpawner != null)
            {
                ballSpawner.StopSpawning();
                ballSpawner.ClearAllBalls();
            }
            
            if (StatisticsManager.Instance != null)
            {
                StatisticsManager.Instance.ResetStatistics();
            }
            
            if (DataLogger.Instance != null)
            {
                DataLogger.Instance.StartNewSession();
            }
            
            yield return new WaitForSeconds(0.5f);
            
            // Restart level
            if (levelController != null)
            {
                levelController.StartLevel();
            }
            
            Debug.Log("Scene restart complete");
        }
    }
}