using System.Collections.Generic;
using UnityEngine;
using SortingBoardGame.Data;
using SortingBoardGame.Gameplay;

namespace SortingBoardGame.Managers
{
    public class StatisticsManager : MonoBehaviour
    {
        public static StatisticsManager Instance { get; private set; }
        
        [Header("Statistics Tracking")]
        [SerializeField] private List<BallData> completedBalls = new List<BallData>();
        
        // Real-time statistics
        private int totalBallsPlaced = 0;
        private int successfulPlacements = 0;
        private int failedPlacements = 0;
        private float totalPlacementError = 0f;
        private float totalCompletionTime = 0f;
        private float levelStartTime = 0f;
        
        // Events for HUD updates
        public System.Action<float> OnAccuracyUpdated;
        public System.Action<float> OnAverageErrorUpdated;
        public System.Action<float> OnAverageTimeUpdated;
        public System.Action<float> OnThroughputUpdated;
        public System.Action<int, int> OnPlacementCountUpdated; // (successful, total)
        
        // Public properties for real-time access
        public float AccuracyPercentage => totalBallsPlaced > 0 ? (successfulPlacements / (float)totalBallsPlaced) * 100f : 0f;
        public float AverageErrorPx => totalBallsPlaced > 0 ? totalPlacementError / totalBallsPlaced : 0f;
        public float AverageCompletionTime => totalBallsPlaced > 0 ? totalCompletionTime / totalBallsPlaced : 0f;
        public float ObjectsPerMinute => GetObjectsPerMinute();
        public int TotalBallsPlaced => totalBallsPlaced;
        public int SuccessfulPlacements => successfulPlacements;
        public int FailedPlacements => failedPlacements;
        
        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            levelStartTime = Time.time;
            Debug.Log("StatisticsManager initialized - tracking started");
        }
        
        public void RecordBallPlacement(Ball ball)
        {
            if (ball == null || ball.Data == null) return;
            
            BallData ballData = ball.Data;
            
            // Add to completed balls list
            completedBalls.Add(ballData);
            
            // Update counters
            totalBallsPlaced++;
            totalPlacementError += ballData.placementErrorPx;
            totalCompletionTime += ballData.completionTime;
            
            if (ballData.outcome == PlacementOutcome.Success)
            {
                successfulPlacements++;
            }
            else
            {
                failedPlacements++;
            }
            
            // Log data for export
            if (DataLogger.Instance != null)
            {
                DataLogger.Instance.LogBallData(ballData);
            }
            
            // Trigger HUD updates
            UpdateHUD();
            
            Debug.Log($"Recorded ball placement: {ball.BallId} - {ballData.outcome} - Error: {ballData.placementErrorPx:F1}px - Time: {ballData.completionTime:F2}s");
        }
        
        private void UpdateHUD()
        {
            // Trigger all HUD update events
            OnAccuracyUpdated?.Invoke(AccuracyPercentage);
            OnAverageErrorUpdated?.Invoke(AverageErrorPx);
            OnAverageTimeUpdated?.Invoke(AverageCompletionTime);
            OnThroughputUpdated?.Invoke(ObjectsPerMinute);
            OnPlacementCountUpdated?.Invoke(successfulPlacements, totalBallsPlaced);
        }
        
        private float GetObjectsPerMinute()
        {
            float elapsedTime = Time.time - levelStartTime;
            if (elapsedTime > 0)
            {
                return (totalBallsPlaced / elapsedTime) * 60f;
            }
            return 0f;
        }
        
        public void ResetStatistics()
        {
            completedBalls.Clear();
            totalBallsPlaced = 0;
            successfulPlacements = 0;
            failedPlacements = 0;
            totalPlacementError = 0f;
            totalCompletionTime = 0f;
            levelStartTime = Time.time;
            
            UpdateHUD();
            
            Debug.Log("Statistics reset");
        }
        
        public StatisticsSummary GetLevelSummary()
        {
            return new StatisticsSummary
            {
                accuracyPercentage = AccuracyPercentage,
                meanPlacementError = AverageErrorPx,
                meanCompletionTime = AverageCompletionTime,
                totalThroughput = ObjectsPerMinute,
                totalBalls = totalBallsPlaced,
                successfulBalls = successfulPlacements,
                failedBalls = failedPlacements,
                totalLevelTime = Time.time - levelStartTime
            };
        }
        
        public List<BallData> GetAllBallData()
        {
            return new List<BallData>(completedBalls);
        }
        
        // Method to get statistics for specific time periods
        public StatisticsSummary GetStatisticsForLastNBalls(int n)
        {
            if (completedBalls.Count == 0) return new StatisticsSummary();
            
            int startIndex = Mathf.Max(0, completedBalls.Count - n);
            List<BallData> recentBalls = completedBalls.GetRange(startIndex, completedBalls.Count - startIndex);
            
            int successful = 0;
            float totalError = 0f;
            float totalTime = 0f;
            
            foreach (BallData ball in recentBalls)
            {
                if (ball.outcome == PlacementOutcome.Success)
                    successful++;
                totalError += ball.placementErrorPx;
                totalTime += ball.completionTime;
            }
            
            int total = recentBalls.Count;
            
            return new StatisticsSummary
            {
                accuracyPercentage = total > 0 ? (successful / (float)total) * 100f : 0f,
                meanPlacementError = total > 0 ? totalError / total : 0f,
                meanCompletionTime = total > 0 ? totalTime / total : 0f,
                totalBalls = total,
                successfulBalls = successful,
                failedBalls = total - successful
            };
        }
    }
    
    [System.Serializable]
    public class StatisticsSummary
    {
        public float accuracyPercentage;
        public float meanPlacementError;
        public float meanCompletionTime;
        public float totalThroughput;
        public int totalBalls;
        public int successfulBalls;
        public int failedBalls;
        public float totalLevelTime;
        
        public override string ToString()
        {
            return $"Accuracy: {accuracyPercentage:F1}% | " +
                   $"Avg Error: {meanPlacementError:F1}px | " +
                   $"Avg Time: {meanCompletionTime:F2}s | " +
                   $"Throughput: {totalThroughput:F1}/min | " +
                   $"Success: {successfulBalls}/{totalBalls}";
        }
    }
}