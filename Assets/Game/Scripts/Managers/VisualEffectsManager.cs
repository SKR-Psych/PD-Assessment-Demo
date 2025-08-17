using UnityEngine;
using SortingBoardGame.Data;
using SortingBoardGame.Gameplay;

namespace SortingBoardGame.Managers
{
    public class VisualEffectsManager : MonoBehaviour
    {
        [Header("Particle Effects")]
        [SerializeField] private ParticleSystem successParticlesPrefab;
        [SerializeField] private ParticleSystem failureParticlesPrefab;
        
        [Header("Placement Error Indicator")]
        [SerializeField] private GameObject placementErrorIndicatorPrefab;
        [SerializeField] private Canvas uiCanvas;
        
        [Header("Effect Settings")]
        [SerializeField] private float particleLifetime = 2f;
        [SerializeField] private float errorIndicatorDuration = 3f;
        
        private Camera mainCamera;
        
        void Start()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
                mainCamera = FindObjectOfType<Camera>();
            
            if (uiCanvas == null)
                uiCanvas = FindObjectOfType<Canvas>();
        }
        
        public void PlaySuccessEffect(Vector3 worldPosition, BallColor ballColor)
        {
            if (successParticlesPrefab == null) return;
            
            // Instantiate success particle effect
            ParticleSystem particles = Instantiate(successParticlesPrefab, worldPosition, Quaternion.identity);
            
            // Set particle color to match ball color
            var main = particles.main;
            main.startColor = GetUnityColor(ballColor);
            
            // Set particle emission properties
            var emission = particles.emission;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0.0f, 50) // Burst of 50 particles at start
            });
            
            // Auto-destroy after lifetime
            Destroy(particles.gameObject, particleLifetime);
            
            Debug.Log($"Playing success particle effect for {ballColor} ball at {worldPosition}");
        }
        
        public void PlayFailureEffect(Vector3 worldPosition)
        {
            if (failureParticlesPrefab == null) return;
            
            // Instantiate failure particle effect
            ParticleSystem particles = Instantiate(failureParticlesPrefab, worldPosition, Quaternion.identity);
            
            // Set dull color for failure
            var main = particles.main;
            main.startColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Dull gray
            
            // Auto-destroy after lifetime
            Destroy(particles.gameObject, particleLifetime);
            
            Debug.Log($"Playing failure particle effect at {worldPosition}");
        }
        
        public void ShowPlacementErrorIndicator(Vector3 worldPosition, float errorDistance, bool isSuccess)
        {
            if (placementErrorIndicatorPrefab == null || uiCanvas == null || mainCamera == null) 
            {
                CreateDefaultErrorIndicator(worldPosition, errorDistance, isSuccess);
                return;
            }
            
            // Convert world position to screen position
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            
            // Create error indicator UI element
            GameObject indicator = Instantiate(placementErrorIndicatorPrefab, uiCanvas.transform);
            RectTransform rectTransform = indicator.GetComponent<RectTransform>();
            
            if (rectTransform != null)
            {
                rectTransform.position = screenPosition;
            }
            
            // Configure the indicator (assuming it has a PlacementErrorIndicator component)
            PlacementErrorIndicator errorComponent = indicator.GetComponent<PlacementErrorIndicator>();
            if (errorComponent != null)
            {
                errorComponent.Initialize(errorDistance, isSuccess);
            }
            
            // Auto-destroy after duration
            Destroy(indicator, errorIndicatorDuration);
        }
        
        private void CreateDefaultErrorIndicator(Vector3 worldPosition, float errorDistance, bool isSuccess)
        {
            // Create a simple 3D text indicator as fallback
            GameObject textObj = new GameObject("PlacementErrorIndicator");
            textObj.transform.position = worldPosition + Vector3.up * 0.5f;
            
            TextMesh textMesh = textObj.AddComponent<TextMesh>();
            textMesh.text = $"Error: {errorDistance:F1}px";
            textMesh.fontSize = 20;
            textMesh.color = isSuccess ? Color.green : Color.red;
            textMesh.anchor = TextAnchor.MiddleCenter;
            
            // Make text face camera
            textObj.transform.LookAt(mainCamera.transform);
            textObj.transform.Rotate(0, 180, 0);
            
            // Auto-destroy
            Destroy(textObj, errorIndicatorDuration);
        }
        
        private Color GetUnityColor(BallColor ballColor)
        {
            switch (ballColor)
            {
                case BallColor.Red:
                    return Color.red;
                case BallColor.Blue:
                    return Color.blue;
                case BallColor.Yellow:
                    return Color.yellow;
                default:
                    return Color.white;
            }
        }
        
        // Create default particle systems if prefabs are not assigned
        public void CreateDefaultParticleSystems()
        {
            if (successParticlesPrefab == null)
            {
                successParticlesPrefab = CreateDefaultSuccessParticles();
            }
            
            if (failureParticlesPrefab == null)
            {
                failureParticlesPrefab = CreateDefaultFailureParticles();
            }
        }
        
        private ParticleSystem CreateDefaultSuccessParticles()
        {
            GameObject particleObj = new GameObject("SuccessParticles");
            ParticleSystem particles = particleObj.AddComponent<ParticleSystem>();
            
            var main = particles.main;
            main.startLifetime = 1f;
            main.startSpeed = 5f;
            main.startSize = 0.1f;
            main.startColor = Color.white;
            main.maxParticles = 50;
            
            var emission = particles.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0.0f, 50)
            });
            
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.2f;
            
            var velocityOverLifetime = particles.velocityOverLifetime;
            velocityOverLifetime.enabled = true;
            velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
            velocityOverLifetime.radial = new ParticleSystem.MinMaxCurve(2f);
            
            // Don't make it a prefab, just return the component
            return particles;
        }
        
        private ParticleSystem CreateDefaultFailureParticles()
        {
            GameObject particleObj = new GameObject("FailureParticles");
            ParticleSystem particles = particleObj.AddComponent<ParticleSystem>();
            
            var main = particles.main;
            main.startLifetime = 0.5f;
            main.startSpeed = 2f;
            main.startSize = 0.05f;
            main.startColor = Color.gray;
            main.maxParticles = 20;
            
            var emission = particles.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0.0f, 20)
            });
            
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.1f;
            
            return particles;
        }
    }
}