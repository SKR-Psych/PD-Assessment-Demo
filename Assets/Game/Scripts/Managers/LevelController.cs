using UnityEngine;
using SortingBoardGame.Data;
using SortingBoardGame.Gameplay;

namespace SortingBoardGame.Managers
{
    public class LevelController : MonoBehaviour
    {
        [Header("Level Configuration")]
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private BallSpawner ballSpawner;
        [SerializeField] private BallInteractionHandler interactionHandler;
        
        [Header("Level State")]
        [SerializeField] private bool levelActive = false;
        [SerializeField] private bool levelCompleted = false;
        
        // Level events
        public System.Action OnLevelStarted;
        public System.Action OnLevelCompleted;
        public System.Action<StatisticsSummary> OnLevelSummaryReady;
        
        void Start()
        {
            InitializeLevel();
            
            // Auto-start after a short delay
            Invoke(nameof(StartLevel), 1f);
        }
        
        void Update()
        {
            if (levelActive && !levelCompleted)
            {
                CheckLevelCompletion();
            }
            
            // Handle Escape key for reset/exit
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleEscapeKey();
            }
        }
        
        private void InitializeLevel()
        {
            if (gameConfig == null)
            {
                gameConfig = GameManager.Instance?.Config;
            }
            
            if (gameConfig == null)
            {
                Debug.LogError("GameConfig not assigned to LevelController!");
                return;
            }
            
            if (ballSpawner == null)
                ballSpawner = FindObjectOfType<BallSpawner>();
            
            if (interactionHandler == null)
                interactionHandler = FindObjectOfType<BallInteractionHandler>();
            
            Debug.Log("Level Controller initialized for Level 1");
        }
        
        public void StartLevel()
        {
            if (levelActive)
            {
                Debug.LogWarning("Level is already active!");
                return;
            }
            
            levelActive = true;
            levelCompleted = false;
            
            // Reset statistics
            if (StatisticsManager.Instance != null)
            {
                StatisticsManager.Instance.ResetStatistics();
            }
            
            // Start new data logging session
            if (DataLogger.Instance != null)
            {
                DataLogger.Instance.StartNewSession();
            }
            
            // Start ball spawning
            if (ballSpawner != null)
            {
                ballSpawner.StartSpawning();
            }
            
            OnLevelStarted?.Invoke();
            
            Debug.Log($"Level 1 started - Target: {gameConfig.totalBalls} balls");
        }
        
        private void CheckLevelCompletion()
        {
            if (interactionHandler == null) return;
            
            // Check if all balls are placed
            bool allBallsPlaced = interactionHandler.AreAllBallsPlaced();
            
            if (allBallsPlaced)
            {
                CompleteLevel();
            }
        }
        
        private void CompleteLevel()
        {
            if (levelCompleted) return;
            
            levelCompleted = true;
            levelActive = false;
            
            // Stop ball spawning
            if (ballSpawner != null)
            {
                ballSpawner.StopSpawning();
            }
            
            // Get final statistics
            StatisticsSummary summary = null;
            if (StatisticsManager.Instance != null)
            {
                summary = StatisticsManager.Instance.GetLevelSummary();
            }
            
            // Export data
            if (DataLogger.Instance != null)
            {
                DataLogger.Instance.ExportSessionData();
            }
            
            // Notify GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.NotifyLevelComplete();
            }
            
            OnLevelCompleted?.Invoke();
            OnLevelSummaryReady?.Invoke(summary);
            
            Debug.Log($"Level 1 completed! Summary: {summary}");
            
            // Show completion UI after a short delay
            Invoke(nameof(ShowCompletionSummary), 2f);
        }
        
        private void ShowCompletionSummary()
        {
            if (StatisticsManager.Instance != null)
            {
                StatisticsSummary summary = StatisticsManager.Instance.GetLevelSummary();
                
                string summaryText = $"Level 1 Complete!\n\n" +
                                   $"Accuracy: {summary.accuracyPercentage:F1}%\n" +
                                   $"Average Error: {summary.meanPlacementError:F1}px\n" +
                                   $"Average Time: {summary.meanCompletionTime:F2}s\n" +
                                   $"Throughput: {summary.totalThroughput:F1} balls/min\n" +
                                   $"Success: {summary.successfulBalls}/{summary.totalBalls}\n" +
                                   $"Total Time: {summary.totalLevelTime:F1}s";
                
                Debug.Log(summaryText);
                
                // TODO: Show UI popup with summary
                // For now, just log to console
            }
        }
        
        public void RestartLevel()
        {
            if (levelActive)
            {
                // Stop current level
                levelActive = false;
                if (ballSpawner != null)
                {
                    ballSpawner.StopSpawning();
                    ballSpawner.ClearAllBalls();
                }
            }
            
            levelCompleted = false;
            
            // Start new level
            StartLevel();
            
            Debug.Log("Level restarted");
        }
        
        public void ExitLevel()
        {
            if (levelActive)
            {
                levelActive = false;
                if (ballSpawner != null)
                {
                    ballSpawner.StopSpawning();
                }
                
                // Export any remaining data
                if (DataLogger.Instance != null)
                {
                    DataLogger.Instance.ExportSessionData();
                }
            }
            
            Debug.Log("Level exited");
        }
        
        // Handle Escape key for reset/exit (integrated into main Update method)
        
        private void HandleEscapeKey()
        {
            if (levelCompleted)
            {
                // If level is completed, restart
                RestartLevel();
            }
            else if (levelActive)
            {
                // If level is active, show pause/exit options
                // For now, just restart
                RestartLevel();
            }
            else
            {
                // If no level active, start level
                StartLevel();
            }
        }
        
        // Public properties
        public bool IsLevelActive => levelActive;
        public bool IsLevelCompleted => levelCompleted;
        public int TargetBallCount => gameConfig != null ? gameConfig.totalBalls : 20;
        
        // Auto-start level on scene load (integrated into main Start method)
    }
}