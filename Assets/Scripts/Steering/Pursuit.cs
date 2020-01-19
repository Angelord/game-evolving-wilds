using UnityEngine;

namespace Claw.AI.Steering {

	public class Pursuit : SteeringBehaviour {

		private Rigidbody2D target;
		private Seek seek;

		public Rigidbody2D Target { get { return target; } set { target = value; } }

		protected override void OnInitialize() {
			seek = RequireBehaviour<Seek>();
		}

		protected override Vector2 DoForceCalculation() {

			if (target == null) {
				return Vector2.zero;
			}

			Vector2 toTarget = target.transform.position - transform.position;

			float relativeHeading = Vector2.Dot(transform.up, target.transform.up);

			if (Vector2.Dot(toTarget, transform.up) > 0 &&
			    relativeHeading < -0.95) {
				
				return seek.CalculateForce(target.position);
			}

			float lookAheadTime = toTarget.magnitude / (Controller.Speed + target.velocity.magnitude); 

			return seek.CalculateForce(target.position + target.velocity * lookAheadTime);
		}
	}
}
