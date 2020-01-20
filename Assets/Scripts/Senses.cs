using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EvolvingWilds {
    [RequireComponent(typeof(CircleCollider2D))]
    public class Senses : MonoBehaviour {

        private List<Food> _visibleFood = new List<Food>();
        public List<Creature> _visibleCreatures = new List<Creature>();
        
        public float Range { set { GetComponent<CircleCollider2D>().radius = value; } }

        public IEnumerable<Food> GetVisibleFood() {
            return _visibleFood;
        }

        public IEnumerable<Creature> GetVisibleCreatures() {
            return _visibleCreatures;
        }

        private void Start() {
            Simulation.Instance.OnCreatureDestroyed += OnCreatureDestroyed;
            Simulation.Instance.OnFoodDestroyed += OnFoodDestoryed;
        }
        
        private void OnFoodDestoryed(Food food) {
            if (_visibleFood.Contains(food)) {
                _visibleFood.Remove(food);
            }
        }

        private void OnCreatureDestroyed(Creature creature) {
            if (_visibleCreatures.Contains(creature)) {
                _visibleCreatures.Remove(creature);
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            WildsEntity entity = other.gameObject.GetComponent<WildsEntity>();
            if(entity == null) return;
            if (entity.gameObject == transform.parent.gameObject) return;

            if (entity is Creature) {
                _visibleCreatures.Add(entity as Creature);
            }
            else if (entity is Food) {
                _visibleFood.Add(entity as Food);
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            WildsEntity entity = other.gameObject.GetComponent<WildsEntity>();
            if(entity == null) return;
            if (entity.gameObject == transform.parent.gameObject) return;

            if (entity is Creature) {
                _visibleCreatures.Remove(entity as Creature);
            }
            else if (entity is Food) {
                _visibleFood.Remove(entity as Food);
            }
        }
    }
}