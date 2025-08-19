using UnityEngine;
using SortingBoardGame.Data;
using SortingBoardGame.Managers;

namespace SortingBoardGame.Gameplay
{
    public class GameBoard : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private Material boardMaterial;
        
        private GameObject boardObject;
        
        void Start()
        {
            if (gameConfig == null)
            {
                gameConfig = GameManager.Instance?.Config;
            }
            CreateBoard();
        }
        
        private void CreateBoard()
        {
            if (gameConfig == null)
            {
                Debug.LogError("GameConfig is null in GameBoard!");
                return;
            }
            
            // Create board GameObject
            boardObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boardObject.name = "GameBoard";
            boardObject.transform.parent = transform;
            
            // Set board dimensions: 1.0m × 0.6m at y = 0.9m
            Vector3 boardSize = gameConfig.boardSize;
            boardSize.y = 0.05f; // Small height for the board surface
            boardObject.transform.localScale = boardSize;
            boardObject.transform.position = new Vector3(0, gameConfig.boardHeight, 0);
            
            // Apply neutral colored material
            Renderer boardRenderer = boardObject.GetComponent<Renderer>();
            if (boardMaterial != null)
            {
                boardRenderer.material = boardMaterial;
            }
            else
            {
                // Create default neutral material
                Material defaultMaterial = new Material(Shader.Find("Standard"));
                defaultMaterial.color = new Color(0.8f, 0.8f, 0.8f); // Light gray
                boardRenderer.material = defaultMaterial;
            }
            
            // Add collider for physics interactions
            BoxCollider boardCollider = boardObject.GetComponent<BoxCollider>();
            boardCollider.isTrigger = false;
            
            Debug.Log($"Game board created: {boardSize.x}m × {boardSize.z}m at height {gameConfig.boardHeight}m");
        }
        
        public Vector3 GetBoardCenter()
        {
            return boardObject != null ? boardObject.transform.position : Vector3.zero;
        }
        
        public Vector3 GetBoardSize()
        {
            return gameConfig != null ? gameConfig.boardSize : new Vector3(1.0f, 0.0f, 0.6f);
        }
    }
}