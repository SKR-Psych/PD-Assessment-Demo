using UnityEngine;
using SortingBoardGame.Data;

namespace SortingBoardGame.Gameplay
{
    public class Hole : MonoBehaviour
    {
        [SerializeField] private HoleConfig holeConfig;
        [SerializeField] private Material highlightMaterial;
        
        private Renderer holeRenderer;
        private Material originalMaterial;
        private bool isHighlighted = false;
        
        public BallColor HoleColor => holeConfig.color;
        public float HoleDiameter => holeConfig.diameter;
        public Vector3 HolePosition => transform.position;
        
        void Start()
        {
            holeRenderer = GetComponent<Renderer>();
            if (holeRenderer != null)
            {
                originalMaterial = holeRenderer.material;
            }
        }
        
        public void Initialize(HoleConfig config)
        {
            holeConfig = config;
            SetupHole();
        }
        
        private void SetupHole()
        {
            // Position the hole
            transform.position = holeConfig.position;
            
            // Scale hole based on diameter (convert pixels to Unity units)
            float unityScale = holeConfig.diameter / 100f; // Rough conversion
            transform.localScale = new Vector3(unityScale, 0.1f, unityScale);
            
            // Create hole material with appropriate color
            Material holeMaterial = new Material(Shader.Find("Standard"));
            holeMaterial.color = GetUnityColor(holeConfig.color);
            
            // Make it slightly emissive for better visibility
            holeMaterial.EnableKeyword("_EMISSION");
            holeMaterial.SetColor("_EmissionColor", holeMaterial.color * 0.3f);
            
            holeRenderer = GetComponent<Renderer>();
            if (holeRenderer == null)
            {
                // Add a renderer if none exists
                MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
                MeshFilter filter = gameObject.AddComponent<MeshFilter>();
                
                // Create a simple cylinder mesh for the hole
                GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                filter.mesh = cylinder.GetComponent<MeshFilter>().mesh;
                DestroyImmediate(cylinder);
                
                holeRenderer = renderer;
            }
            
            holeRenderer.material = holeMaterial;
            originalMaterial = holeMaterial;
            
            // Add collider for ball detection
            SphereCollider holeCollider = gameObject.GetComponent<SphereCollider>();
            if (holeCollider == null)
            {
                holeCollider = gameObject.AddComponent<SphereCollider>();
            }
            holeCollider.isTrigger = true;
            holeCollider.radius = 0.6f; // Slightly larger than visual for easier targeting
            
            Debug.Log($"Hole created: {holeConfig.color} color, {holeConfig.diameter}px diameter at {holeConfig.position}");
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
        
        public void SetHighlight(bool highlight)
        {
            if (isHighlighted == highlight) return;
            
            isHighlighted = highlight;
            
            if (holeRenderer == null) return;
            
            if (highlight && highlightMaterial != null)
            {
                holeRenderer.material = highlightMaterial;
            }
            else if (highlight)
            {
                // Create highlight effect by increasing emission
                Material highlightMat = new Material(originalMaterial);
                highlightMat.EnableKeyword("_EMISSION");
                highlightMat.SetColor("_EmissionColor", GetUnityColor(holeConfig.color) * 0.8f);
                holeRenderer.material = highlightMat;
            }
            else
            {
                holeRenderer.material = originalMaterial;
            }
        }
        
        public bool CanAcceptBall(BallColor ballColor, float ballSize)
        {
            // Check color match and size compatibility
            return holeConfig.color == ballColor && ballSize <= holeConfig.diameter;
        }
        
        public float GetDistanceFromCenter(Vector3 ballPosition)
        {
            Vector3 holeCenter = transform.position;
            holeCenter.y = ballPosition.y; // Compare on same Y level
            return Vector3.Distance(ballPosition, holeCenter);
        }
        
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ball"))
            {
                // Handle ball entering hole area
                Debug.Log($"Ball entered {holeConfig.color} hole area");
            }
        }
        
        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Ball"))
            {
                // Handle ball leaving hole area
                Debug.Log($"Ball left {holeConfig.color} hole area");
            }
        }
    }
}