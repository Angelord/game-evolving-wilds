using System;
using UnityEngine;

namespace Claw.AI.Steering {
    public class Hide : SteeringBehaviour {

        [SerializeField] private Rigidbody2D pursuer;
        [SerializeField] private string obstacleLayer = "Obstacles";
        [SerializeField] private float maxHideDistance = 10.0f;
        [SerializeField] private float distanceFromBoundary = 1.0f;
        private Evade evade;
        private Arrive arrive;
        private Collider2D[] obstaclesInRange;
        
        protected override void OnInitialize() {
            evade = RequireBehaviour<Evade>();
            evade.Pursuer = pursuer;
            arrive = RequireBehaviour<Arrive>();
            obstaclesInRange = new Collider2D[Mathf.RoundToInt(maxHideDistance)];
        }

        protected override Vector2 DoForceCalculation() {

            if (pursuer == null) {
                return Vector2.zero;
            }

            int layerMask = LayerMask.GetMask(obstacleLayer);
            int obstacleCount = GetNearbyObjects(maxHideDistance, obstaclesInRange, layerMask);
            
            float distanceToNearest = float.MaxValue;
            Vector2 nearest = Vector2.zero;
            bool validPosExists = false;
            for (int i = 0; i < obstacleCount; i++) {
                Vector2 hidePos = GetHidingPosition(obstaclesInRange[i]);
                float distance = Vector2.Distance(transform.position, hidePos);
                if (distance < distanceToNearest) {
                    distanceToNearest = distance;
                    nearest = hidePos;
                    validPosExists = true;
                }
            }
            
            if (validPosExists) { 
                return evade.CalculateForce();
            }
            
            return arrive.CalculateForce(nearest);
        }

        private Vector2 GetHidingPosition(Collider2D obstacle) {

            Vector2 boundsSz = obstacle.bounds.size;
            float boundsMax = Mathf.Max(boundsSz.x, boundsSz.y);

            float distAway = boundsMax + distanceFromBoundary;
            Vector2 toObst = obstacle.transform.position - pursuer.transform.position;
            toObst.Normalize();

            return (toObst * distAway) + (Vector2)obstacle.transform.position;
        }

        private void OnDrawGizmosSelected() {
            if (!enabled) { return; }

            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(transform.position, maxHideDistance);
        }
    }
}