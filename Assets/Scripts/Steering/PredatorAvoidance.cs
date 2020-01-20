using System;
using EvolvingWilds;
using UnityEngine;

namespace Claw.AI.Steering {
    public class PredatorAvoidance : SteeringBehaviour {

        private Vector2 steeringForce;
        
        protected override Vector2 DoForceCalculation() {
            
            steeringForce = Vector2.zero;
            int count = 0;
            foreach (var predator in Senses.GetVisibleCreatures()) {
                if(!predator.Species.CanEat(FoodType.Meat)) continue;

                count++;
                Vector2 fromPredator = transform.position - predator.transform.position;
                steeringForce += fromPredator.normalized * Controller.Speed;
            }

            if (count != 0) {
                steeringForce = steeringForce - Rigidbody.velocity;
                steeringForce = steeringForce.normalized * Controller.Speed;
            }

            return steeringForce;
        }

        private void OnDrawGizmosSelected() {
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)steeringForce);
        }
    }
}