using UnityEngine;
using SortingBoardGame.Managers;
using SortingBoardGame.Setup;

namespace SortingBoardGame
{
    /// <summary>
    /// Main game initializer - attach this to a GameObject in your scene to set up the entire game
    /// </summary>
    public class GameInitializer : MonoBehaviour
    {
        [Header("Initialization Settings")]
        [SerializeField] private bool initializeOnStart = true;
        [SerializeField] private bool useQuickSetup = true;
        [SerializeField] private bool enableTesting = false;
        [SerializeField] private bool enablePerformanceMonitoring = true;
        
        [Header("Debug Options")]
        [SerializeField] private bool showDebugLogs = true;
        [SerializeField] private bool runCompilationTest = false;
        
        void Start()
        {
            if (initializeOnStart)
            {
                InitializeGame();
            }
        }
        
        public void InitializeGame()
        {
            if (showDebugLogs)
            {
                Debug.Log("=== SORTING BOARD GAME INITIALIZATION ===");
            }
            
            // Run compilation test first if enabled
            if (runCompilationTest)
            {
                RunCompilationTest();
            }
            
            // Use quick setup if enabled
            if (useQuickSetup)
            {
                SetupGameQuickly();
            }
            else
            {
                SetupGameManually();
            }
            
            // Enable performance monitoring if requested
            if (enablePerformanceMonitoring)
            {
                EnablePerformanceMonitoring();
            }
            
            // Enable testing components if requested
            if (enableTesting)
            {
                EnableTestingComponents();
            }
            
            if (showDebugLogs)
            {
                Debug.Log("=== GAME INITIALIZATION COMPLETE ===");
                LogSystemStatus();
            }
        }
        
        private void RunCompilationTest()
        {
            GameObject testObj = new GameObject("CompilationTest");
            testObj.AddComponent<CompilationTest>();
            
            // Destroy after test
            Destroy(testObj, 2f);
        }
        
        private void SetupGameQuickly()
        {
            // Use QuickGameSetup for rapid initialization
            GameObject setupObj = new GameObject("QuickGameSetup");
            QuickGameSetup quickSetup = setupObj.AddComponent<QuickGameSetup>();
            quickSetup.SetupGame();
            
            // Clean up setup object after use
            Destroy(setupObj, 1f);
        }
        
        private void SetupGameManually()
        {
            // Use AutoSetup for more controlled initialization
            GameObject setupObj = new GameObject("AutoSetup");
            AutoSetup autoSetup = setupObj.AddComponent<AutoSetup>();
            autoSetup.SetupGame();
            
            // Clean up setup object after use
            Destroy(setupObj, 1f);
        }
        
        private void EnablePerformanceMonitoring()
        {
            if (PerformanceManager.Instance == null)
            {
                GameObject perfObj = new GameObject("PerformanceManager");
                perfObj.AddComponent<PerformanceManager>();
            }
        }
        
        private void EnableTestingComponents()
        {
            // Add GameTester
            if (FindObjectOfType<GameTester>() == null)
            {
                GameObject testerObj = new GameObject("GameTester");
                GameTester tester = testerObj.AddComponent<GameTester>();
                
                // Run a quick test after initialization
                Invoke(nameof(RunQuickTest), 3f);
            }
        }
        
        private void RunQuickTest()
        {
            GameTester tester = FindObjectOfType<GameTester>();
            if (tester != null)
            {
                tester.RunQuickTest();
            }
        }
        
        private void LogSystemStatus()
        {
            Debug.Log("=== SYSTEM STATUS ===");
            Debug.Log($"GameManager: {(GameManager.Instance != null ? "✓ Ready" : "✗ Missing")}");
            Debug.Log($"AudioManager: {(AudioManager.Instance != null ? "✓ Ready" : "✗ Missing")}");
            Debug.Log($"StatisticsManager: {(StatisticsManager.Instance != null ? "✓ Ready" : "✗ Missing")}");
            Debug.Log($"DataLogger: {(DataLogger.Instance != null ? "✓ Ready" : "✗ Missing")}");
            Debug.Log($"PerformanceManager: {(PerformanceManager.Instance != null ? "✓ Ready" : "✗ Missing")}");
            
            // Check scene components
            Debug.Log($"BallSpawner: {(FindObjectOfType<BallSpawner>() != null ? "✓ Ready" : "✗ Missing")}");
            Debug.Log($"HoleManager: {(FindObjectOfType<HoleManager>() != null ? "✓ Ready" : "✗ Missing")}");
            Debug.Log($"MouseInteractionSystem: {(FindObjectOfType<MouseInteractionSystem>() != null ? "✓ Ready" : "✗ Missing")}");
            Debug.Log($"LevelController: {(FindObjectOfType<LevelController>() != null ? "✓ Ready" : "✗ Missing")}");
            
            Debug.Log("=== END STATUS ===");
        }
        
        // Public methods for manual control
        [ContextMenu("Initialize Game")]
        public void InitializeGameFromMenu()
        {
            InitializeGame();
        }
        
        [ContextMenu("Test All Systems")]
        public void TestAllSystemsFromMenu()
        {
            GameTester tester = FindObjectOfType<GameTester>();
            if (tester != null)
            {
                tester.StartFullSystemTest();
            }
            else
            {
                Debug.LogWarning("GameTester not found. Enable testing components first.");
            }
        }
        
        [ContextMenu("Log System Status")]
        public void LogSystemStatusFromMenu()
        {
            LogSystemStatus();
        }
        
        // Hotkey support
        void Update()
        {
            // F5 - Reinitialize game
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Debug.Log("F5 pressed - Reinitializing game...");
                InitializeGame();
            }
            
            // F6 - Log system status
            if (Input.GetKeyDown(KeyCode.F6))
            {
                LogSystemStatus();
            }
            
            // F7 - Run performance test
            if (Input.GetKeyDown(KeyCode.F7))
            {
                if (PerformanceManager.Instance != null)
                {
                    PerformanceManager.Instance.TestPerformanceWithFullLoad();
                }
            }
        }
    }
}