using UnityEngine;
using SortingBoardGame.Data;
using SortingBoardGame.Managers;

namespace SortingBoardGame.Gameplay
{
    public class BallInteractionHandler : MonoBehaviour
    {
        [SerializeField] private MouseInteractionSystem mouseSystem;
        [SerializeField] private HoleManager holeManager;
        [SerializeField] private BallSpawner ballSpawner;
        [SerializeField] private VisualEffectsManager visualEffectsManager;
        [SerializeField] private StatisticsManager statisticsManager;
        
        void Start()
        {
            if (mouseSystem == null)
                mouseSystem = FindObjectOfType<MouseInteractionSystem>();
            
            if (holeManager == null)
                holeManager = FindObjectOfType<HoleManager>();
            
            if (ballSpawner == null)
                ballSpawner = FindObjectOfType<BallSpawner>();
            
            if (visualEffectsManager == null)
                visualEffectsManager = FindObjectOfType<VisualEffectsManager>();
            
            if (statisticsManager == null)
                statisticsManager = StatisticsManager.Instance;
            
            // Subscribe to mouse interaction events
            if (mouseSystem != null)
            {
                mouseSystem.OnBallHoverStart += HandleBallHoverStart;
                mouseSystem.OnBallHoverEnd += HandleBallHoverEnd;
                mouseSystem.OnBallPickup += HandleBallPickup;
                mouseSystem.OnBallDrag += HandleBallDrag;
                mouseSystem.OnBallRelease += HandleBallRelease;
            }
        }
        
        void OnDestroy()
        {
            // Unsubscribe from events
            if (mouseSystem != null)
            {
                mouseSystem.OnBallHoverStart -= HandleBallHoverStart;
                mouseSystem.OnBallHoverEnd -= HandleBallHoverEnd;
                mouseSystem.OnBallPickup -= HandleBallPickup;
                mouseSystem.OnBallDrag -= HandleBallDrag;
                mouseSystem.OnBallRelease -= HandleBallRelease;
            }
        }
        
        private void HandleBallHoverStart(Ball ball)
        {
            if (ball == null || holeManager == null) return;
            
            // Highlight compatible holes when hovering over a ball
            holeManager.HighlightCompatibleHoles(ball.Color, ball.Size, true);
            
            Debug.Log($"Hovering over {ball.Color} ball {ball.BallId}");
        }
        
        private void HandleBallHoverEnd(Ball ball)
        {
            if (ball == null || holeManager == null) return;
            
            // Remove hole highlighting when not hovering
            holeManager.HighlightCompatibleHoles(ball.Color, ball.Size, false);
        }
        
        private void HandleBallPickup(Ball ball)
        {
            if (ball == null) return;
            
            // Record grasp time in ball data
            ball.Data.graspTime = Time.time;
            
            Debug.Log($"Picked up {ball.Color} ball {ball.BallId} at time {ball.Data.graspTime}");
        }
        
        private void HandleBallDrag(Ball ball, Vector3 position)
        {
            if (ball == null || holeManager == null) return;
            
            // Check if ball is near a compatible hole and highlight it
            Hole nearestHole = holeManager.GetNearestCompatibleHole(position, ball.Color, ball.Size);
            
            if (nearestHole != null)
            {
                float distance = nearestHole.GetDistanceFromCenter(position);
                float highlightDistance = nearestHole.HoleDiameter / 100f; // Convert pixels to Unity units
                
                // Highlight hole if ball is close enough
                bool shouldHighlight = distance <= highlightDistance;
                nearestHole.SetHighlight(shouldHighlight);
                
                // Remove highlighting from other holes if this one is highlighted
                if (shouldHighlight)
                {
                    foreach (Hole hole in holeManager.GetAllHoles())
                    {
                        if (hole != nearestHole)
                        {
                            hole.SetHighlight(false);
                        }
                    }
                }
            }
        }
        
