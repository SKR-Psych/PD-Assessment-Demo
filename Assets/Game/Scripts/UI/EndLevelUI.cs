using UnityEngine;
using UnityEngine.UI;
using SortingBoardGame.Managers;

namespace SortingBoardGame.UI
{
    public class EndLevelUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject summaryPanel;
        [SerializeField] private Text titleText;
        [SerializeField] private Text accuracyText;
        [SerializeField] private Text errorText;
        [SerializeField] private Text timeText;
        [SerializeField] private Text throughputText;
        [SerializeField] private Text successText;
        [SerializeField] private Text totalTimeText;
        
        [Header("Buttons")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;
        
        [Header("UI Settings")]
        [SerializeField] private Canvas summaryCanvas;
        
        void Start()
        {
            InitializeUI();
            SubscribeToEvents();
        }
        
        void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        
        private void InitializeUI()
        {
            // Create UI elements if not assigned
            if (summaryCanvas == null)
            {
                CreateSummaryCanvas();
            }
            
            if (summaryPanel == null)
            {
                CreateSummaryPanel();
            }
            
            CreateSummaryElements();
            
            // Hide summary initially
            HideSummary();
        }
        
        private void CreateSummaryCanvas()
        {
            GameObject canvasObj = new GameObject("EndLevel Canvas");
            summaryCanvas = canvasObj.AddComponent<Canvas>();
            summaryCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            summaryCanvas.sortingOrder = 200; // Above HUD
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        private void CreateSummaryPanel()
        {
            GameObject panelObj = new GameObject("Summary Panel");
            panelObj.transform.SetParent(summaryCanvas.transform, false);
            
            summaryPanel = panelObj;
            
            // Add background image
            Image panelImage = panelObj.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.9f); // Semi-transparent black
            
            // Center the panel
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(600, 500);
        }
        
        private void CreateSummaryElements()
        {
            // Create title
            titleText = CreateSummaryText("TitleText", "Level Complete!", 0, 28, Color.yellow);
            
            // Create statistics text elements
            accuracyText = CreateSummaryText("AccuracyText", "Accuracy: 0.0%", 1, 20, Color.white);
            errorText = CreateSummaryText("ErrorText", "Mean Error: 0.0px", 2, 20, Color.white);
            timeText = CreateSummaryText("TimeText", "Mean Time: 0.0s", 3, 20, Color.white);
            throughputText = CreateSummaryText("ThroughputText", "Throughput: 0.0/min", 4, 20, Color.white);
            successText = CreateSummaryText("SuccessText", "Success: 0/0", 5, 20, Color.white);
            totalTimeText = CreateSummaryText("TotalTimeText", "Total Time: 0.0s", 6, 20, Color.white);
            
            // Create buttons
            CreateButtons();
        }
        
        private Text CreateSummaryText(string name, string text, int index, int fontSize, Color color)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(summaryPanel.transform, false);
            
            Text textComponent = textObj.AddComponent<Text>();
            textComponent.text = text;
            textComponent.fontSize = fontSize;
            textComponent.color = color;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 1);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.pivot = new Vector2(0.5f, 1);
            textRect.anchoredPosition = new Vector2(0, -50 - (index * 40));
            textRect.sizeDelta = new Vector2(-40, 30);
            
