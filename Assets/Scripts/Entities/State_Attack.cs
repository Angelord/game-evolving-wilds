using Claw.AI.Steering;
using UnityEngine;

namespace EvolvingWilds {
    public class State_Attack : CreatureState {

        private Creature _target;
        private Pursuit _pursuit;
        private float _lastAttack;

        public override WildsEntity Target { get { return _target; } }
        
        public State_Attack(Creature creature, Creature target) : base(creature) {
            this._target = target;
        }


        protected override float DoUtilityCalculation() {
            if (!Species.CanEat(FoodType.Meat)) {
                return 0.0f;
            }

            float distance = GetDistanceToTarget();
            float sight = Species.GetStat(StatType.Sight);
            float consumption = Creature.Species.CalorieConsumption;

            if (distance > sight * 2) {
                return 0.0f;
            }
            
            float value = 0.8f * (1.0f - Creature.Calories / (consumption * 3.0f))
                            + 0.15f * (_target.CorpseCalories / consumption)
                            + 0.05f * (1.0f - distance / Species.GetStat(StatType.Sight))
                            - 0.2f * (_target.DamagePerSecond / Creature.DamagePerSecond)
                            - 0.1f * (1.0f - Creature.Health / Species.GetStat(StatType.Health))
                            + 0.1f * (1.0f - _target.Health / _target.Species.GetStat(StatType.Health))
                            + (_target.IsAttacking(Creature) ? 0.15f : 0.0f);

            if (IsCurrent && !Done) {
                value += 0.2f;
            }

            return value;
        }

        protected override void OnEnter() {
            _pursuit = Steering.GetComponent<Pursuit>();
            _pursuit.enabled = true;
            _pursuit.Target = _target.GetComponent<Rigidbody2D>();
        }

        protected override void OnExit() {
            _pursuit.enabled = false;
        }

        public override void Update() {

            if (_target == null) {
                Done = true;
                return;
            }

            float distance = GetDistanceToTarget();

            if (distance > Species.GetStat(StatType.Sight)) {    // Ran away
                Done = true;
                return;
            }

            if (GetDistanceToTarget() <= Species.GetStat(StatType.Range)
                && Time.time - _lastAttack > Species.GetStat(StatType.AttackSpeed)) {
                _lastAttack = Time.time;
                _target.TakeDamage(Species.GetStat(StatType.Damage));
            }
        }

        private float GetDistanceToTarget() {
            return Vector2.Distance(_target.transform.position, Creature.transform.position);
        }
    }
}