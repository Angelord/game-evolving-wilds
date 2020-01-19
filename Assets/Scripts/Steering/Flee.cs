using UnityEngine;

namespace Claw.AI.Steering {
	public class Flee : SteeringBehaviour {

		private Transform target;

		public Transform Target { get { return target; } set { target = value; } }

		public Vector2 CalculateForce(Vector2 targetPos) {
			
			Vector2 desiredVel = ((Vector2)transform.position - targetPos).normalized * Controller.Speed;

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
