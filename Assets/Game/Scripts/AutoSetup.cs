using UnityEngine;
using SortingBoardGame.Managers;
using SortingBoardGame.Gameplay;
using SortingBoardGame.UI;

namespace SortingBoardGame
{
    public class AutoSetup : MonoBehaviour
    {
        [Header("Auto Setup")]
        [SerializeField] private bool setupOnStart = true;
        
        void Start()
        {
            if (setupOnStart)
            {
                SetupGame();
            }
        }
        
        public void SetupGame()
        {
            Debug.Log("Auto-setting up game components...");
            
            // Create GameManager if it doesn't exist
            if (GameManager.Instance == null)
            {
                GameObject gameManagerObj = new GameObject("GameManager");
                gameManagerObj.AddComponent<GameManager>();
            }
            
            // Create AudioManager if it doesn't exist
            if (AudioManager.Instance == null)
            {
                GameObject audioManagerObj = new GameObject("AudioManager");
                audioManagerObj.AddComponent<AudioManager>();
            }
            
            // Create StatisticsManager if it doesn't exist
            if (StatisticsManager.Instance == null)
            {
                GameObject statsManagerObj = new GameObject("StatisticsManager");
                statsManagerObj.AddComponent<StatisticsManager>();
            }
            
            // Create DataLogger if it doesn't exist
            if (DataLogger.Instance == null)
            {
                GameObject dataLoggerObj = new GameObject("DataLogger");
                dataLoggerObj.AddComponent<DataLogger>();
            }
            
            // Create GameBoard if it doesn't exist
            if (FindObjectOfType<GameBoard>() == null)
            {
                GameObject gameBoardObj = new GameObject("GameBoard");
                gameBoardObj.AddComponent<GameBoard>();
            }
            
            // Create HoleManager if it doesn't exist
            if (FindObjectOfType<HoleManager>() == null)
            {
                GameObject holeManagerObj = new GameObject("HoleManager");
                holeManagerObj.AddComponent<HoleManager>();
            }
            
            // Create BallSpawner if it doesn't exist
            if (FindObjectOfType<BallSpawner>() == null)
            {
                GameObject ballSpawnerObj = new GameObject("BallSpawner");
                ballSpawnerObj.AddComponent<BallSpawner>();
            }
            
            // Create MouseInteractionSystem if it doesn't exist
            if (FindObjectOfType<MouseInteractionSystem>() == null)
            {
                GameObject mouseSystemObj = new GameObject("MouseInteractionSystem");
                mouseSystemObj.AddComponent<MouseInteractionSystem>();
            }
            
            // Create BallInteractionHandler if it doesn't exist
            if (FindObjectOfType<BallInteractionHandler>() == null)
            {
                GameObject interactionHandlerObj = new GameObject("BallInteractionHandler");
                interactionHandlerObj.AddComponent<BallInteractionHandler>();
            }
            
            // Create VisualEffectsManager if it doesn't exist
            if (FindObjectOfType<VisualEffectsManager>() == null)
            {
                GameObject visualEffectsObj = new GameObject("VisualEffectsManager");
                visualEffectsObj.AddComponent<VisualEffectsManager>();
            }
            
            // Create HUDManager if it doesn't exist
            if (FindObjectOfType<HUDManager>() == null)
            {
                GameObject hudManagerObj = new GameObject("HUDManager");
                hudManagerObj.AddComponent<HUDManager>();
            }
            
            // Create LevelController if it doesn't exist
            if (FindObjectOfType<SortingBoardGame.Managers.LevelController>() == null)
            {
                GameObject levelControllerObj = new GameObject("LevelController");
                levelControllerObj.AddComponent<SortingBoardGame.Managers.LevelController>();
            }
            
            Debug.Log("Game auto-setup complete!");
        }
    }
}