        private void HandleBallRelease(Ball ball, Vector3 releasePosition)
        {
            if (ball == null || holeManager == null) return;
            
            // Record release time
            ball.Data.releaseTime = Time.time;
            ball.Data.completionTime = ball.Data.releaseTime - ball.Data.spawnTime;
            
            // Check if placement is valid
            Hole targetHole;
            float placementError;
            bool isValidPlacement = holeManager.IsValidPlacement(
                releasePosition, 
                ball.Color, 
                ball.Size, 
                out targetHole, 
                out placementError
            );
            
            // Update ball data with placement information
            ball.Data.placementErrorPx = placementError * 100f; // Convert to pixels
            
            if (isValidPlacement && targetHole != null)
            {
                // Successful placement
                HandleSuccessfulPlacement(ball, targetHole, placementError);
            }
            else
            {
                // Failed placement
                HandleFailedPlacement(ball, releasePosition);
            }
            
            // Record ball placement in statistics
            if (statisticsManager != null)
            {
                statisticsManager.RecordBallPlacement(ball);
            }
            
            // Clear all hole highlighting
            holeManager.HighlightCompatibleHoles(ball.Color, ball.Size, false);
            
            Debug.Log($"Released {ball.Color} ball {ball.BallId} at {releasePosition} - " +
                     $"Valid: {isValidPlacement}, Error: {placementError:F2}");
        }
        
        private void HandleSuccessfulPlacement(Ball ball, Hole targetHole, float placementError)
        {
            // Update ball data
            ball.Data.targetColor = targetHole.HoleColor;
            ball.Data.targetSize = targetHole.HoleDiameter;
            ball.Data.outcome = PlacementOutcome.Success;
            
            // Snap ball to hole position
            Vector3 snapPosition = targetHole.HolePosition;
            ball.SnapToHole(snapPosition);
            
            // Trigger success visual feedback
            if (visualEffectsManager != null)
            {
                visualEffectsManager.PlaySuccessEffect(snapPosition, ball.Color);
                visualEffectsManager.ShowPlacementErrorIndicator(snapPosition, ball.Data.placementErrorPx, true);
            }
            
            // Trigger success audio feedback
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySuccessSound(snapPosition);
            }
            
            Debug.Log($"Successful placement! Ball {ball.BallId} placed in {targetHole.HoleColor} hole " +
                     $"with {placementError:F2} error");
        }
        
        private void HandleFailedPlacement(Ball ball, Vector3 releasePosition)
        {
            // Update ball data
            ball.Data.outcome = PlacementOutcome.Dropped;
            
            // Apply bounce effect
            Vector3 bounceDirection = Vector3.up + Random.insideUnitSphere * 0.5f;
            ball.BounceOff(bounceDirection);
            
            // Trigger failure visual feedback
            if (visualEffectsManager != null)
            {
                visualEffectsManager.PlayFailureEffect(releasePosition);
                visualEffectsManager.ShowPlacementErrorIndicator(releasePosition, ball.Data.placementErrorPx, false);
            }
            
            // Trigger failure audio feedback
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayFailureSound(releasePosition);
            }
            
            Debug.Log($"Failed placement! Ball {ball.BallId} dropped at {releasePosition}");
        }
        
        // Public method to check if all balls are placed (for level completion)
        public bool AreAllBallsPlaced()
        {
            if (ballSpawner == null) return false;
            
            return ballSpawner.AllBallsPlaced();
        }
        
        // Public method to get placement statistics
        public (int successful, int failed, int total) GetPlacementStats()
        {
            if (ballSpawner == null) return (0, 0, 0);
            
            int successful = 0;
            int failed = 0;
            int total = 0;
            
            foreach (Ball ball in ballSpawner.ActiveBalls)
            {
                if (ball.State == BallState.Placed || ball.State == BallState.Failed)
                {
                    total++;
                    if (ball.Data.outcome == PlacementOutcome.Success)
                        successful++;
                    else
                        failed++;
                }
            }
            
            return (successful, failed, total);
        }
    }
}