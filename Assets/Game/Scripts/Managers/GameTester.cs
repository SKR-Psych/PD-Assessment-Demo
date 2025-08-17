using UnityEngine;
using System.Collections;
using SortingBoardGame.Gameplay;

namespace SortingBoardGame.Managers
{
    public class GameTester : MonoBehaviour
    {
        [Header("Testing Configuration")]
        [SerializeField] private bool enableAutoTesting = false;
        [SerializeField] private bool testOnStart = false;
        [SerializeField] private float testDuration = 30f;
        
        [Header("Test Results")]
        [SerializeField] private bool lastTestPassed = false;
        [SerializeField] private float lastTestMinFPS = 0f;
        [SerializeField] private float lastTestAvgFPS = 0f;
        
        void Start()
        {
            if (testOnStart)
            {
                StartCoroutine(DelayedAutoTest());
            }
        }
        
        void Update()
        {
            // Manual testing hotkeys
            if (Input.GetKeyDown(KeyCode.F1))
            {
                StartPerformanceTest();
            }
            
            if (Input.GetKeyDown(KeyCode.F2))
            {
                StartGameplayTest();
            }
            
            if (Input.GetKeyDown(KeyCode.F3))
            {
                StartFullSystemTest();
            }
        }
        
        private IEnumerator DelayedAutoTest()
        {
            // Wait for all systems to initialize
            yield return new WaitForSeconds(3f);
            
            if (enableAutoTesting)
            {
                StartFullSystemTest();
            }
        }
        
        public void StartPerformanceTest()
        {
            StartCoroutine(PerformanceTestCoroutine());
        }
        
        public void StartGameplayTest()
        {
            StartCoroutine(GameplayTestCoroutine());
        }
        
        public void StartFullSystemTest()
        {
            StartCoroutine(FullSystemTestCoroutine());
        }
        
        private IEnumerator PerformanceTestCoroutine()
        {
            Debug.Log("=== PERFORMANCE TEST STARTED ===");
            
            // Ensure performance manager is active
            if (PerformanceManager.Instance == null)
            {
                Debug.LogError("PerformanceManager not found - test aborted");
                yield break;
            }
            
            // Run performance test with full load
            PerformanceManager.Instance.TestPerformanceWithFullLoad();
            
            // Monitor for test duration
            float startTime = Time.time;
            float minFPS = float.MaxValue;
            float maxFPS = 0f;
            float totalFPS = 0f;
            int frameCount = 0;
            
            while (Time.time - startTime < testDuration)
            {
                float currentFPS = PerformanceManager.Instance.GetCurrentFPS();
                
                if (currentFPS < minFPS) minFPS = currentFPS;
                if (currentFPS > maxFPS) maxFPS = currentFPS;
                totalFPS += currentFPS;
                frameCount++;
                
                yield return new WaitForSeconds(0.1f);
            }
            
            float avgFPS = totalFPS / frameCount;
            lastTestMinFPS = minFPS;
            lastTestAvgFPS = avgFPS;
            lastTestPassed = minFPS >= 48f; // 80% of 60 FPS target
            
            Debug.Log($"=== PERFORMANCE TEST RESULTS ===");
            Debug.Log($"Duration: {testDuration}s");
            Debug.Log($"Min FPS: {minFPS:F1}");
            Debug.Log($"Max FPS: {maxFPS:F1}");
            Debug.Log($"Avg FPS: {avgFPS:F1}");
            Debug.Log($"Test Result: {(lastTestPassed ? "PASSED" : "FAILED")}");
            Debug.Log($"=== END PERFORMANCE TEST ===");
        }
        
        private IEnumerator GameplayTestCoroutine()
        {
            Debug.Log("=== GAMEPLAY TEST STARTED ===");
            
            // Test ball spawning
            BallSpawner ballSpawner = FindObjectOfType<BallSpawner>();
            if (ballSpawner == null)
            {
                Debug.LogError("BallSpawner not found - test aborted");
                yield break;
            }
            
            // Test mouse interaction
            MouseInteractionSystem mouseSystem = FindObjectOfType<MouseInteractionSystem>();
            if (mouseSystem == null)
            {
                Debug.LogError("MouseInteractionSystem not found - test aborted");
                yield break;
            }
            
            // Test hole system
            HoleManager holeManager = FindObjectOfType<HoleManager>();
            if (holeManager == null)
            {
                Debug.LogError("HoleManager not found - test aborted");
                yield break;
            }
            
            Debug.Log("✓ All gameplay systems found");
            
            // Test ball spawning
            if (!ballSpawner.IsSpawning)
            {
                ballSpawner.StartSpawning();
            }
            
            yield return new WaitForSeconds(5f);
            
            // Check if balls are spawning
            if (ballSpawner.BallsSpawned > 0)
            {
                Debug.Log($"✓ Ball spawning working - {ballSpawner.BallsSpawned} balls spawned");
            }
            else
            {
                Debug.LogError("✗ Ball spawning failed");
            }
            
            // Test hole system
            var holes = holeManager.GetAllHoles();
            if (holes.Count == 3)
            {
                Debug.Log("✓ Hole system working - 3 holes found");
            }
            else
            {
                Debug.LogError($"✗ Hole system failed - {holes.Count} holes found (expected 3)");
            }
            
            Debug.Log("=== GAMEPLAY TEST COMPLETE ===");
        }
        
