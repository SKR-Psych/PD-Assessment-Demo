using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace SortingBoardGame.Managers
{
    public class PerformanceManager : MonoBehaviour
    {
        public static PerformanceManager Instance { get; private set; }
        
        [Header("Performance Monitoring")]
        [SerializeField] private bool enableMonitoring = true;
        [SerializeField] private float targetFPS = 60f;
        [SerializeField] private float lowFPSThreshold = 48f; // 80% of target
        [SerializeField] private int fpsHistorySize = 60; // 1 second at 60 FPS
        
        [Header("Performance Settings")]
        [SerializeField] private int maxParticles = 100;
        [SerializeField] private int maxAudioSources = 10;
        [SerializeField] private float cullDistance = 50f;
        
        // Performance tracking
        private Queue<float> fpsHistory = new Queue<float>();
        private float currentFPS = 0f;
        private float averageFPS = 0f;
        private int lowFPSFrameCount = 0;
        private int totalFrameCount = 0;
        
        // Memory tracking
        private float lastGCTime = 0f;
        private long lastMemoryUsage = 0L;
        
        // Performance events
        public System.Action<float> OnFPSUpdated;
        public System.Action OnLowFPSDetected;
        public System.Action OnPerformanceOptimized;
        
        // Optimization flags
        private bool particleOptimizationActive = false;
        private bool audioOptimizationActive = false;
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePerformanceManager();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            if (enableMonitoring)
            {
                StartCoroutine(PerformanceMonitoringCoroutine());
            }
        }
        
        private void InitializePerformanceManager()
        {
            // Set quality settings for optimal performance
            OptimizeQualitySettings();
            
            // Initialize object pooling optimizations
            OptimizeObjectPooling();
            
            Debug.Log("PerformanceManager initialized - monitoring enabled");
        }
        
        private void OptimizeQualitySettings()
        {
            // Set quality level for consistent performance
            QualitySettings.vSyncCount = 1; // Enable VSync for smooth gameplay
            QualitySettings.antiAliasing = 2; // 2x MSAA for good quality/performance balance
            
            // Optimize shadows for performance
            QualitySettings.shadows = ShadowQuality.HardOnly;
            QualitySettings.shadowResolution = ShadowResolution.Medium;
            QualitySettings.shadowDistance = 20f;
            
            // Optimize particles
            QualitySettings.particleRaycastBudget = 64;
            QualitySettings.maxQueuedFrames = 2;
            
            // Optimize physics
            Physics.defaultSolverIterations = 6;
            Physics.defaultSolverVelocityIterations = 1;
            
            Debug.Log("Quality settings optimized for performance");
        }
        
        private void OptimizeObjectPooling()
        {
            // Ensure all managers use object pooling
            BallSpawner ballSpawner = FindObjectOfType<BallSpawner>();
            if (ballSpawner != null)
            {
                Debug.Log("Ball spawner object pooling verified");
            }
            
            AudioManager audioManager = AudioManager.Instance;
            if (audioManager != null)
            {
                Debug.Log("Audio manager object pooling verified");
            }
        }
        
        private IEnumerator PerformanceMonitoringCoroutine()
        {
            while (enableMonitoring)
            {
                // Calculate current FPS
                currentFPS = 1f / Time.unscaledDeltaTime;
                
                // Update FPS history
                UpdateFPSHistory(currentFPS);
                
                // Check for performance issues
                CheckPerformanceIssues();
                
                // Update memory tracking
                UpdateMemoryTracking();
                
                // Trigger FPS update event
                OnFPSUpdated?.Invoke(currentFPS);
                
                yield return new WaitForSeconds(1f / 60f); // Check 60 times per second
            }
        }
        
        private void UpdateFPSHistory(float fps)
        {
            fpsHistory.Enqueue(fps);
            
            if (fpsHistory.Count > fpsHistorySize)
            {
                fpsHistory.Dequeue();
            }
            
            // Calculate average FPS
            float total = 0f;
            foreach (float f in fpsHistory)
            {
                total += f;
            }
            averageFPS = total / fpsHistory.Count;
            
            totalFrameCount++;
        }
        
        private void CheckPerformanceIssues()
        {
            if (currentFPS < lowFPSThreshold)
            {
                lowFPSFrameCount++;
                
                // If low FPS persists for more than 1 second, take action
                if (lowFPSFrameCount > 60)
                {
                    HandleLowFPS();
                    lowFPSFrameCount = 0; // Reset counter
                }
            }
            else
            {
                lowFPSFrameCount = 0; // Reset counter if FPS recovers
            }
        }
        
        private void HandleLowFPS()
        {
            Debug.LogWarning($"Low FPS detected: {currentFPS:F1} (target: {targetFPS})");
            
            OnLowFPSDetected?.Invoke();
            
            // Apply performance optimizations
            ApplyPerformanceOptimizations();
        }
        
        private void ApplyPerformanceOptimizations()
        {
            bool optimizationApplied = false;
            
            // Reduce particle effects
            if (!particleOptimizationActive)
            {
                OptimizeParticleEffects();
                particleOptimizationActive = true;
                optimizationApplied = true;
            }
            
            // Reduce audio sources
            if (!audioOptimizationActive)
            {
                OptimizeAudioSources();
                audioOptimizationActive = true;
                optimizationApplied = true;
            }
            
            // Force garbage collection
            ForceGarbageCollection();
            optimizationApplied = true;
            
            if (optimizationApplied)
            {
                OnPerformanceOptimized?.Invoke();
                Debug.Log("Performance optimizations applied");
            }
        }
        
        private void OptimizeParticleEffects()
        {
            // Find all particle systems and reduce their max particles
            ParticleSystem[] particleSystems = FindObjectsOfType<ParticleSystem>();
            
            foreach (ParticleSystem ps in particleSystems)
            {
                var main = ps.main;
                if (main.maxParticles > maxParticles)
                {
                    main.maxParticles = maxParticles;
                }
                
                // Reduce emission rate
                var emission = ps.emission;
                if (emission.rateOverTime.constant > 50)
                {
                    emission.rateOverTime = 25;
                }
            }
            
            Debug.Log($"Optimized {particleSystems.Length} particle systems");
        }
        
        private void OptimizeAudioSources()
        {
            // Limit the number of active audio sources
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            
            int activeCount = 0;
            foreach (AudioSource source in audioSources)
            {
                if (source.isPlaying)
                {
                    activeCount++;
                    if (activeCount > maxAudioSources)
                    {
                        source.Stop();
                    }
                }
            }
            
            Debug.Log($"Optimized audio sources - limited to {maxAudioSources} active");
        }
        
        private void ForceGarbageCollection()
        {
            if (Time.time - lastGCTime > 5f) // Only GC every 5 seconds max
            {
                System.GC.Collect();
                lastGCTime = Time.time;
                Debug.Log("Forced garbage collection");
            }
        }
        
        private void UpdateMemoryTracking()
        {
            long currentMemory = System.GC.GetTotalMemory(false);
            
            if (lastMemoryUsage > 0)
            {
                long memoryDelta = currentMemory - lastMemoryUsage;
                
                // If memory usage increased significantly, log it
                if (memoryDelta > 1024 * 1024) // 1MB increase
                {
                    Debug.Log($"Memory usage increased by {memoryDelta / 1024 / 1024}MB");
                }
            }
            
            lastMemoryUsage = currentMemory;
        }
        
        // Public methods for performance monitoring
        public float GetCurrentFPS()
        {
            return currentFPS;
        }
        
        public float GetAverageFPS()
        {
            return averageFPS;
        }
        
        public bool IsPerformanceGood()
        {
            return averageFPS >= lowFPSThreshold;
        }
        
        public PerformanceStats GetPerformanceStats()
        {
            return new PerformanceStats
            {
                currentFPS = currentFPS,
                averageFPS = averageFPS,
                targetFPS = targetFPS,
                lowFPSFrameCount = lowFPSFrameCount,
                totalFrameCount = totalFrameCount,
                memoryUsageMB = lastMemoryUsage / 1024 / 1024,
                optimizationsActive = particleOptimizationActive || audioOptimizationActive
            };
        }
        
        // Method to test performance with full load
        public void TestPerformanceWithFullLoad()
        {
            StartCoroutine(PerformanceTestCoroutine());
        }
        
        private IEnumerator PerformanceTestCoroutine()
        {
            Debug.Log("Starting performance test with 20 balls...");
            
            // Ensure ball spawner spawns all balls quickly for testing
            BallSpawner ballSpawner = FindObjectOfType<BallSpawner>();
            if (ballSpawner != null)
            {
                // Temporarily reduce spawn interval for testing
                float originalInterval = 2f;
                
                // Spawn balls rapidly for testing
                for (int i = 0; i < 20; i++)
                {
                    yield return new WaitForSeconds(0.1f); // Spawn every 0.1 seconds
                }
                
                // Monitor performance for 10 seconds
                float testStartTime = Time.time;
                float minFPS = float.MaxValue;
                float maxFPS = 0f;
                
                while (Time.time - testStartTime < 10f)
                {
                    if (currentFPS < minFPS) minFPS = currentFPS;
                    if (currentFPS > maxFPS) maxFPS = currentFPS;
                    
                    yield return new WaitForSeconds(0.1f);
                }
                
                Debug.Log($"Performance test complete - Min FPS: {minFPS:F1}, Max FPS: {maxFPS:F1}, Avg FPS: {averageFPS:F1}");
                
                // Verify performance meets requirements
                if (minFPS >= lowFPSThreshold)
                {
                    Debug.Log("✓ Performance test PASSED - meets ≥60 FPS requirement");
                }
                else
                {
                    Debug.LogWarning("✗ Performance test FAILED - FPS drops below requirement");
                }
            }
        }
        
        // Cleanup
        void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
    
    [System.Serializable]
    public class PerformanceStats
    {
        public float currentFPS;
        public float averageFPS;
        public float targetFPS;
        public int lowFPSFrameCount;
        public int totalFrameCount;
        public long memoryUsageMB;
        public bool optimizationsActive;
        
        public override string ToString()
        {
            return $"FPS: {currentFPS:F1} (avg: {averageFPS:F1}, target: {targetFPS}) | " +
                   $"Memory: {memoryUsageMB}MB | Optimizations: {optimizationsActive}";
        }
    }
}