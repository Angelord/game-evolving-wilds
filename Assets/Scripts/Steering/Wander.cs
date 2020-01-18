using UnityEngine;
using Random = UnityEngine.Random;

namespace Claw.AI.Steering {
    public class Wander : SteeringBehaviour {

        [SerializeField] private float wanderRadius = 1.2f;
        [SerializeField] private float wanderDistance = 2.5f;
        [SerializeField] private float wanderJitter = 0.1f;
        private Vector2 wanderTarget;
        private Seek seek;
        private Vector2 target;

        protected override void OnInitialize() {
            wanderTarget = new Vector2(
                Random.Range(-1.0f, 1.0f),
                Random.Range(-1.0f, 1.0f));
            seek = RequireBehaviour<Seek>();
        }

        protected override Vector2 DoForceCalculation() {
            
            wanderTarget += new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * wanderJitter;
            wanderTarget.Normalize();
            
            Vector2 facing = Rigidbody.velocity;
            if (facing.sqrMagnitude < 0.01f) {
                facing = new Vector2(
                    Random.Range(-1.0f, 1.0f),
                    Random.Range(-1.0f, 1.0f));
            }
            facing.Normalize();

            target = (Vector2)transform.position + wanderTarget * wanderRadius + facing * wanderDistance;
            
            return seek.CalculateForce(target);
        }
    }
}