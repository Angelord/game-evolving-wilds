using System;
using System.Collections.Generic;
using UnityEngine;
using Claw.AI.Steering;
using Random = UnityEngine.Random;

namespace EvolvingWilds {
    [RequireComponent(typeof(SteeringController))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Creature : WildsEntity {

        private const float CALORIES_DECREASE_RATE = 0.008f;
        private const float STATE_RETHINK_INTERVAL = 0.5f;
        private const float STATE_RETHINK_DEVIATION = 0.1f;

        public GameObject meatPrefab;
        
        private Species _species;
        private SteeringController _steering;
        private Senses _senseses;
        private CircleCollider2D _collider;
        private CreatureBuilder _builder;
        private float _nextStateRethink;
        
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

        public Senses Senses { get { return _senseses; } }

        public float DamagePerSecond {
            get { return _species.GetStat(StatType.Damage) / _species.GetStat(StatType.AttackSpeed); }
        }

        public void Initialize(Species species) {

            _builder = GetComponent<CreatureBuilder>();
            
            _species = species;
            _species.OnMutationAdded += OnMutationAdded;
            _species.OnMutationRemoved += OnMutationRemoved;

            _senseses = GetComponentInChildren<Senses>();
            
            _steering = GetComponent<SteeringController>();
            _steering.AddBehaviour<PredatorAvoidance>();
            _steering.AddBehaviour<Pursuit>();
            _steering.AddBehaviour<Arrive>();
            _steering.AddBehaviour<Wander>();
            _steering.DisableAll();
            
            _health = species.GetStat(StatType.Health);
            Calories = species.CalorieConsumption / 2.0f;

            for (int i = 0; i < _species.MutationCount; i++) {
                OnMutationAdded(_species.GetMutation(i));
            }
            
            UpdateStats();

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
            _senseses.Range = _species.GetStat(StatType.Sight);
        }

        private void RethinkState() {

            _decisions.Clear();
            
            _decisions.Add(new State_Wander(this));
            _decisions.Add(new State_Run(this));
            
            foreach (var creature in _senseses.GetVisibleCreatures()) {
                if(creature.Species == _species) continue;
                _decisions.Add(new State_Attack(this, creature));
            }

            foreach (var food in _senseses.GetVisibleFood()) {
                _decisions.Add(new State_Eat(this, food));
            }

            if (_currentState != null && !_currentState.Done) {
                _decisions.Add(_currentState);
            }

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

            _nextStateRethink = Time.time + STATE_RETHINK_INTERVAL + Random.Range(0.0f, STATE_RETHINK_INTERVAL);
        }

        private void Update() {

            Calories -= Species.CalorieConsumption * CALORIES_DECREASE_RATE * Time.deltaTime;
            if (Calories <= 0.0f) {
                Die();        
            }

            _currentState.Update();

            if (_currentState.Done || Time.time >= _nextStateRethink) {
                RethinkState();
            }

            if (Calories >= Species.CalorieConsumption * 2) {
                Debug.Log("Reproducing");
                Calories /= 2.0f;
                Simulation.Instance.Reproduce(transform.position, _species);
            }
        }

        private void Die() {
            Food meat = Instantiate(meatPrefab, transform.position, Quaternion.identity).GetComponent<Food>();

            meat.Calories = CorpseCalories; 
            
            Destroy(this.gameObject);

            Simulation.Instance.CreatureDied(this);
        }

        // On Entity In Sight -> Rethink state
    }
}