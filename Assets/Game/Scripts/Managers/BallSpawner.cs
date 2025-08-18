using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SortingBoardGame.Data;
using SortingBoardGame.Gameplay;

namespace SortingBoardGame.Managers
{
    public class BallSpawner : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private GameBoard gameBoard;
        
        private Queue<Ball> ballPool = new Queue<Ball>();
        private List<Ball> activeBalls = new List<Ball>();
        private Coroutine spawningCoroutine;
        
        private int ballsSpawned = 0;
        private int maxBalls = 20; // Level 1: 20 balls total
        private string currentSessionId;
        
        public bool IsSpawning { get; private set; } = false;
        public int BallsSpawned => ballsSpawned;
        public int MaxBalls => maxBalls;
        public List<Ball> ActiveBalls => new List<Ball>(activeBalls);
        
        void Start()
        {
            if (gameConfig == null)
            {
                gameConfig = GameManager.Instance?.Config;
            }
            
            currentSessionId = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            maxBalls = gameConfig != null ? gameConfig.totalBalls : 20;
            InitializeBallPool();
        }
        
        private void InitializeBallPool()
        {
            // Pre-create balls for object pooling
            int poolSize = maxBalls + 5; // Extra balls for safety
            
            for (int i = 0; i < poolSize; i++)
            {
                GameObject ballObj;
                
                if (ballPrefab != null)
                {
                    ballObj = Instantiate(ballPrefab, transform);
                }
                else
                {
                    // Create basic ball using primitive
                    ballObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    ballObj.transform.parent = transform;
                    ballObj.AddComponent<Ball>();
                }
                
                ballObj.name = $"Ball_Pooled_{i}";
                ballObj.SetActive(false);
                
                Ball ball = ballObj.GetComponent<Ball>();
                if (ball == null)
                {
                    ball = ballObj.AddComponent<Ball>();
                }
                
                ballPool.Enqueue(ball);
            }
            
            Debug.Log($"Ball pool initialized with {poolSize} balls");
        }
        
        public void StartSpawning()
        {
            if (IsSpawning)
            {
                Debug.LogWarning("Ball spawning already in progress!");
                return;
            }
            
            IsSpawning = true;
            ballsSpawned = 0;
            spawningCoroutine = StartCoroutine(SpawnBallsCoroutine());
            
            Debug.Log($"Started spawning {maxBalls} balls at {(gameConfig != null ? gameConfig.spawnInterval : 2f)}s intervals");
        }
        
        public void StopSpawning()
        {
            if (spawningCoroutine != null)
            {
                StopCoroutine(spawningCoroutine);
                spawningCoroutine = null;
            }
            
            IsSpawning = false;
            Debug.Log("Ball spawning stopped");
        }
        
        private IEnumerator SpawnBallsCoroutine()
        {
            while (ballsSpawned < maxBalls)
            {
                SpawnBall();
                ballsSpawned++;
                
                // Wait for spawn interval (2 seconds for Level 1)
                yield return new WaitForSeconds(gameConfig != null ? gameConfig.spawnInterval : 2f);
            }
            
            IsSpawning = false;
            Debug.Log($"Finished spawning all {maxBalls} balls");
        }
        
        private void SpawnBall()
        {
            if (ballPool.Count == 0)
            {
                Debug.LogError("Ball pool is empty! Cannot spawn more balls.");
                return;
            }
            
            // Get ball from pool
            Ball ball = ballPool.Dequeue();
            ball.gameObject.SetActive(true);
            
            // Assign random color (red, blue, or yellow)
            BallColor randomColor = (BallColor)Random.Range(0, 3);
            
            // Initialize ball
            ball.Initialize(ballsSpawned + 1, randomColor, currentSessionId);
            
            // Position ball at spawn location
            Vector3 spawnPosition = GetSpawnPosition();
            ball.ApplySpawnImpulse(spawnPosition);
            
            // Add to active balls list
            activeBalls.Add(ball);
            
            Debug.Log($"Spawned ball {ballsSpawned + 1}: {randomColor} color at {spawnPosition}");
        }
        
        private Vector3 GetSpawnPosition()
        {
            Vector3 boardCenter = gameBoard != null ? gameBoard.GetBoardCenter() : Vector3.zero;
            Vector3 boardSize = gameBoard != null ? gameBoard.GetBoardSize() : (gameConfig != null ? gameConfig.boardSize : new Vector3(1.0f, 0.0f, 0.6f));
            
            // Spawn balls at random positions above the board edge
            Vector3 spawnPos = boardCenter;
            spawnPos.x += Random.Range(-boardSize.x/3f, boardSize.x/3f);
            spawnPos.y += 0.5f; // Above the board
            spawnPos.z = boardCenter.z + boardSize.z/2f + 0.2f; // Behind the board
            
            return spawnPos;
        }
        
        public void ReturnBallToPool(Ball ball)
        {
            if (ball == null) return;
            
            // Remove from active balls
            activeBalls.Remove(ball);
            
            // Reset ball state
            ball.gameObject.SetActive(false);
            ball.transform.position = Vector3.zero;
            ball.SetState(BallState.Spawning);
            
            // Return to pool
            ballPool.Enqueue(ball);
        }
        
        public void ClearAllBalls()
        {
            // Return all active balls to pool
            for (int i = activeBalls.Count - 1; i >= 0; i--)
            {
                ReturnBallToPool(activeBalls[i]);
            }
            
            activeBalls.Clear();
            Debug.Log("All balls cleared and returned to pool");
        }
        
        public Ball GetBallById(int ballId)
        {
            foreach (Ball ball in activeBalls)
            {
                if (ball.BallId == ballId)
                {
                    return ball;
                }
            }
            return null;
        }
        
        public bool AllBallsPlaced()
        {
            if (ballsSpawned < maxBalls) return false;
            
            foreach (Ball ball in activeBalls)
            {
                if (ball.State != BallState.Placed && ball.State != BallState.Failed)
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}