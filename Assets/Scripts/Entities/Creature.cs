using System.Collections.Generic;
using UnityEngine;
using Claw.AI.Steering;

namespace EvolvingWilds {
    [RequireComponent(typeof(SteeringController))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Creature : WildsEntity {

        private const float CALORIES_DECREASE_RATE = 0.008f;

        public GameObject meatPrefab;
        
        private Species _species;
        private SteeringController _steering;
        private Senses _senses;
        private CircleCollider2D _collider;
        private CreatureBuilder _builder;
        
        private List<CreatureState> _decisions = new List<CreatureState>();
        private CreatureState _currentState = null;
        
        private float _health;
        public float Calories;

        public SteeringController Steering { get { return _steering; } }

        public float CorpseCalories {
            get { return _species.CalorieConsumption / 2 + Calories; }
        }

        public float Health { get { return _health; } }

        public Species Species { get { return _species; } }

        public CreatureBuilder Builder { get { return _builder; } }

        public float DamagePerSecond {
            get { return _species.GetStat(StatType.Damage) / _species.GetStat(StatType.AttackSpeed); }
        }

        public void Initialize(Species species) {

            _builder = GetComponent<CreatureBuilder>();
            
            _species = species;
            _species.OnMutationAdded += OnMutationAdded;
            _species.OnMutationRemoved += OnMutationRemoved;

            _senses = GetComponentInChildren<Senses>();
            _senses.OnObjectEnterSight += OnObjectEnterSight;
            _senses.OnObjectLeaveSight += OnObjectLeaveSight;
            
            _steering = GetComponent<SteeringController>();
            _steering.AddBehaviour<Wander>();
            _steering.AddBehaviour<Arrive>();
            _steering.AddBehaviour<Pursuit>();
            _steering.AddBehaviour<Flee>();
            _steering.DisableAll();
            
            _health = species.GetStat(StatType.Health);
            Calories = species.CalorieConsumption / 2.0f;

            for (int i = 0; i < _species.MutationCount; i++) {
                OnMutationAdded(_species.GetMutation(i));
            }
            
            UpdateStats();

            _decisions.Add(new State_Wander(this));
            RethinkState();

            name += " -" + species.Name;
        }

        public bool IsAttacking(Creature creature) {
            State_Attack attackState = _currentState as State_Attack;
            return attackState != null && attackState.Target == creature;
        }

        public void GainCalories(float amount) {
            Calories += amount;
        }

        public void TakeDamage(float amount) {
            _health -= amount;
            if (_health <= 0.0f) {
                Die();
            }
            else {
                RethinkState();
            }
        }

        private void OnMutationAdded(Mutation mutation) {
            UpdateStats();
            mutation.Apply(this);
        }

        private void OnMutationRemoved(Mutation mutation) {
            UpdateStats();
            mutation.Unapply(this);
        }

        private void UpdateStats() {
            _health = Mathf.Clamp(_health, 0.0f, _species.GetStat(StatType.Health));
            _steering.Speed = _species.GetStat(StatType.MovementSpeed);
            _senses.Range = _species.GetStat(StatType.Sight);
        }

        private void OnObjectEnterSight(WildsEntity entity) {
            GenerateDecisions(entity);
            RethinkState();
        }

        private void OnObjectLeaveSight(WildsEntity entity) {
            RemoveDecisions(entity);
            RethinkState();
        }

        private void RethinkState() {
            
            foreach (var decision in _decisions) {
                decision.CalculateUtility();
            }

            // Determine the best state
            CreatureState bestState = _decisions[0];
            for (int i = 1; i < _decisions.Count; i++) {
                CreatureState creatureState = _decisions[i];
                if (creatureState.Utility > bestState.Utility) {
                    bestState = creatureState;
                }
            }

            if (_currentState == bestState) {
                return;
            }
            
            if (_currentState != null) {
                _currentState.Exit();
            }

            _currentState = bestState;
            
            _currentState.Enter();
        }
        
        private void GenerateDecisions(WildsEntity entity) {
            if (entity is Creature) {
                Creature creature = entity as Creature;

                // TODO Add mating
                
                if (creature.Species == Species) {
                    return;
                }
                
                _decisions.Add(new State_Attack(this, creature));
                _decisions.Add(new State_Run(this, creature));
            }
            else if (entity is Food) {
                
                Food food = entity as Food;
                
                _decisions.Add(new State_Eat(this, food));
            }
        }

        private void RemoveDecisions(WildsEntity entity) {
            for (int i = _decisions.Count - 1; i >= 1; i--) {
                if (_decisions[i].Target == entity) {
                    _decisions.RemoveAt(i);
                }
            }
        }

        private void Update() {

            Calories -= Species.CalorieConsumption * CALORIES_DECREASE_RATE * Time.deltaTime;
            if (Calories <= 0.0f) {
                Die();        
            }

            _currentState.Update();

            if (_currentState.Done) {
                RethinkState();
            }

            if (Calories >= Species.CalorieConsumption * 2) {
                Debug.Log("Reproducing");
                Calories /= 2.0f;
                Reproduce();
            }
        }

        private void Reproduce() {

            Vector2 childOffset = new Vector2(
                Random.Range(-1.0f, 1.0f),
                Random.Range(-1.0f, 1.0f)
                ) * Species.GetStat(StatType.Range);
            
            Creature child = Instantiate(gameObject, transform.position + (Vector3)childOffset, Quaternion.identity).GetComponent<Creature>();
            child.Builder.Clear();
            child.Initialize(Species);
        }

        private void Die() {
            Food meat = Instantiate(meatPrefab, transform.position, Quaternion.identity).GetComponent<Food>();

            meat.Calories = CorpseCalories; 
            
            Destroy(this.gameObject);

            Simulation.DeathCount++;
        }

        // On Entity In Sight -> Rethink state
    }
}