        private IEnumerator FullSystemTestCoroutine()
        {
            Debug.Log("=== FULL SYSTEM TEST STARTED ===");
            
            // Test all managers
            yield return StartCoroutine(TestAllManagers());
            
            // Test gameplay systems
            yield return StartCoroutine(GameplayTestCoroutine());
            
            // Test performance
            yield return StartCoroutine(PerformanceTestCoroutine());
            
            // Test data logging
            yield return StartCoroutine(TestDataLogging());
            
            // Test audio system
            yield return StartCoroutine(TestAudioSystem());
            
            Debug.Log("=== FULL SYSTEM TEST COMPLETE ===");
        }
        
        private IEnumerator TestAllManagers()
        {
            Debug.Log("Testing all manager systems...");
            
            // Test GameManager
            if (GameManager.Instance != null)
            {
                Debug.Log("✓ GameManager found");
            }
            else
            {
                Debug.LogError("✗ GameManager not found");
            }
            
            // Test AudioManager
            if (AudioManager.Instance != null)
            {
                Debug.Log("✓ AudioManager found");
            }
            else
            {
                Debug.LogError("✗ AudioManager not found");
            }
            
            // Test StatisticsManager
            if (StatisticsManager.Instance != null)
            {
                Debug.Log("✓ StatisticsManager found");
            }
            else
            {
                Debug.LogError("✗ StatisticsManager not found");
            }
            
            // Test DataLogger
            if (DataLogger.Instance != null)
            {
                Debug.Log("✓ DataLogger found");
            }
            else
            {
                Debug.LogError("✗ DataLogger not found");
            }
            
            // Test PerformanceManager
            if (PerformanceManager.Instance != null)
            {
                Debug.Log("✓ PerformanceManager found");
            }
            else
            {
                Debug.LogError("✗ PerformanceManager not found");
            }
            
            yield return null;
        }
        
        private IEnumerator TestDataLogging()
        {
            Debug.Log("Testing data logging system...");
            
            if (DataLogger.Instance != null)
            {
                if (DataLogger.Instance.IsLoggingEnabled)
                {
                    Debug.Log("✓ Data logging enabled");
                }
                else
                {
                    Debug.LogWarning("⚠ Data logging disabled");
                }
                
                Debug.Log($"✓ Current session: {DataLogger.Instance.CurrentSessionId}");
                Debug.Log($"✓ Trial count: {DataLogger.Instance.TrialCount}");
            }
            
            yield return null;
        }
        
        private IEnumerator TestAudioSystem()
        {
            Debug.Log("Testing audio system...");
            
            if (AudioManager.Instance != null)
            {
                // Test success sound
                AudioManager.Instance.PlaySuccessSound(Vector3.zero);
                yield return new WaitForSeconds(0.5f);
                
                // Test failure sound
                AudioManager.Instance.PlayFailureSound(Vector3.zero);
                yield return new WaitForSeconds(0.5f);
                
                // Test roll sound
                AudioManager.Instance.PlayBallRollSound(Vector3.zero);
                yield return new WaitForSeconds(0.5f);
                
                Debug.Log("✓ Audio system test complete");
            }
            else
            {
                Debug.LogError("✗ AudioManager not found");
            }
        }
        
        // Public methods for external testing
        public bool RunQuickTest()
        {
            // Quick validation of all systems
            bool allSystemsReady = true;
            
            if (GameManager.Instance == null) allSystemsReady = false;
            if (AudioManager.Instance == null) allSystemsReady = false;
            if (StatisticsManager.Instance == null) allSystemsReady = false;
            if (DataLogger.Instance == null) allSystemsReady = false;
            if (PerformanceManager.Instance == null) allSystemsReady = false;
            
            Debug.Log($"Quick test result: {(allSystemsReady ? "PASSED" : "FAILED")}");
            return allSystemsReady;
        }
        
        public void LogSystemStatus()
        {
            Debug.Log("=== SYSTEM STATUS ===");
            Debug.Log($"GameManager: {(GameManager.Instance != null ? "✓" : "✗")}");
            Debug.Log($"AudioManager: {(AudioManager.Instance != null ? "✓" : "✗")}");
            Debug.Log($"StatisticsManager: {(StatisticsManager.Instance != null ? "✓" : "✗")}");
            Debug.Log($"DataLogger: {(DataLogger.Instance != null ? "✓" : "✗")}");
            Debug.Log($"PerformanceManager: {(PerformanceManager.Instance != null ? "✓" : "✗")}");
            
            if (PerformanceManager.Instance != null)
            {
                PerformanceStats stats = PerformanceManager.Instance.GetPerformanceStats();
                Debug.Log($"Performance: {stats}");
            }
            Debug.Log("=== END STATUS ===");
        }
    }
}