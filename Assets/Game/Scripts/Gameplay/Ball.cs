using UnityEngine;
using SortingBoardGame.Data;
using SortingBoardGame.Managers;

namespace SortingBoardGame.Gameplay
{
    public enum BallState
    {
        Spawning,
        Idle,
        Hovered,
        Dragged,
        Placed,
        Failed
    }
    
    public class Ball : MonoBehaviour
    {
        [SerializeField] private BallColor ballColor;
        [SerializeField] private float ballSize = 120f; // 120px diameter
        [SerializeField] private Material glowMaterial;
        [SerializeField] private TrailRenderer trailRenderer;
        
        private BallState currentState = BallState.Spawning;
        private Rigidbody ballRigidbody;
        private Renderer ballRenderer;
        private Material originalMaterial;
        private bool isGlowing = false;
        
        // Ball data tracking
        private BallData ballData;
        private int ballId;
        private float lastRollSoundTime = 0f;
        
        public BallColor Color => ballColor;
        public float Size => ballSize;
        public BallState State => currentState;
        public int BallId => ballId;
        public BallData Data => ballData;
        
        void Awake()
        {
            ballRigidbody = GetComponent<Rigidbody>();
            ballRenderer = GetComponent<Renderer>();
            
            if (ballRigidbody == null)
            {
                ballRigidbody = gameObject.AddComponent<Rigidbody>();
            }
            
            if (trailRenderer == null)
            {
                trailRenderer = gameObject.AddComponent<TrailRenderer>();
                SetupTrailRenderer();
            }
            
            // Ensure ball has proper tag
            gameObject.tag = "Ball";
        }
        
        void Start()
        {
            originalMaterial = ballRenderer.material;
            SetupPhysics();
        }
        
        void Update()
        {
            // Play rolling sound when ball is moving
            if (currentState == BallState.Idle && ballRigidbody != null)
            {
                float velocity = ballRigidbody.velocity.magnitude;
                if (velocity > 0.5f && Time.time - lastRollSoundTime > 0.5f)
                {
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayBallRollSound(transform.position, velocity / 5f);
                        lastRollSoundTime = Time.time;
                    }
                }
            }
        }
        
        private void SetupPhysics()
        {
            // Configure Rigidbody for semi-realistic but playful physics
            ballRigidbody.mass = 1f; // No mass differences as per requirements
            ballRigidbody.drag = 0.5f; // Some air resistance
            ballRigidbody.angularDrag = 0.3f; // Rolling resistance
            
            // Add physics material for realistic rolling
            SphereCollider sphereCollider = GetComponent<SphereCollider>();
            if (sphereCollider == null)
            {
                sphereCollider = gameObject.AddComponent<SphereCollider>();
            }
            
            // Create physics material for rolling
            PhysicMaterial ballPhysicsMaterial = new PhysicMaterial("BallPhysics");
            ballPhysicsMaterial.dynamicFriction = 0.3f;
            ballPhysicsMaterial.staticFriction = 0.3f;
            ballPhysicsMaterial.bounciness = 0.4f;
            ballPhysicsMaterial.frictionCombine = PhysicMaterialCombine.Average;
            ballPhysicsMaterial.bounceCombine = PhysicMaterialCombine.Average;
            
            sphereCollider.material = ballPhysicsMaterial;
        }
        
        private void SetupTrailRenderer()
        {
            trailRenderer.enabled = false; // Start disabled
            trailRenderer.time = 0.5f;
            trailRenderer.startWidth = 0.1f;
            trailRenderer.endWidth = 0.02f;
            trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
            trailRenderer.color = GetUnityColor(ballColor);
        }
        
        public void Initialize(int id, BallColor color, string sessionId)
        {
            ballId = id;
            ballColor = color;
            
            // Initialize ball data for tracking
            ballData = new BallData();
            ballData.sessionId = sessionId;
            ballData.trialId = id;
            ballData.ballColor = color;
            ballData.ballSize = ballSize;
            ballData.spawnTime = Time.time;
            
            // Set ball appearance
            SetBallColor(color);
            SetState(BallState.Spawning);
            
            // Scale ball to correct size (convert pixels to Unity units)
            float unityScale = ballSize / 100f; // Rough conversion
            transform.localScale = Vector3.one * unityScale;
        }
        
