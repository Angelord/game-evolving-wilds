using Claw.AI.Steering;
using UnityEngine;

namespace EvolvingWilds {
    public class State_Run : CreatureState {

        private PredatorAvoidance _predatorAvoidance;
        
        public State_Run(Creature creature) : base(creature) { }
        
        protected override float DoUtilityCalculation() {

            float greatestThreat = 0.0f;

            float healthPercent = Creature.Health / Species.GetStat(StatType.Health);
            
            foreach (var threat in Senses.GetVisibleCreatures()) {
                if(threat.Species == Species || !threat.Species.CanEat(FoodType.Meat)) continue;
                
                float distance = Vector2.Distance(threat.transform.position, Creature.transform.position);

                float value = 1.0f * (threat.DamagePerSecond / Creature.DamagePerSecond)
                              + 0.5f * healthPercent
                              + 0.5f * (1.0f - distance / (threat.Species.GetStat(StatType.Sight) * 3.0f))
                              - (Creature.Species.EaterType == EaterType.Herbivore ? 0.0f : 0.4f);

                greatestThreat = Mathf.Max(value, greatestThreat);
            }

            return greatestThreat;
        }

        protected override void OnEnter() {
            _predatorAvoidance = Steering.GetBehaviour<PredatorAvoidance>();
            _predatorAvoidance.enabled = true;
        }

        protected override void OnExit() {
            _predatorAvoidance.enabled = false;
        }

        public override void Update() {
        }
    }
}