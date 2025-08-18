using UnityEngine;
using System.Collections;
using SortingBoardGame.Data;
using SortingBoardGame.Gameplay;

namespace SortingBoardGame.Managers
{
    public enum GameState
    {
        Initializing,
        MainMenu,
        Playing,
        Paused,
        LevelComplete,
        GameOver,
        Exiting
    }
    
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("Game Configuration")]
        [SerializeField] private GameConfig gameConfig;
        
        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.Initializing;
        [SerializeField] private bool isPaused = false;
        
        [Header("System References")]
        [SerializeField] private LevelController levelController;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private StatisticsManager statisticsManager;
        [SerializeField] private DataLogger dataLogger;
        [SerializeField] private HUDManager hudManager;
        
        // Game state events
        public System.Action<GameState> OnGameStateChanged;
        public System.Action OnGamePaused;
        public System.Action OnGameResumed;
        public System.Action OnGameReset;
        
        // Error recovery
        private Coroutine errorRecoveryCoroutine;
        private int consecutiveErrors = 0;
        private const int maxConsecutiveErrors = 5;
        
        public GameConfig Config => gameConfig;
        public GameState CurrentState => currentState;
        public bool IsPaused => isPaused;
        
        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            StartCoroutine(InitializeSystemsCoroutine());
        }
        
        private void InitializeGame()
        {
            // Create default config if none assigned
            if (gameConfig == null)
            {
                gameConfig = CreateDefaultGameConfig();
            }
            
            // Set target frame rate
            Application.targetFrameRate = gameConfig.targetFPS;
            
            // Set screen resolution for Windows build
            if (!Application.isEditor)
            {
                Screen.SetResolution(gameConfig.targetResolutionWidth, gameConfig.targetResolutionHeight, false);
            }
            
            // Setup error handling
            Application.logMessageReceived += HandleLogMessage;
            
            Debug.Log("Game Manager initialized - Sorting Board Game v1.0");
        }
        
        private GameConfig CreateDefaultGameConfig()
        {
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
        
        private IEnumerator InitializeSystemsCoroutine()
        {
            SetGameState(GameState.Initializing);
            
            // Find system references if not assigned
            yield return StartCoroutine(FindSystemReferences());
            
            // Initialize all systems
            yield return StartCoroutine(InitializeSystems());
            
            // Start the game
            SetGameState(GameState.Playing);
            
            Debug.Log("All systems initialized successfully");
        }
        
        private IEnumerator FindSystemReferences()
        {
            if (levelController == null)
                levelController = FindObjectOfType<LevelController>();
            
            if (audioManager == null)
                audioManager = AudioManager.Instance;
            
            if (statisticsManager == null)
                statisticsManager = StatisticsManager.Instance;
            
            if (dataLogger == null)
                dataLogger = DataLogger.Instance;
            
            if (hudManager == null)
                hudManager = FindObjectOfType<HUDManager>();
            
            yield return null; // Wait one frame
        }
        
        private IEnumerator InitializeSystems()
        {
            // Initialize systems in order
            try
            {
                // Audio system
                if (audioManager != null)
                {
                    yield return null;
                    Debug.Log("Audio system ready");
                }
                
                // Statistics system
                if (statisticsManager != null)
                {
                    yield return null;
                    Debug.Log("Statistics system ready");
                }
                
                // Data logging system
                if (dataLogger != null)
                {
                    yield return null;
                    Debug.Log("Data logging system ready");
                }
                
                // HUD system
                if (hudManager != null)
                {
                    yield return null;
                    Debug.Log("HUD system ready");
                }
                
                // Level controller (start last)
                if (levelController != null)
                {
                    yield return null;
                    Debug.Log("Level controller ready");
                }
                
                consecutiveErrors = 0; // Reset error count on successful initialization
            }
            catch (System.Exception e)
            {
                Debug.LogError($"System initialization error: {e.Message}");
                HandleSystemError("System initialization failed");
            }
        }
        
        void Update()
        {
            // Handle input based on current state
            HandleInput();
            
            // Monitor system health
            MonitorSystemHealth();
        }
        
        private void HandleInput()
        {
            // Handle Escape key based on current state
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleEscapeKey();
            }
            
            // Handle pause/resume
            if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Space))
            {
                if (currentState == GameState.Playing)
                {
                    PauseGame();
                }
                else if (currentState == GameState.Paused)
                {
                    ResumeGame();
                }
            }
        }
        
        private void HandleEscapeKey()
        {
            switch (currentState)
            {
                case GameState.Playing:
                    PauseGame();
                    break;
                    
                case GameState.Paused:
                    ResumeGame();
                    break;
                    
                case GameState.LevelComplete:
                    if (levelController != null)
                    {
                        levelController.RestartLevel();
                        SetGameState(GameState.Playing);
                    }
                    break;
                    
                default:
                    // In other states, try to return to playing
                    if (levelController != null)
                    {
                        levelController.RestartLevel();
                        SetGameState(GameState.Playing);
                    }
                    break;
            }
        }
        
        public void SetGameState(GameState newState)
        {
            if (currentState == newState) return;
            
            GameState previousState = currentState;
            currentState = newState;
            
            OnGameStateChanged?.Invoke(newState);
            
            Debug.Log($"Game state changed: {previousState} -> {newState}");
            
            // Handle state-specific logic
            switch (newState)
            {
                case GameState.Playing:
                    ResumeGame();
                    break;
                    
                case GameState.Paused:
                    PauseGame();
                    break;
                    
                case GameState.LevelComplete:
                    HandleLevelComplete();
                    break;
                    
                case GameState.Exiting:
                    HandleGameExit();
                    break;
            }
        }
        
        public void PauseGame()
        {
            if (isPaused) return;
            
            isPaused = true;
            Time.timeScale = 0f;
            
            // Pause audio
            if (audioManager != null)
            {
                AudioListener.pause = true;
            }
            
            OnGamePaused?.Invoke();
            
            Debug.Log("Game paused");
        }
        
        public void ResumeGame()
        {
            if (!isPaused) return;
            
            isPaused = false;
            Time.timeScale = 1f;
            
            // Resume audio
            if (audioManager != null)
            {
                AudioListener.pause = false;
            }
            
            OnGameResumed?.Invoke();
            
            Debug.Log("Game resumed");
        }
        
        public void ResetGame()
        {
            // Reset all systems
            if (levelController != null)
            {
                levelController.RestartLevel();
            }
            
            if (statisticsManager != null)
            {
                statisticsManager.ResetStatistics();
            }
            
            if (dataLogger != null)
            {
                dataLogger.StartNewSession();
            }
            
            SetGameState(GameState.Playing);
            OnGameReset?.Invoke();
            
            Debug.Log("Game reset");
        }
        
        private void HandleLevelComplete()
        {
            // Level completion is handled by LevelController
            // This is just for state management
            Debug.Log("Level completed - waiting for user input");
        }
        
        private void HandleGameExit()
        {
            // Save any remaining data
            if (dataLogger != null)
            {
                dataLogger.ExportSessionData();
            }
            
            // Cleanup and quit
            StartCoroutine(ExitGameCoroutine());
        }
        
        private IEnumerator ExitGameCoroutine()
        {
            Debug.Log("Exiting game...");
            
            // Wait a frame to ensure data is saved
            yield return null;
            
            QuitGame();
        }
        
        public void QuitGame()
        {
            SetGameState(GameState.Exiting);
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        // Error recovery system
        private void HandleLogMessage(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                consecutiveErrors++;
                
                if (consecutiveErrors >= maxConsecutiveErrors)
                {
                    HandleSystemError($"Too many consecutive errors: {logString}");
                }
            }
            else if (type == LogType.Log)
            {
                // Reset error count on successful operations
                consecutiveErrors = 0;
            }
        }
        
        private void HandleSystemError(string errorMessage)
        {
            Debug.LogWarning($"System error detected: {errorMessage}");
            
            if (errorRecoveryCoroutine == null)
            {
                errorRecoveryCoroutine = StartCoroutine(ErrorRecoveryCoroutine());
            }
        }
        
        private IEnumerator ErrorRecoveryCoroutine()
        {
            Debug.Log("Starting error recovery...");
            
            // Pause the game
            PauseGame();
            
            // Wait a moment
            yield return new WaitForSecondsRealtime(1f);
            
            // Try to recover systems
            try
            {
                // Reset level if possible
                if (levelController != null)
                {
                    levelController.RestartLevel();
                }
                
                // Reset statistics
                if (statisticsManager != null)
                {
                    statisticsManager.ResetStatistics();
                }
                
                consecutiveErrors = 0;
                ResumeGame();
                
                Debug.Log("Error recovery successful");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error recovery failed: {e.Message}");
                
                // If recovery fails, quit gracefully
                QuitGame();
            }
            
            errorRecoveryCoroutine = null;
        }
        
        private void MonitorSystemHealth()
        {
            // Monitor frame rate
            if (Time.frameCount % 60 == 0) // Check once per second at 60 FPS
            {
                float currentFPS = 1f / Time.unscaledDeltaTime;
                
                if (currentFPS < gameConfig.targetFPS * 0.8f) // If FPS drops below 80% of target
                {
                    Debug.LogWarning($"Low FPS detected: {currentFPS:F1} (target: {gameConfig.targetFPS})");
                }
            }
        }
        
        // Public methods for external systems
        public void NotifyLevelComplete()
        {
            SetGameState(GameState.LevelComplete);
        }
        
        public void NotifySystemReady(string systemName)
        {
            Debug.Log($"System ready: {systemName}");
        }
        
        public void NotifySystemError(string systemName, string error)
        {
            Debug.LogError($"System error in {systemName}: {error}");
            HandleSystemError($"{systemName}: {error}");
        }
        
        // Cleanup
        void OnDestroy()
        {
            Application.logMessageReceived -= HandleLogMessage;
        }
        
        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && currentState == GameState.Playing)
            {
                PauseGame();
            }
        }
        
        void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && currentState == GameState.Playing)
            {
                PauseGame();
            }
        }
    }
}