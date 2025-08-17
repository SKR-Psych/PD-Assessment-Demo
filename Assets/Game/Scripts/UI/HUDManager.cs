using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SortingBoardGame.Managers;

namespace SortingBoardGame.UI
{
    public class HUDManager : MonoBehaviour
    {
        [Header("HUD Text Elements")]
        [SerializeField] private TextMeshProUGUI accuracyText;
        [SerializeField] private TextMeshProUGUI averageErrorText;
        [SerializeField] private TextMeshProUGUI averageTimeText;
        [SerializeField] private TextMeshProUGUI throughputText;
        [SerializeField] private TextMeshProUGUI placementCountText;
        
        [Header("HUD Panel")]
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private Canvas hudCanvas;
        
        [Header("Audio Controls")]
        [SerializeField] private Button musicToggleButton;
        [SerializeField] private TextMeshProUGUI musicToggleText;
        
        void Start()
        {
            InitializeHUD();
            SubscribeToStatistics();
            SetupAudioControls();
        }
        
        void OnDestroy()
        {
            UnsubscribeFromStatistics();
        }
        
        private void InitializeHUD()
        {
            // Create HUD elements if not assigned
            if (hudCanvas == null)
            {
                CreateHUDCanvas();
            }
            
            if (hudPanel == null)
            {
                CreateHUDPanel();
            }
            
            CreateHUDElements();
            
            // Initialize display with zero values
            UpdateAccuracy(0f);
            UpdateAverageError(0f);
            UpdateAverageTime(0f);
            UpdateThroughput(0f);
            UpdatePlacementCount(0, 0);
        }
        
        private void CreateHUDCanvas()
        {
            GameObject canvasObj = new GameObject("HUD Canvas");
            hudCanvas = canvasObj.AddComponent<Canvas>();
            hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            hudCanvas.sortingOrder = 100;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        private void CreateHUDPanel()
        {
            GameObject panelObj = new GameObject("HUD Panel");
            panelObj.transform.SetParent(hudCanvas.transform, false);
            
            hudPanel = panelObj;
            
            // Add background image
            Image panelImage = panelObj.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.7f); // Semi-transparent black
            
            // Position panel at top-left
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(0, 1);
            panelRect.pivot = new Vector2(0, 1);
            panelRect.anchoredPosition = new Vector2(20, -20);
            panelRect.sizeDelta = new Vector2(300, 200);
        }
        
        private void CreateHUDElements()
        {
            // Create text elements for statistics
            accuracyText = CreateHUDText("AccuracyText", "Accuracy: 0.0%", 0);
            averageErrorText = CreateHUDText("AverageErrorText", "Avg Error: 0.0px", 1);
            averageTimeText = CreateHUDText("AverageTimeText", "Avg Time: 0.0s", 2);
            throughputText = CreateHUDText("ThroughputText", "Rate: 0.0/min", 3);
            placementCountText = CreateHUDText("PlacementCountText", "Placed: 0/0", 4);
            
            // Create music toggle button
            CreateMusicToggleButton();
        }
        
        private TextMeshProUGUI CreateHUDText(string name, string initialText, int index)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(hudPanel.transform, false);
            
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = initialText;
            text.fontSize = 16;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Left;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 1);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.pivot = new Vector2(0, 1);
            textRect.anchoredPosition = new Vector2(10, -10 - (index * 25));
            textRect.sizeDelta = new Vector2(-20, 20);
            
