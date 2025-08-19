using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SortingBoardGame.Data;
using SortingBoardGame.Gameplay;

namespace SortingBoardGame
{
    // Using BallData from SortingBoardGame.Data namespace instead of SimpleBallData
    
    public class SimpleGame : MonoBehaviour
    {
        [Header("Game Settings")]
        public int totalBalls = 20;
        public float spawnInterval = 2.0f;
        public float ballDiameter = 120f;
        public Vector3 boardSize = new Vector3(1.0f, 0.0f, 0.6f);
        public float boardHeight = 0.9f;
        public float[] holeDiameters = { 160f, 140f, 120f };
        
        [Header("Game Objects")]
        public GameObject ballPrefab;
        public Material[] ballMaterials;
        public Material[] holeMaterials;
        
        private Camera mainCamera;
        private GameObject gameBoard;
        private List<GameObject> holes = new List<GameObject>();
        private List<GameObject> balls = new List<GameObject>();
        private Queue<GameObject> ballPool = new Queue<GameObject>();
        
        private bool isSpawning = false;
        private int ballsSpawned = 0;
        private string sessionId;
        
        // Statistics
        private int successfulPlacements = 0;
        private int totalPlacements = 0;
        private float totalError = 0f;
        private float totalTime = 0f;
        private float levelStartTime;
        
        // Mouse interaction
        private GameObject draggedBall = null;
        private bool isDragging = false;
        private Vector3 dragOffset = Vector3.zero;
        
        void Start()
        {
            sessionId = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            levelStartTime = Time.time;
            
            SetupCamera();
            CreateBoard();
            CreateHoles();
            InitializeBallPool();
            
            // Start spawning after a delay
            Invoke(nameof(StartSpawning), 1f);
        }
        
        void Update()
        {
            HandleMouseInput();
            
            // Handle Escape key
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                RestartGame();
            }
        }
        
