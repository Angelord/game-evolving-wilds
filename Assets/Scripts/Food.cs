using System;
using System.Collections;
using UnityEngine;

namespace EvolvingWilds {

    public enum FoodType {
        Meat,
        Vegetable
    }

    public class Food : WildsEntity {
        public FoodType FoodType;
        public float Calories;
        private Shaker _shaker;
        private float _lastShake;
        
        private void Start() {
            _shaker = GetComponent<Shaker>();
        }

        public float LoseCalories(float amount) {
            Calories -= amount;
            
            if (Calories <= 0.0f) {
                Destroy(this.gameObject);
                return amount + Calories;
            }

            _lastShake = Time.time;
            _shaker.enabled = true;
            
            return amount;
        }

        private void Update() {
            if (_shaker.enabled && _lastShake + Time.deltaTime < Time.time) {
                _shaker.enabled = false;
            }
        }
    }
}