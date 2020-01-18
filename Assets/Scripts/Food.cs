using UnityEngine;

namespace EvolvingWilds {

    public enum FoodType {
        Meat,
        Vegetable
    }

    public class Food : WildsEntity {
        public FoodType FoodType;
        public float Calories;
        
        public float LoseCalories(float amount) {
            Calories -= amount;
            // TODO : Shake
            
            if (Calories <= 0.0f) {
                Destroy(this.gameObject);
                return amount + Calories;
            }

            return amount;
        }
    }
}