        private void SetBallColor(BallColor color)
        {
            Material ballMaterial = new Material(Shader.Find("Standard"));
            ballMaterial.color = GetUnityColor(color);
            
            // Make balls glossy as per requirements
            ballMaterial.SetFloat("_Glossiness", 0.8f);
            ballMaterial.SetFloat("_Metallic", 0.1f);
            
            ballRenderer.material = ballMaterial;
            originalMaterial = ballMaterial;
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
        
        public void SetState(BallState newState)
        {
            if (currentState == newState) return;
            
            BallState previousState = currentState;
            currentState = newState;
            
            OnStateChanged(previousState, newState);
        }
        
        private void OnStateChanged(BallState from, BallState to)
        {
            switch (to)
            {
                case BallState.Spawning:
                    ballRigidbody.isKinematic = false;
                    trailRenderer.enabled = false;
                    break;
                    
                case BallState.Idle:
                    ballRigidbody.isKinematic = false;
                    trailRenderer.enabled = false;
                    SetGlow(false);
                    break;
                    
                case BallState.Hovered:
                    SetGlow(true);
                    break;
                    
                case BallState.Dragged:
                    ballRigidbody.isKinematic = true; // Disable physics during drag
                    trailRenderer.enabled = true;
                    ballData.graspTime = Time.time;
                    break;
                    
                case BallState.Placed:
                    ballRigidbody.isKinematic = false;
                    trailRenderer.enabled = false;
                    SetGlow(false);
                    ballData.releaseTime = Time.time;
                    ballData.completionTime = ballData.releaseTime - ballData.spawnTime;
                    break;
                    
                case BallState.Failed:
                    ballRigidbody.isKinematic = false;
                    trailRenderer.enabled = false;
                    SetGlow(false);
                    ballData.releaseTime = Time.time;
                    ballData.completionTime = ballData.releaseTime - ballData.spawnTime;
                    break;
            }
        }
        
        public void SetGlow(bool glow)
        {
            if (isGlowing == glow) return;
            
            isGlowing = glow;
            
            if (glow && glowMaterial != null)
            {
                ballRenderer.material = glowMaterial;
            }
            else if (glow)
            {
                // Create glow effect by increasing emission
                Material glowMat = new Material(originalMaterial);
                glowMat.EnableKeyword("_EMISSION");
                glowMat.SetColor("_EmissionColor", GetUnityColor(ballColor) * 0.5f);
                ballRenderer.material = glowMat;
            }
            else
            {
                ballRenderer.material = originalMaterial;
            }
        }
        
        public void ApplySpawnImpulse(Vector3 spawnPosition)
        {
            transform.position = spawnPosition;
            
            // Apply gentle rolling impulse onto the board
            Vector3 randomDirection = new Vector3(
                Random.Range(-0.5f, 0.5f),
                0,
                Random.Range(-0.5f, 0.5f)
            ).normalized;
            
            ballRigidbody.AddForce(randomDirection * 2f, ForceMode.Impulse);
            ballRigidbody.AddTorque(Random.insideUnitSphere * 1f, ForceMode.Impulse);
            
            SetState(BallState.Idle);
        }
        
        public void SnapToHole(Vector3 holePosition)
        {
            // Smooth snapping animation to hole
            transform.position = holePosition;
            ballRigidbody.velocity = Vector3.zero;
            ballRigidbody.angularVelocity = Vector3.zero;
            SetState(BallState.Placed);
        }
        
        public void BounceOff(Vector3 bounceDirection)
        {
            // Apply bounce effect for failed placements
            ballRigidbody.AddForce(bounceDirection * 3f, ForceMode.Impulse);
            SetState(BallState.Failed);
        }
        
        // Mouse interaction is now handled by MouseInteractionSystem
        // These methods are kept for compatibility but may not be used
        void OnMouseEnter()
        {
            // Mouse interaction handled by MouseInteractionSystem
        }
        
        void OnMouseExit()
        {
            // Mouse interaction handled by MouseInteractionSystem
        }
        
        // Public method to check if ball can be interacted with
        public bool CanBeInteracted()
        {
            return currentState == BallState.Idle || currentState == BallState.Hovered;
        }
        
        // Public method to get current position for calculations
        public Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}