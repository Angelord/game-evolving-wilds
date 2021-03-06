using UnityEngine;

namespace Claw.AI.Steering {
    public class Evade : SteeringBehaviour {

        [SerializeField] private Rigidbody2D pursuer;
        private Flee flee;

        public Rigidbody2D Pursuer { get { return pursuer; } set { pursuer = value; } }

        protected override void OnInitialize() {
            flee = RequireBehaviour<Flee>();
        }
        
        protected override Vector2 DoForceCalculation() {

            if (pursuer == null) {
                return Vector2.zero;
            }

            Vector2 toPursuer = pursuer.transform.position - transform.position;

            float lookAheadTime = toPursuer.magnitude / 
                                  (Controller.Speed + pursuer.velocity.magnitude);

            return flee.CalculateForce(pursuer.position + pursuer.velocity * lookAheadTime);
        }
    }
}