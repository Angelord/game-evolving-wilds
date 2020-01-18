using UnityEngine;
using Claw.AI.Steering;

namespace EvolvingWilds {
    
    public class State_Wander : CreatureState {

        public State_Wander(Creature creature) : base(creature) { }

        public override WildsEntity Target { get { return null; } }

        protected override float DoUtilityCalculation() {
            return 0.2f;
        }

        protected override void OnEnter() {
            Steering.GetBehaviour<Wander>().enabled = true;
        }

        protected override void OnExit() {
            Steering.GetBehaviour<Wander>().enabled = false;
        }
    }
}