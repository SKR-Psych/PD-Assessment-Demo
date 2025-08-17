using UnityEngine;
using SortingBoardGame.Data;

namespace SortingBoardGame.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [SerializeField] private GameConfig gameConfig;
        
        public GameConfig Config => gameConfig;
        
        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeGame()
        {
            // Set target frame rate
            Application.targetFrameRate = gameConfig.targetFPS;
            
            // Set screen resolution for Windows build
            if (!Application.isEditor)
            {
                Screen.SetResolution(gameConfig.targetResolutionWidth, gameConfig.targetResolutionHeight, false);
            }
            
            // Initialize other systems
            Debug.Log("Game Manager initialized - Sorting Board Game v1.0");
        }
        
        void Update()
        {
            // Handle Escape key for reset/exit
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleEscapeKey();
            }
        }
        
        private void HandleEscapeKey()
        {
            // For now, just log - will implement level reset/exit later
            Debug.Log("Escape key pressed - Level reset/exit functionality");
        }
        
        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}