
using UnityEngine;

namespace Claw.AI.Steering {
    public class Seek : SteeringBehaviour {

        [SerializeField] private Transform target;

        public Transform Target { get { return target; } set { target = value; } }

        public Vector2 CalculateForce(Vector2 targetPos) {
            
            Vector2 desiredVel = (targetPos - (Vector2)transform.position).normalized * Controller.Speed;
            
            return (desiredVel - Rigidbody.velocity);
        }

        protected override Vector2 DoForceCalculation() {

            if (target == null) {
                return Vector2.zero;
            }

            return CalculateForce(target.position);
        }
    }
}