using Claw.AI.Steering;
using UnityEngine;

namespace EvolvingWilds {
    public class State_Eat : CreatureState {

        private Food _food;
        private Arrive _arrive;
        
        public State_Eat(Creature creature, Food food) : base(creature) {
            _food = food;
        }

        protected override float DoUtilityCalculation() {

            if (!Creature.Species.CanEat(_food.FoodType)) {
                return 0.0f;
            }

            // TODO : Take foraging into account 

            float distance = Vector2.Distance(_food.transform.position, Creature.transform.position);
            float consumption = Creature.Species.CalorieConsumption;
            float value = 0.85f * (1.0f - Creature.Calories / (consumption * 3.0f))
                   + 0.1f * (_food.Calories / consumption)
                   + 0.05f * (1.0f - distance / Species.GetStat(StatType.Sight));
            
            if (IsCurrent && !Done) {
                value += 0.1f;
            }

            return value;
        }

        protected override void OnEnter() {
            _arrive = Steering.GetBehaviour<Arrive>();
            _arrive.enabled = true;
            _arrive.Target = _food.transform;
//            _arrive.MoveToFront();
        }

        protected override void OnExit() {
            _arrive.enabled = false;
        }

        public override void Update() {
            if (_food == null) {
                Done = true;
                return;
            }

            float distanceToFood = Vector2.Distance(Creature.transform.position, _food.transform.position);
            if (distanceToFood > Species.GetStat(StatType.Range)) {
                _arrive.enabled = true;
                return;
            }
            _arrive.enabled = false;

            float eatAmount = _food.LoseCalories(Species.GetStat(StatType.Foraging) * Time.deltaTime);
            
            Creature.GainCalories(eatAmount);
        }
    }
}