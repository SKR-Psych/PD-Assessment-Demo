using UnityEngine;
using SortingBoardGame.Managers;

namespace SortingBoardGame
{
    public class GameTest : MonoBehaviour
    {
        void Start()
        {
            // Wait a few seconds then run tests
            Invoke(nameof(RunTests), 3f);
        }
        
        void Update()
        {
            // Test hotkeys
            if (Input.GetKeyDown(KeyCode.T))
            {
                RunTests();
            }
        }
        
        void RunTests()
        {
            Debug.Log("=== RUNNING GAME TESTS ===");
            
            // Test GameManager
            if (GameManager.Instance != null)
            {
                Debug.Log("✓ GameManager found");
            }
            else
            {
                Debug.LogError("✗ GameManager missing");
            }
            
            // Test AudioManager
            if (AudioManager.Instance != null)
            {
                Debug.Log("✓ AudioManager found");
            }
            else
            {
                Debug.LogError("✗ AudioManager missing");
            }
            
            // Test StatisticsManager
            if (StatisticsManager.Instance != null)
            {
                Debug.Log("✓ StatisticsManager found");
            }
            else
            {
                Debug.LogError("✗ StatisticsManager missing");
            }
            
            // Test DataLogger
            if (DataLogger.Instance != null)
            {
                Debug.Log("✓ DataLogger found");
            }
            else
            {
                Debug.LogError("✗ DataLogger missing");
            }
            
            // Test other components
            var gameBoard = FindObjectOfType<SortingBoardGame.Gameplay.GameBoard>();
            if (gameBoard != null)
            {
                Debug.Log("✓ GameBoard found");
            }
            else
            {
                Debug.LogError("✗ GameBoard missing");
            }
            
            var holeManager = FindObjectOfType<HoleManager>();
            if (holeManager != null)
            {
                Debug.Log("✓ HoleManager found");
            }
            else
            {
                Debug.LogError("✗ HoleManager missing");
            }
            
            var ballSpawner = FindObjectOfType<BallSpawner>();
            if (ballSpawner != null)
            {
                Debug.Log("✓ BallSpawner found");
            }
            else
            {
                Debug.LogError("✗ BallSpawner missing");
            }
            
            Debug.Log("=== TESTS COMPLETE ===");
        }
    }
}