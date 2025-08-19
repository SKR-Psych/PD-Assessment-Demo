using UnityEngine;
using UnityEngine.UI;

namespace SortingBoardGame.UI
{
    public class PlacementErrorIndicator : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Text errorText;
        [SerializeField] private Slider errorBar;
        [SerializeField] private Image backgroundImage;
        
        [Header("Visual Settings")]
        [SerializeField] private Color successColor = Color.green;
        [SerializeField] private Color failureColor = Color.red;
        [SerializeField] private float maxErrorDistance = 100f; // Max error in pixels for bar scaling
        
        private bool isInitialized = false;
        
        void Awake()
        {
            // Try to find components if not assigned
            if (errorText == null)
                errorText = GetComponentInChildren<Text>();
            
            if (errorBar == null)
                errorBar = GetComponentInChildren<Slider>();
            
            if (backgroundImage == null)
                backgroundImage = GetComponent<Image>();
        }
        
        public void Initialize(float errorDistance, bool isSuccess)
        {
            isInitialized = true;
            
            // Set colors based on success/failure
            Color indicatorColor = isSuccess ? successColor : failureColor;
            
            // Update text
            if (errorText != null)
            {
                errorText.text = $"Error: {errorDistance:F1}px";
                errorText.color = indicatorColor;
            }
            
            // Update error bar
            if (errorBar != null)
            {
                errorBar.value = Mathf.Clamp01(errorDistance / maxErrorDistance);
                
                // Color the bar fill
                Image fillImage = errorBar.fillRect?.GetComponent<Image>();
                if (fillImage != null)
                {
                    fillImage.color = indicatorColor;
                }
            }
            
            // Update background
            if (backgroundImage != null)
            {
                Color bgColor = indicatorColor;
                bgColor.a = 0.3f; // Semi-transparent background
                backgroundImage.color = bgColor;
            }
            
            // Start fade out animation
            StartCoroutine(FadeOutCoroutine());
        }
        
        private System.Collections.IEnumerator FadeOutCoroutine()
        {
            float duration = 3f;
            float elapsed = 0f;
            
            // Get initial alpha values
            float textAlpha = errorText != null ? errorText.color.a : 0f;
            float bgAlpha = backgroundImage != null ? backgroundImage.color.a : 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                
                // Fade text
                if (errorText != null)
                {
                    Color textColor = errorText.color;
                    textColor.a = textAlpha * alpha;
                    errorText.color = textColor;
                }
                
                // Fade background
                if (backgroundImage != null)
                {
                    Color bgColor = backgroundImage.color;
                    bgColor.a = bgAlpha * alpha;
                    backgroundImage.color = bgColor;
                }
                
                // Fade error bar
                if (errorBar != null)
                {
                    CanvasGroup canvasGroup = errorBar.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                        canvasGroup = errorBar.gameObject.AddComponent<CanvasGroup>();
                    
                    canvasGroup.alpha = alpha;
                }
                
                yield return null;
            }
        }
        
        // Create a default error indicator if no prefab is available
        public static GameObject CreateDefaultIndicator()
        {
            GameObject indicator = new GameObject("PlacementErrorIndicator");
            
            // Add Canvas Group for fading
            CanvasGroup canvasGroup = indicator.AddComponent<CanvasGroup>();
            
            // Add background image
            Image background = indicator.AddComponent<Image>();
            background.color = new Color(0, 0, 0, 0.5f);
            
            // Add RectTransform
            RectTransform rectTransform = indicator.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(150, 50);
            
            // Create text child
            GameObject textObj = new GameObject("ErrorText");
            textObj.transform.SetParent(indicator.transform);
            
            Text text = textObj.AddComponent<Text>();
            text.text = "Error: 0.0px";
            text.fontSize = 14;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            // Add the component
            PlacementErrorIndicator component = indicator.AddComponent<PlacementErrorIndicator>();
            component.errorText = text;
            component.backgroundImage = background;
            
            return indicator;
        }
    }
}