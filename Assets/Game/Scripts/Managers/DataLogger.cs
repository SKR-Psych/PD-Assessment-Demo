using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using SortingBoardGame.Data;

namespace SortingBoardGame.Managers
{
    public class DataLogger : MonoBehaviour
    {
        public static DataLogger Instance { get; private set; }
        
        [Header("Data Export Settings")]
        [SerializeField] private bool enableLogging = true;
        [SerializeField] private bool exportCSV = true;
        [SerializeField] private bool exportJSON = true;
        
        private string sessionId;
        private string logDirectory;
        private List<BallData> sessionData = new List<BallData>();
        
        // CSV headers
        private readonly string csvHeaders = "session_id,trial_id,spawn_time,grasp_time,release_time,completion_time,ball_color,ball_size,target_color,target_size,placement_error_px,outcome";
        
        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDataLogger();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeDataLogger()
        {
            // Generate session ID
            sessionId = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            
            // Create log directory
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string dateFolder = System.DateTime.Now.ToString("yyyy-MM-dd");
            logDirectory = Path.Combine(documentsPath, "SortingBoard", "Logs", dateFolder);
            
            try
            {
                Directory.CreateDirectory(logDirectory);
                Debug.Log($"Data logging initialized - Session: {sessionId} - Directory: {logDirectory}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to create log directory: {e.Message}");
                enableLogging = false;
            }
        }
        
        public void LogBallData(BallData ballData)
        {
            if (!enableLogging || ballData == null) return;
            
            // Ensure session ID is set
            if (string.IsNullOrEmpty(ballData.sessionId))
            {
                ballData.sessionId = sessionId;
            }
            
            // Add to session data
            sessionData.Add(ballData);
            
            Debug.Log($"Logged ball data: Trial {ballData.trialId} - {ballData.ballColor} - {ballData.outcome}");
        }
        
        public void ExportSessionData()
        {
            if (!enableLogging || sessionData.Count == 0) return;
            
            try
            {
                if (exportCSV)
                {
                    ExportToCSV();
                }
                
                if (exportJSON)
                {
                    ExportToJSON();
                }
                
                Debug.Log($"Session data exported successfully - {sessionData.Count} trials");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to export session data: {e.Message}");
            }
        }
        
        private void ExportToCSV()
        {
            string fileName = $"session_{sessionId}.csv";
            string filePath = Path.Combine(logDirectory, fileName);
            
            StringBuilder csvContent = new StringBuilder();
            csvContent.AppendLine(csvHeaders);
            
            foreach (BallData ball in sessionData)
            {
                string csvRow = $"{ball.sessionId}," +
                               $"{ball.trialId}," +
                               $"{ball.spawnTime:F3}," +
                               $"{ball.graspTime:F3}," +
                               $"{ball.releaseTime:F3}," +
                               $"{ball.completionTime:F3}," +
                               $"{ball.ballColor}," +
                               $"{ball.ballSize:F1}," +
                               $"{ball.targetColor}," +
                               $"{ball.targetSize:F1}," +
                               $"{ball.placementErrorPx:F2}," +
                               $"{ball.outcome}";
                
                csvContent.AppendLine(csvRow);
            }
            
            File.WriteAllText(filePath, csvContent.ToString());
            Debug.Log($"CSV exported to: {filePath}");
        }
        
        private void ExportToJSON()
        {
            string fileName = $"session_{sessionId}.json";
            string filePath = Path.Combine(logDirectory, fileName);
            
            // Create JSON structure
            SessionDataExport exportData = new SessionDataExport
            {
                sessionId = sessionId,
                exportTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                totalTrials = sessionData.Count,
                ballData = sessionData
            };
            
            string jsonContent = JsonUtility.ToJson(exportData, true);
            File.WriteAllText(filePath, jsonContent);
            Debug.Log($"JSON exported to: {filePath}");
        }
        
        public void ClearSessionData()
        {
            sessionData.Clear();
            Debug.Log("Session data cleared");
        }
        
        public void StartNewSession()
        {
            // Export current session if it has data
            if (sessionData.Count > 0)
            {
                ExportSessionData();
            }
            
            // Clear and start new session
            ClearSessionData();
            sessionId = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            
            Debug.Log($"New session started: {sessionId}");
        }
        
        // Public properties
        public string CurrentSessionId => sessionId;
        public int TrialCount => sessionData.Count;
        public bool IsLoggingEnabled => enableLogging;
        
        // Auto-export on application quit
        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && sessionData.Count > 0)
            {
                ExportSessionData();
            }
        }
        
        void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && sessionData.Count > 0)
            {
                ExportSessionData();
            }
        }
        
        void OnDestroy()
        {
            if (sessionData.Count > 0)
            {
                ExportSessionData();
            }
        }
    }
    
    [System.Serializable]
    public class SessionDataExport
    {
        public string sessionId;
        public string exportTime;
        public int totalTrials;
        public List<BallData> ballData;
    }
}