            return text;
        }
        
        private void CreateMusicToggleButton()
        {
            GameObject buttonObj = new GameObject("MusicToggleButton");
            buttonObj.transform.SetParent(hudPanel.transform, false);
            
            musicToggleButton = buttonObj.AddComponent<Button>();
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0, 0);
            buttonRect.anchorMax = new Vector2(1, 0);
            buttonRect.pivot = new Vector2(0.5f, 0);
            buttonRect.anchoredPosition = new Vector2(0, 10);
            buttonRect.sizeDelta = new Vector2(-20, 30);
            
            // Create button text
            GameObject textObj = new GameObject("ButtonText");
            textObj.transform.SetParent(buttonObj.transform, false);
            
            musicToggleText = textObj.AddComponent<TextMeshProUGUI>();
            musicToggleText.text = "🔊 Music: ON";
            musicToggleText.fontSize = 14;
            musicToggleText.color = Color.white;
            musicToggleText.alignment = TextAlignmentOptions.Center;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }
        
        private void SetupAudioControls()
        {
            if (musicToggleButton != null)
            {
                musicToggleButton.onClick.AddListener(ToggleMusic);
            }
        }
        
        private void ToggleMusic()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ToggleMusicMute();
                UpdateMusicToggleButton();
            }
        }
        
        private void UpdateMusicToggleButton()
        {
            if (AudioManager.Instance != null && musicToggleText != null)
            {
                bool isMuted = AudioManager.Instance.IsMusicMuted;
                musicToggleText.text = isMuted ? "🔇 Music: OFF" : "🔊 Music: ON";
            }
        }
        
        private void SubscribeToStatistics()
        {
            if (StatisticsManager.Instance != null)
            {
                StatisticsManager.Instance.OnAccuracyUpdated += UpdateAccuracy;
                StatisticsManager.Instance.OnAverageErrorUpdated += UpdateAverageError;
                StatisticsManager.Instance.OnAverageTimeUpdated += UpdateAverageTime;
                StatisticsManager.Instance.OnThroughputUpdated += UpdateThroughput;
                StatisticsManager.Instance.OnPlacementCountUpdated += UpdatePlacementCount;
            }
        }
        
        private void UnsubscribeFromStatistics()
        {
            if (StatisticsManager.Instance != null)
            {
                StatisticsManager.Instance.OnAccuracyUpdated -= UpdateAccuracy;
                StatisticsManager.Instance.OnAverageErrorUpdated -= UpdateAverageError;
                StatisticsManager.Instance.OnAverageTimeUpdated -= UpdateAverageTime;
                StatisticsManager.Instance.OnThroughputUpdated -= UpdateThroughput;
                StatisticsManager.Instance.OnPlacementCountUpdated -= UpdatePlacementCount;
            }
        }
        
        private void UpdateAccuracy(float accuracy)
        {
            if (accuracyText != null)
            {
                accuracyText.text = $"Accuracy: {accuracy:F1}%";
                
                // Color code based on accuracy
                if (accuracy >= 80f)
                    accuracyText.color = Color.green;
                else if (accuracy >= 60f)
                    accuracyText.color = Color.yellow;
                else
                    accuracyText.color = Color.red;
            }
        }
        
        private void UpdateAverageError(float averageError)
        {
            if (averageErrorText != null)
            {
                averageErrorText.text = $"Avg Error: {averageError:F1}px";
                
                // Color code based on error (lower is better)
                if (averageError <= 20f)
                    averageErrorText.color = Color.green;
                else if (averageError <= 50f)
                    averageErrorText.color = Color.yellow;
                else
                    averageErrorText.color = Color.red;
            }
        }
        
        private void UpdateAverageTime(float averageTime)
        {
            if (averageTimeText != null)
            {
                averageTimeText.text = $"Avg Time: {averageTime:F1}s";
                
                // Color code based on time (faster is better)
                if (averageTime <= 3f)
                    averageTimeText.color = Color.green;
                else if (averageTime <= 5f)
                    averageTimeText.color = Color.yellow;
                else
                    averageTimeText.color = Color.red;
            }
        }
        
        private void UpdateThroughput(float throughput)
        {
            if (throughputText != null)
            {
                throughputText.text = $"Rate: {throughput:F1}/min";
                throughputText.color = Color.white;
            }
        }
        
        private void UpdatePlacementCount(int successful, int total)
        {
            if (placementCountText != null)
            {
                placementCountText.text = $"Placed: {successful}/{total}";
                placementCountText.color = Color.white;
            }
        }
        
        public void ShowHUD()
        {
            if (hudPanel != null)
            {
                hudPanel.SetActive(true);
            }
        }
        
        public void HideHUD()
        {
            if (hudPanel != null)
            {
                hudPanel.SetActive(false);
            }
        }
        
        // Update method to ensure 60+ FPS performance
        void Update()
        {
            // Efficient HUD updates - only update music button state if needed
            if (Time.frameCount % 60 == 0) // Update once per second at 60 FPS
            {
                UpdateMusicToggleButton();
            }
        }
    }
}