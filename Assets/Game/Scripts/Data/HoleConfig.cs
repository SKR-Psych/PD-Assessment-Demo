using UnityEngine;

namespace SortingBoardGame.Data
{
    [System.Serializable]
    public class HoleConfig
    {
        public Vector3 position;
        public float diameter; // 160px, 140px, or 120px
        public BallColor color; // Red, Blue, or Yellow
        public Material holeMaterial;
        
        public HoleConfig(Vector3 pos, float dia, BallColor col)
        {
            position = pos;
            diameter = dia;
            color = col;
            holeMaterial = null;
        }
    }
}