        private void SetupCamera()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                GameObject cameraObj = new GameObject("Main Camera");
                mainCamera = cameraObj.AddComponent<Camera>();
                cameraObj.AddComponent<AudioListener>();
                cameraObj.tag = "MainCamera";
            }
            
            // Position camera at 45° angle
            mainCamera.transform.position = new Vector3(0, 2, -2);
            mainCamera.transform.rotation = Quaternion.Euler(45, 0, 0);
            mainCamera.fieldOfView = 45f;
        }
        
        private void CreateBoard()
        {
            gameBoard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameBoard.name = "GameBoard";
            gameBoard.transform.position = new Vector3(0, boardHeight, 0);
            gameBoard.transform.localScale = new Vector3(boardSize.x, 0.05f, boardSize.z);
            
            // Set board material
            Renderer boardRenderer = gameBoard.GetComponent<Renderer>();
            Material boardMaterial = new Material(Shader.Find("Standard"));
            boardMaterial.color = new Color(0.8f, 0.8f, 0.8f);
            boardRenderer.material = boardMaterial;
        }
        
        private void CreateHoles()
        {
            Color[] holeColors = { Color.red, Color.blue, Color.yellow };
            
            for (int i = 0; i < 3; i++)
            {
                GameObject hole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                hole.name = $"Hole_{holeColors[i].ToString()}";
                
                // Position holes across the board
                float spacing = boardSize.x / 4f;
                Vector3 holePosition = new Vector3(
                    -boardSize.x/2f + spacing * (i + 1),
                    boardHeight + 0.03f,
                    0
                );
                hole.transform.position = holePosition;
                
                // Scale hole based on diameter
                float scale = holeDiameters[i] / 100f;
                hole.transform.localScale = new Vector3(scale, 0.1f, scale);
                
                // Set hole color
                Renderer holeRenderer = hole.GetComponent<Renderer>();
                Material holeMaterial = new Material(Shader.Find("Standard"));
                holeMaterial.color = holeColors[i];
                holeMaterial.EnableKeyword("_EMISSION");
                holeMaterial.SetColor("_EmissionColor", holeColors[i] * 0.3f);
                holeRenderer.material = holeMaterial;
                
                // Add trigger collider
                SphereCollider holeCollider = hole.AddComponent<SphereCollider>();
                holeCollider.isTrigger = true;
                holeCollider.radius = 0.6f;
                
                holes.Add(hole);
            }
            
            Debug.Log($"Created {holes.Count} holes");
        }
        
        private void InitializeBallPool()
        {
            for (int i = 0; i < totalBalls + 5; i++)
            {
                GameObject ball = CreateBall();
                ball.name = $"Ball_Pooled_{i}";
                ball.SetActive(false);
                ballPool.Enqueue(ball);
            }
            
            Debug.Log($"Ball pool initialized with {ballPool.Count} balls");
        }
        
        private GameObject CreateBall()
        {
            GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.tag = "Ball";
            
            // Add Rigidbody
            Rigidbody rb = ball.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.drag = 0.5f;
            rb.angularDrag = 0.3f;
            
            // Add physics material
            SphereCollider collider = ball.GetComponent<SphereCollider>();
            PhysicMaterial physicsMaterial = new PhysicMaterial("BallPhysics");
            physicsMaterial.dynamicFriction = 0.3f;
            physicsMaterial.staticFriction = 0.3f;
            physicsMaterial.bounciness = 0.4f;
            collider.material = physicsMaterial;
            
            // Scale ball
            float scale = ballDiameter / 100f;
            ball.transform.localScale = Vector3.one * scale;
            
            // Add SimpleBall component
            ball.AddComponent<SimpleBall>();
            
            return ball;
        }
        
        public void StartSpawning()
        {
            if (isSpawning) return;
            
            isSpawning = true;
            ballsSpawned = 0;
            StartCoroutine(SpawnBallsCoroutine());
        }
        
        private IEnumerator SpawnBallsCoroutine()
        {
            while (ballsSpawned < totalBalls)
            {
                SpawnBall();
                ballsSpawned++;
                yield return new WaitForSeconds(spawnInterval);
            }
            
            isSpawning = false;
            Debug.Log("Finished spawning all balls");
        }
        
        private void SpawnBall()
        {
            if (ballPool.Count == 0) return;
            
            GameObject ball = ballPool.Dequeue();
            ball.SetActive(true);
            
            // Set random color
            BallColor randomColor = (BallColor)Random.Range(0, 3);
            SetBallColor(ball, randomColor);
            
            // Position ball
            Vector3 spawnPos = new Vector3(
                Random.Range(-boardSize.x/3f, boardSize.x/3f),
                boardHeight + 0.5f,
                boardSize.z/2f + 0.2f
            );
            ball.transform.position = spawnPos;
            
            // Apply spawn impulse
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            Vector3 randomDirection = new Vector3(
                Random.Range(-0.5f, 0.5f),
                0,
                Random.Range(-0.5f, 0.5f)
            ).normalized;
            rb.AddForce(randomDirection * 2f, ForceMode.Impulse);
            
            // Initialize ball data
            SimpleBall simpleBall = ball.GetComponent<SimpleBall>();
            simpleBall.Initialize(ballsSpawned + 1, randomColor, sessionId);
            
            balls.Add(ball);
            Debug.Log($"Spawned ball {ballsSpawned + 1}: {randomColor}");
        }
        
        private void SetBallColor(GameObject ball, BallColor color)
        {
            Renderer renderer = ball.GetComponent<Renderer>();
            Material ballMaterial = new Material(Shader.Find("Standard"));
            
            switch (color)
            {
                case BallColor.Red:
                    ballMaterial.color = Color.red;
                    break;
                case BallColor.Blue:
                    ballMaterial.color = Color.blue;
                    break;
                case BallColor.Yellow:
                    ballMaterial.color = Color.yellow;
                    break;
            }
            
            ballMaterial.SetFloat("_Glossiness", 0.8f);
            ballMaterial.SetFloat("_Metallic", 0.1f);
            renderer.material = ballMaterial;
        }
        
        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseDown();
            }
            else if (Input.GetMouseButton(0) && isDragging)
            {
                HandleMouseDrag();
            }
            else if (Input.GetMouseButtonUp(0) && isDragging)
            {
                HandleMouseUp();
            }
        }
        
        private void HandleMouseDown()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Ball"))
                {
                    draggedBall = hit.collider.gameObject;
                    isDragging = true;
                    
                    // Calculate drag offset
                    Vector3 mouseWorldPos = GetMouseWorldPosition();
                    dragOffset = draggedBall.transform.position - mouseWorldPos;
                    dragOffset.y = 0;
                    
                    // Make ball kinematic during drag
                    Rigidbody rb = draggedBall.GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                    
                    // Record grasp time
                    SimpleBall simpleBall = draggedBall.GetComponent<SimpleBall>();
                    simpleBall.ballData.graspTime = Time.time;
                    
                    Debug.Log($"Picked up ball {simpleBall.ballData.trialId}");
                }
            }
        }
        
        private void HandleMouseDrag()
        {
            if (draggedBall == null) return;
            
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            mouseWorldPos.y = boardHeight + 0.3f; // Maintain height during drag
            
            draggedBall.transform.position = mouseWorldPos + dragOffset;
        }
        
        private void HandleMouseUp()
        {
            if (draggedBall == null) return;
            
            Vector3 releasePosition = draggedBall.transform.position;
            SimpleBall simpleBall = draggedBall.GetComponent<SimpleBall>();
            
            // Record release time
            simpleBall.ballData.releaseTime = Time.time;
            simpleBall.ballData.completionTime = simpleBall.ballData.releaseTime - simpleBall.ballData.spawnTime;
            
            // Check placement
            bool validPlacement = CheckPlacement(draggedBall, releasePosition);
            
            // Re-enable physics
            Rigidbody rb = draggedBall.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            
            if (validPlacement)
            {
                HandleSuccessfulPlacement(draggedBall, releasePosition);
            }
            else
            {
                HandleFailedPlacement(draggedBall, releasePosition);
            }
            
            // Update statistics
            UpdateStatistics(simpleBall.ballData);
            
            draggedBall = null;
            isDragging = false;
        }
        
        private Vector3 GetMouseWorldPosition()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, new Vector3(0, boardHeight + 0.3f, 0));
            
            float distance;
            if (plane.Raycast(ray, out distance))
            {
                return ray.GetPoint(distance);
            }
            
            return Vector3.zero;
        }
        
        private bool CheckPlacement(GameObject ball, Vector3 position)
        {
            SimpleBall simpleBall = ball.GetComponent<SimpleBall>();
            BallColor ballColor = simpleBall.ballData.ballColor;
            
            // Find nearest compatible hole
            GameObject nearestHole = null;
            float nearestDistance = float.MaxValue;
            
            for (int i = 0; i < holes.Count; i++)
            {
                BallColor holeColor = (BallColor)i;
                if (holeColor == ballColor)
                {
                    float distance = Vector3.Distance(position, holes[i].transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestHole = holes[i];
                    }
                }
            }
            
            if (nearestHole != null)
            {
                float maxDistance = holeDiameters[(int)ballColor] / 200f; // Convert to Unity units
                simpleBall.ballData.placementErrorPx = nearestDistance * 100f;
                
                if (nearestDistance <= maxDistance)
                {
                    simpleBall.ballData.targetColor = ballColor;
                    simpleBall.ballData.targetSize = holeDiameters[(int)ballColor];
                    return true;
                }
            }
            
            return false;
        }
        
        private void HandleSuccessfulPlacement(GameObject ball, Vector3 position)
        {
            SimpleBall simpleBall = ball.GetComponent<SimpleBall>();
            simpleBall.ballData.outcome = PlacementOutcome.Success;
            
            // Snap to hole
            GameObject targetHole = holes[(int)simpleBall.ballData.ballColor];
            ball.transform.position = targetHole.transform.position;
            
            // Stop ball movement
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            
            Debug.Log($"Success! Ball {simpleBall.ballData.trialId} placed correctly");
        }
        
        private void HandleFailedPlacement(GameObject ball, Vector3 position)
        {
            SimpleBall simpleBall = ball.GetComponent<SimpleBall>();
            simpleBall.ballData.outcome = PlacementOutcome.Dropped;
            
            // Apply bounce
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            Vector3 bounceDirection = Vector3.up + Random.insideUnitSphere * 0.5f;
            rb.AddForce(bounceDirection * 3f, ForceMode.Impulse);
            
            Debug.Log($"Failed! Ball {simpleBall.ballData.trialId} placement failed");
        }
        
        private void UpdateStatistics(BallData ballData)
        {
            totalPlacements++;
            totalError += ballData.placementErrorPx;
            totalTime += ballData.completionTime;
            
            if (ballData.outcome == PlacementOutcome.Success)
            {
                successfulPlacements++;
            }
            
            // Display stats
            float accuracy = (successfulPlacements / (float)totalPlacements) * 100f;
            float avgError = totalError / totalPlacements;
            float avgTime = totalTime / totalPlacements;
            
            Debug.Log($"Stats - Accuracy: {accuracy:F1}%, Avg Error: {avgError:F1}px, Avg Time: {avgTime:F2}s");
            
            // Check level completion
            if (totalPlacements >= totalBalls)
            {
                CompleteLevel();
            }
        }
        
        private void CompleteLevel()
        {
            Debug.Log("=== LEVEL COMPLETE ===");
            Debug.Log($"Final Accuracy: {(successfulPlacements / (float)totalPlacements) * 100f:F1}%");
            Debug.Log($"Final Avg Error: {totalError / totalPlacements:F1}px");
            Debug.Log($"Final Avg Time: {totalTime / totalPlacements:F2}s");
            Debug.Log($"Success: {successfulPlacements}/{totalPlacements}");
            Debug.Log("Press Escape to restart");
        }
        
        public void RestartGame()
        {
            // Clear all balls
            foreach (GameObject ball in balls)
            {
                ball.SetActive(false);
                ballPool.Enqueue(ball);
            }
            balls.Clear();
            
            // Reset stats
            successfulPlacements = 0;
            totalPlacements = 0;
            totalError = 0f;
            totalTime = 0f;
            ballsSpawned = 0;
            levelStartTime = Time.time;
            sessionId = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            
            // Stop spawning
            StopAllCoroutines();
            isSpawning = false;
            
            // Restart spawning
            Invoke(nameof(StartSpawning), 1f);
            
            Debug.Log("Game restarted");
        }
    }
    
    public class SimpleBall : MonoBehaviour
    {
        public BallData ballData;
        
        public void Initialize(int id, BallColor color, string sessionId)
        {
            ballData = new BallData();
            ballData.sessionId = sessionId;
            ballData.trialId = id;
            ballData.ballColor = color;
            ballData.ballSize = 120f;
            ballData.spawnTime = Time.time;
        }
    }
}