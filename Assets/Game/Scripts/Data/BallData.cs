using System;
using UnityEngine;

namespace SortingBoardGame.Data
{
    public enum BallColor
    {
        Red,
        Blue,
        Yellow
    }
    
    public enum PlacementOutcome
    {
        Success,
        WrongHole,
        Dropped,
        Timeout
    }
    
    [System.Serializable]
    public class BallData
    {
        public string sessionId;
        public int trialId;
        public float spawnTime;
        public float graspTime;
        public float releaseTime;
        public float completionTime;
        public BallColor ballColor;
        public float ballSize = 120f; // 120px diameter
        public BallColor targetColor;
        public float targetSize;
        public float placementErrorPx;
        public PlacementOutcome outcome;
        
        public BallData()
        {
            sessionId = "";
            trialId = 0;
            spawnTime = 0f;
            graspTime = 0f;
            releaseTime = 0f;
            completionTime = 0f;
            ballColor = BallColor.Red;
            ballSize = 120f;
            targetColor = BallColor.Red;
            targetSize = 120f;
            placementErrorPx = 0f;
            outcome = PlacementOutcome.Success;
        }
    }
}