            return textComponent;
        }
        
        private void CreateButtons()
        {
            // Restart button
            GameObject restartObj = new GameObject("RestartButton");
            restartObj.transform.SetParent(summaryPanel.transform, false);
            
            restartButton = restartObj.AddComponent<Button>();
            Image restartImage = restartObj.AddComponent<Image>();
            restartImage.color = new Color(0.2f, 0.7f, 0.2f, 0.8f); // Green
            
            RectTransform restartRect = restartObj.GetComponent<RectTransform>();
            restartRect.anchorMin = new Vector2(0.2f, 0);
            restartRect.anchorMax = new Vector2(0.2f, 0);
            restartRect.pivot = new Vector2(0.5f, 0);
            restartRect.anchoredPosition = new Vector2(0, 30);
            restartRect.sizeDelta = new Vector2(150, 40);
            
            // Restart button text
            GameObject restartTextObj = new GameObject("RestartText");
            restartTextObj.transform.SetParent(restartObj.transform, false);
            
            Text restartText = restartTextObj.AddComponent<Text>();
            restartText.text = "Restart Level";
            restartText.fontSize = 16;
            restartText.color = Color.white;
            restartText.alignment = TextAnchor.MiddleCenter;
            restartText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            
            RectTransform restartTextRect = restartTextObj.GetComponent<RectTransform>();
            restartTextRect.anchorMin = Vector2.zero;
            restartTextRect.anchorMax = Vector2.one;
            restartTextRect.offsetMin = Vector2.zero;
            restartTextRect.offsetMax = Vector2.zero;
            
            // Exit button
            GameObject exitObj = new GameObject("ExitButton");
            exitObj.transform.SetParent(summaryPanel.transform, false);
            
            exitButton = exitObj.AddComponent<Button>();
            Image exitImage = exitObj.AddComponent<Image>();
            exitImage.color = new Color(0.7f, 0.2f, 0.2f, 0.8f); // Red
            
            RectTransform exitRect = exitObj.GetComponent<RectTransform>();
            exitRect.anchorMin = new Vector2(0.8f, 0);
            exitRect.anchorMax = new Vector2(0.8f, 0);
            exitRect.pivot = new Vector2(0.5f, 0);
            exitRect.anchoredPosition = new Vector2(0, 30);
            exitRect.sizeDelta = new Vector2(150, 40);
            
            // Exit button text
            GameObject exitTextObj = new GameObject("ExitText");
            exitTextObj.transform.SetParent(exitObj.transform, false);
            
            Text exitText = exitTextObj.AddComponent<Text>();
            exitText.text = "Exit Game";
            exitText.fontSize = 16;
            exitText.color = Color.white;
            exitText.alignment = TextAnchor.MiddleCenter;
            exitText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            
            RectTransform exitTextRect = exitTextObj.GetComponent<RectTransform>();
            exitTextRect.anchorMin = Vector2.zero;
            exitTextRect.anchorMax = Vector2.one;
            exitTextRect.offsetMin = Vector2.zero;
            exitTextRect.offsetMax = Vector2.zero;
            
            // Setup button events
            restartButton.onClick.AddListener(RestartLevel);
            exitButton.onClick.AddListener(ExitGame);
        }
        
        private void SubscribeToEvents()
        {
            LevelController levelController = FindObjectOfType<LevelController>();
            if (levelController != null)
            {
                levelController.OnLevelSummaryReady += ShowSummary;
            }
        }
        
        private void UnsubscribeFromEvents()
        {
            LevelController levelController = FindObjectOfType<LevelController>();
            if (levelController != null)
            {
                levelController.OnLevelSummaryReady -= ShowSummary;
            }
        }
        
        public void ShowSummary(StatisticsSummary summary)
        {
            if (summary == null) return;
            
            // Update text elements with summary data
            if (accuracyText != null)
                accuracyText.text = $"Accuracy: {summary.accuracyPercentage:F1}%";
            
            if (errorText != null)
                errorText.text = $"Mean Error: {summary.meanPlacementError:F1}px";
            
            if (timeText != null)
                timeText.text = $"Mean Time: {summary.meanCompletionTime:F2}s";
            
            if (throughputText != null)
                throughputText.text = $"Throughput: {summary.totalThroughput:F1}/min";
            
            if (successText != null)
                successText.text = $"Success: {summary.successfulBalls}/{summary.totalBalls}";
            
            if (totalTimeText != null)
                totalTimeText.text = $"Total Time: {summary.totalLevelTime:F1}s";
            
            // Color code accuracy
            if (accuracyText != null)
            {
                if (summary.accuracyPercentage >= 80f)
                    accuracyText.color = Color.green;
                else if (summary.accuracyPercentage >= 60f)
                    accuracyText.color = Color.yellow;
                else
                    accuracyText.color = Color.red;
            }
            
            // Show the summary panel
            if (summaryPanel != null)
            {
                summaryPanel.SetActive(true);
            }
            
            Debug.Log($"End level summary displayed: {summary}");
        }
        
        public void HideSummary()
        {
            if (summaryPanel != null)
            {
                summaryPanel.SetActive(false);
            }
        }
        
        private void RestartLevel()
        {
            HideSummary();
            
            LevelController levelController = FindObjectOfType<LevelController>();
            if (levelController != null)
            {
                levelController.RestartLevel();
            }
            
            Debug.Log("Restart level requested from UI");
        }
        
        private void ExitGame()
        {
            LevelController levelController = FindObjectOfType<LevelController>();
            if (levelController != null)
            {
                levelController.ExitLevel();
            }
            
            // Quit the application
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitGame();
            }
            else
            {
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            }
            
            Debug.Log("Exit game requested from UI");
        }
    }
}