using System.Collections.Generic;
using UnityEngine;
using Claw.AI.Steering;

namespace EvolvingWilds {
    [RequireComponent(typeof(SteeringController))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Creature : WildsEntity {

        private const float CALORIES_DECREASE_RATE = 0.01f;

        public GameObject meatPrefab;
        
        private Species _species;
        private SteeringController _steering;
        private Senses _senses;
        private CircleCollider2D _collider;
        
        private List<CreatureState> _decisions = new List<CreatureState>();
        private CreatureState _currentState = null;
        
        private float _health;
        private float _calories;

        public SteeringController Steering { get { return _steering; } }

        public float Calories { get { return _calories; } }

        public Species Species { get { return _species; } }

        public void Initialize(Species species) {
            
            _species = species;
            _species.OnMutation += OnMutation;

            _senses = GetComponentInChildren<Senses>();
            _senses.OnObjectEnterSight += OnObjectEnterSight;
            _senses.OnObjectLeaveSight += OnObjectLeaveSight;
            
            _steering = GetComponent<SteeringController>();
            _steering.AddBehaviour<Wander>();
            _steering.AddBehaviour<Arrive>();
            _steering.DisableAll();
            
            _health = species.GetStat(StatType.Health);
            _calories = species.CalorieConsumption / 2.0f;
            
            UpdateStats();
            
            _decisions.Add(new State_Wander(this));

            DetermineState();

            name += " -" + species.Name;
        }

        public void GainCalories(float amount) {
            _calories += amount;
        }
        
        private void OnMutation() {
            float maxHealth = _species.GetStat(StatType.Health);
            _health = Mathf.Clamp(_health, 0.0f, maxHealth);
            UpdateStats();
        }
        
        private void UpdateStats() {
            _steering.Speed = _species.GetStat(StatType.MovementSpeed);
            _senses.Range = _species.GetStat(StatType.Sight);
        }

        private void OnObjectEnterSight(WildsEntity entity) {
            GenerateDecisions(entity);
            DetermineState();
        }

        private void OnObjectLeaveSight(WildsEntity entity) {
            RemoveDecisions(entity);
            DetermineState();
        }

        private void DetermineState() {


            if (_currentState != null && !(_currentState is State_Wander)) {
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
        }
        
        private void GenerateDecisions(WildsEntity entity) {
            if (entity is Creature) {
                Creature creature = entity as Creature;
             
                // TODO Add mate 
                
//                _decisions.Add(new State_Attack(this, creature));
//                _decisions.Add(new State_Run(this, creature));
            }
            else if (entity is Food) {
                
                Food food = entity as Food;
                
                _decisions.Add(new State_Eat(this, food));
            }
        }

        private void RemoveDecisions(WildsEntity entity) {
            for (int i = _decisions.Count - 1; i >= 0; i--) {
                if (_decisions[i].Target == entity) {
                    _decisions.RemoveAt(i);
                }
            }
        }

        private void Update() {

            _calories -= Species.CalorieConsumption * CALORIES_DECREASE_RATE * Time.deltaTime;
            if (_calories <= 0.0f) {
                Die();        
            }

            _currentState.Update();

            if (_currentState.Done) {
                DetermineState();
            }
        }
        
        private void Die() {
            Food meat = Instantiate(meatPrefab, transform.position, Quaternion.identity).GetComponent<Food>();

            meat.Calories = _species.CalorieConsumption / 2 + _calories * _species.CalorieConsumption; 
            
            Destroy(this.gameObject);
        }

        // On Entity In Sight -> Rethink state
    }
}