using Claw.AI.Steering;
using UnityEngine;

namespace EvolvingWilds {
    public class State_Eat : CreatureState {

        private Food _food;
        private Seek _seek;

        public State_Eat(Creature creature, Food food) : base(creature) {
            _food = food;
            _seek = Creature.Steering.GetBehaviour<Seek>();
        }
        
        protected override float DoUtilityCalculation() {
            if (!Creature.Species.CanEat(_food.FoodType)) {
                return 0.0f;
            }

            // TODO : Take foraging into account 

            float distance = Vector2.Distance(_food.transform.position, Creature.transform.position);
            float consumption = Creature.Species.CalorieConsumption;
            return 0.5f * (1.0f - Creature.Calories / consumption) 
                   + 0.3f * ((_food.Calories * 2.0f) / consumption)
                   + 0.2f * (1.0f - distance / Species.GetStat(StatType.Sight));
        }

        protected override void OnEnter() {
            _seek.enabled = true;
            _seek.Target = _food.transform;
            _seek.MoveToFront();
        }

        protected override void OnExit() {
            _seek.enabled = false;
        }

        public override void Update() {
            if (Vector2.Distance(Creature.transform.position, _food.transform.position) > Species.GetStat(StatType.Range)) {
                return;
            }
            _seek.enabled = false;

            if (_food == null) {
                Done = true;
            }
            
            float eatAmount = _food.LoseCalories(Species.GetStat(StatType.Foraging) * Time.deltaTime);

            Creature.GainCalories(eatAmount);
        }
    }
}