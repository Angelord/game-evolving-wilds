using UnityEngine;

namespace Claw.AI.Steering {
	public class OffsetPursuit : SteeringBehaviour {

		[SerializeField] private Rigidbody2D leader;
		[SerializeField] private Vector2 offsetPos; //In the leader's coordinate space
		private Arrive arrive;
		
		protected override void OnInitialize() {
			arrive = RequireBehaviour<Arrive>();
		}

		protected override Vector2 DoForceCalculation() {

			if (leader == null) {
				return Vector2.zero;
			}

			Vector2 worldOffsetPos = leader.transform.localToWorldMatrix.MultiplyPoint(offsetPos);
			Vector2 toOffset = worldOffsetPos - (Vector2)transform.position;
			
			float lookAheadTime = toOffset.magnitude / (Controller.Speed - leader.velocity.magnitude);

			return arrive.CalculateForce(worldOffsetPos + leader.velocity * lookAheadTime);
		}
	}
}
