using UnityEngine;

namespace Claw.AI.Steering {
	public class Flee : SteeringBehaviour {

		[SerializeField] private Transform target;
		[SerializeField] private float panicDistance = 5.0f;
		private float panicDistanceSq;
		
		public float PanicDistance {
			get { return panicDistance; }
			set {
				panicDistance = value;
				panicDistanceSq = panicDistance * panicDistance;
			}
		}

		public Vector2 CalculateForce(Vector2 targetPos) {
			
			if (Vector2.SqrMagnitude((Vector2)transform.position - targetPos) > panicDistanceSq) {
				return Vector2.zero;
			}

			Vector2 desiredVel = ((Vector2)transform.position - targetPos).normalized * Controller.Speed;

			return (desiredVel - Rigidbody.velocity);
		}

		protected override void OnInitialize() {
			PanicDistance = panicDistance;	
		}

		protected override Vector2 DoForceCalculation() {
			
			if (target == null) {
				return Vector2.zero;
			}

			return CalculateForce(target.position);
		}
	}
}
