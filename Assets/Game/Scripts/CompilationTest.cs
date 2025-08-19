using UnityEngine;
using UnityEngine.UI;
using SortingBoardGame.Data;
using SortingBoardGame.Managers;
using SortingBoardGame.Gameplay;
using SortingBoardGame.UI;
using SortingBoardGame.Setup;

namespace SortingBoardGame
{
    /// <summary>
    /// Test script to verify all classes compile correctly
    /// </summary>
    public class CompilationTest : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("Compilation Test: All classes compiled successfully!");
            
            // Test that all main classes can be referenced
            TestDataClasses();
            TestManagerClasses();
            TestGameplayClasses();
            TestUIClasses();
            TestSetupClasses();
        }
        
        private void TestDataClasses()
        {
            // Test Data namespace classes
            GameConfig config = ScriptableObject.CreateInstance<GameConfig>();
            BallData ballData = new BallData();
            HoleConfig holeConfig = new HoleConfig(Vector3.zero, 120f, BallColor.Red);
            
            Debug.Log("✓ Data classes compile correctly");
        }
        
        private void TestManagerClasses()
        {
            // Test Manager namespace classes - these should be singletons or findable
            bool managersExist = true;
            
            // These are tested by checking if the class types exist
            System.Type gameManagerType = typeof(GameManager);
            System.Type audioManagerType = typeof(AudioManager);
            System.Type statisticsManagerType = typeof(StatisticsManager);
            System.Type dataLoggerType = typeof(DataLogger);
            System.Type ballSpawnerType = typeof(BallSpawner);
            System.Type holeManagerType = typeof(HoleManager);
            System.Type mouseInteractionType = typeof(MouseInteractionSystem);
            System.Type visualEffectsType = typeof(VisualEffectsManager);
            System.Type levelControllerType = typeof(LevelController);
            System.Type performanceManagerType = typeof(PerformanceManager);
            System.Type gameTesterType = typeof(GameTester);
            System.Type cameraControllerType = typeof(CameraController);
            System.Type sceneOrchestratorType = typeof(SceneOrchestrator);
            System.Type sceneSetupType = typeof(SceneSetup);
            
            Debug.Log("✓ Manager classes compile correctly");
        }
        
        private void TestGameplayClasses()
        {
            // Test Gameplay namespace classes
            System.Type ballType = typeof(Ball);
            System.Type gameBoardType = typeof(GameBoard);
            System.Type holeType = typeof(Hole);
            System.Type ballInteractionHandlerType = typeof(BallInteractionHandler);
            
            Debug.Log("✓ Gameplay classes compile correctly");
        }
        
        private void TestUIClasses()
        {
            // Test UI namespace classes
            System.Type hudManagerType = typeof(HUDManager);
            System.Type endLevelUIType = typeof(EndLevelUI);
            System.Type placementErrorIndicatorType = typeof(PlacementErrorIndicator);
            
            Debug.Log("✓ UI classes compile correctly");
        }
        
        private void TestSetupClasses()
        {
            // Test Setup namespace classes
            System.Type quickGameSetupType = typeof(QuickGameSetup);
            
            Debug.Log("✓ Setup classes compile correctly");
        }
        
        private void TestEnums()
        {
            // Test all enums
            BallColor ballColor = BallColor.Red;
            BallState ballState = BallState.Idle;
            PlacementOutcome outcome = PlacementOutcome.Success;
            GameState gameState = GameState.Playing;
            
            Debug.Log("✓ Enums compile correctly");
        }
        
        [ContextMenu("Run Compilation Test")]
        public void RunCompilationTest()
        {
            Start();
        }
    }
}