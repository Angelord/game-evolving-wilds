using Claw.AI.Steering;
using UnityEngine;

namespace EvolvingWilds {
    public class State_Run : CreatureState {

        private Creature _target;
        private Flee _flee;
        
        public override WildsEntity Target { get { return _target; } }

        public State_Run(Creature creature, Creature target) : base(creature) {
            _target = target;
        }
        
        protected override float DoUtilityCalculation() {

            if (!_target.Species.CanEat(FoodType.Meat)) {
                return 0.0f;    // No point running from herbivores
            }
            
            float distance = Vector2.Distance(_target.transform.position, Creature.transform.position);

            float value = 1.0f * (_target.DamagePerSecond / Creature.DamagePerSecond)
                        + 0.6f * (Creature.Health / Species.GetStat(StatType.Health))
                        + 0.6f * (1.0f - distance / (_target.Species.GetStat(StatType.Sight) * 3.0f));

            if (IsCurrent && !Done) {
                return value + 0.2f;
            }
            
            return value;
        }

        protected override void OnEnter() {
            _flee = Steering.GetBehaviour<Flee>();
            _flee.Target = _target.transform;
            _flee.enabled = true;
        }

        protected override void OnExit() {
            _flee.enabled = false;
        }

        public override void Update() {

            float distance = Vector2.Distance(_target.transform.position, Creature.transform.position);

            if (distance > _target.Species.GetStat(StatType.Sight) * 3.0f) {
                Done = true;
            }
        }
    }
}