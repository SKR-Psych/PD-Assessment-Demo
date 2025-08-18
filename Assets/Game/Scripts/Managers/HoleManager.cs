using System.Collections.Generic;
using UnityEngine;
using SortingBoardGame.Data;
using SortingBoardGame.Gameplay;

namespace SortingBoardGame.Managers
{
    public class HoleManager : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private GameObject holePrefab;
        [SerializeField] private GameBoard gameBoard;
        
        private List<Hole> holes = new List<Hole>();
        private List<HoleConfig> holeConfigs = new List<HoleConfig>();
        
        void Start()
        {
            if (gameConfig == null)
            {
                gameConfig = GameManager.Instance?.Config;
            }
            CreateHoleConfigurations();
            CreateHoles();
        }
        
        private void CreateHoleConfigurations()
        {
            if (gameConfig == null)
            {
                Debug.LogError("GameConfig not assigned to HoleManager!");
                return;
            }
            
            // Create hole configurations based on GameConfig
            Vector3 boardCenter = gameBoard != null ? gameBoard.GetBoardCenter() : Vector3.zero;
            Vector3 boardSize = gameBoard != null ? gameBoard.GetBoardSize() : (gameConfig != null ? gameConfig.boardSize : new Vector3(1.0f, 0.0f, 0.6f));
            
            // Position holes evenly across the board
            float spacing = boardSize.x / 4f; // Divide board width by 4 for 3 holes with spacing
            
            for (int i = 0; i < 3; i++)
            {
                Vector3 holePosition = boardCenter;
                holePosition.x = boardCenter.x - boardSize.x/2f + spacing * (i + 1);
                holePosition.y = boardCenter.y + 0.03f; // Slightly above board surface
                
                HoleConfig config = new HoleConfig(
                    holePosition,
                    gameConfig.holeDiameters[i],
                    (BallColor)i // Red=0, Blue=1, Yellow=2
                );
                
                holeConfigs.Add(config);
            }
        }
        
        private void CreateHoles()
        {
            foreach (HoleConfig config in holeConfigs)
            {
                GameObject holeObj;
                
                if (holePrefab != null)
                {
                    holeObj = Instantiate(holePrefab, config.position, Quaternion.identity, transform);
                }
                else
                {
                    // Create basic hole using primitive
                    holeObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    holeObj.transform.position = config.position;
                    holeObj.transform.parent = transform;
                }
                
                holeObj.name = $"Hole_{config.color}_{config.diameter}px";
                
                // Add Hole component and initialize
                Hole hole = holeObj.GetComponent<Hole>();
                if (hole == null)
                {
                    hole = holeObj.AddComponent<Hole>();
                }
                
                hole.Initialize(config);
                holes.Add(hole);
            }
            
            Debug.Log($"Created {holes.Count} holes on the game board");
        }
        
        public Hole GetNearestCompatibleHole(Vector3 ballPosition, BallColor ballColor, float ballSize)
        {
            Hole nearestHole = null;
            float nearestDistance = float.MaxValue;
            
            foreach (Hole hole in holes)
            {
                if (hole.CanAcceptBall(ballColor, ballSize))
                {
                    float distance = hole.GetDistanceFromCenter(ballPosition);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestHole = hole;
                    }
                }
            }
            
            return nearestHole;
        }
        
        public void HighlightCompatibleHoles(BallColor ballColor, float ballSize, bool highlight)
        {
            foreach (Hole hole in holes)
            {
                if (hole.CanAcceptBall(ballColor, ballSize))
                {
                    hole.SetHighlight(highlight);
                }
            }
        }
        
        public bool IsValidPlacement(Vector3 ballPosition, BallColor ballColor, float ballSize, out Hole targetHole, out float placementError)
        {
            targetHole = GetNearestCompatibleHole(ballPosition, ballColor, ballSize);
            placementError = 0f;
            
            if (targetHole == null)
            {
                return false;
            }
            
            placementError = targetHole.GetDistanceFromCenter(ballPosition);
            
            // Consider placement valid if ball is close enough to hole center
            float maxPlacementDistance = targetHole.HoleDiameter / 200f; // Convert pixels to Unity units
            return placementError <= maxPlacementDistance;
        }
        
        public List<Hole> GetAllHoles()
        {
            return new List<Hole>(holes);
        }
    }
}