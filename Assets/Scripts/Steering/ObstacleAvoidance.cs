using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Claw.AI.Steering {
    public class ObstacleAvoidance : SteeringBehaviour {

        private static readonly Vector2[] FEELERS = {
            new Vector2(-0.6f, 0.4f),
            new Vector2(0.0f, 1.0f),
            new Vector2(0.6f, 0.4f)
        };
        
        [SerializeField] private string obstacleLayer = "Obstacles";
        [SerializeField] private float feelerLength = 1.0f;
        private RaycastHit2D[] feelerHits = new RaycastHit2D[FEELERS.Length];    //left, middle, right

        protected override Vector2 DoForceCalculation() {
            Vector2 steeringDir = Vector2.zero;
            int hitCount = 0;
            float scaledLength = GetSpeedScaledLength();
            int layerMask = LayerMask.GetMask(obstacleLayer);
            Matrix4x4 worldMat = transform.localToWorldMatrix;

            for (int i = 0; i < FEELERS.Length; i++) {
                
                Vector2 feelerDir = worldMat.MultiplyVector(FEELERS[i]);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, feelerDir, scaledLength, layerMask);
                
                if (!hit) { continue; }
                hitCount++;
                
                float penetrationDepth = Mathf.Clamp(1.0f - hit.distance / scaledLength, 0.0f, 1.0f);
                steeringDir += hit.normal * penetrationDepth;
            }

            if (hitCount == 0) {
                return Vector2.zero;
            }

            steeringDir /= hitCount;
            
            return steeringDir * Controller.Speed;
        }

        //Return feeler length scaled based on the boid's speed.
        private float GetSpeedScaledLength() {
            return Mathf.Clamp(Rigidbody.velocity.magnitude / Controller.Speed,
                       0.0f, 1.0f) * feelerLength;
        }
        
        private void OnDrawGizmosSelected() {
            if (!enabled) { return; }

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;

            if (!Application.isPlaying) {
                Gizmos.color = Color.yellow;
                for (int i = 0; i < FEELERS.Length; i++) {
                    Gizmos.DrawLine(Vector2.zero, FEELERS[i] * feelerLength);
                }
            }
            else {
                float scaledLength = GetSpeedScaledLength();
                for (int i = 0; i < FEELERS.Length; i++) {
                    Gizmos.color = feelerHits[i] ? Color.red : Color.yellow;
                    Gizmos.DrawLine(Vector2.zero, FEELERS[i] * scaledLength);
                }
            }
        }
    }
}