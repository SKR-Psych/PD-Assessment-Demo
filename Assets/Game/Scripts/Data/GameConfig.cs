using UnityEngine;

namespace SortingBoardGame.Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Sorting Board Game/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Level 1 Configuration")]
        public int totalBalls = 20;
        public float spawnInterval = 2.0f;
        public float ballDiameter = 120f; // pixels
        
        [Header("Board Configuration")]
        public Vector3 boardSize = new Vector3(1.0f, 0.0f, 0.6f); // 1.0m × 0.6m
        public float boardHeight = 0.9f; // y = 0.9m waist height
        
        [Header("Hole Configuration")]
        public float[] holeDiameters = { 160f, 140f, 120f }; // pixels
        public Color[] holeColors = { Color.red, Color.blue, Color.yellow };
        
        [Header("Camera Configuration")]
        public float cameraAngle = 45f;
        public float cameraFOV = 45f;
        
        [Header("Performance")]
        public int targetFPS = 60;
        public int targetResolutionWidth = 1920;
        public int targetResolutionHeight = 1080;
    }
}