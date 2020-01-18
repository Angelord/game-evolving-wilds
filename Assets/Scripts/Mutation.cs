using System.Linq;
using UnityEngine;

namespace EvolvingWilds {
    [CreateAssetMenu(fileName = "Mutation", menuName = "Custom/Mutation", order = 1)]
    public class Mutation : ScriptableObject {
        public string Name;
        public Sprite Icon;
        public float ResearchTime;
        public float CalorieConsumption;
        public Mutation[] Prerequisites;
        public Mutation[] Incompatibles;
		
        public StatModifier[] Modifiers;
		
        // TODO : Body Part
		
        // TODO : Is Compatible (Species)

        public void ModifyStat(StatType stat, ref float value) {
            foreach (StatModifier modifier in Modifiers) {
                if (modifier.Stat == stat) {
                    value += modifier.Value;
                }
            }
        }

        public void OnAdd(Species species) {
            // Remove any non-compatible mutations
            foreach (Mutation mutation in Incompatibles) {
                if(!species.HasMutation(mutation)) continue;
                species.RemoveMutation(mutation);
            }	
        }

        public void OnRemove(Species species) {
            // Remove any mutations that are prerequisites for us
            for (int i = species.MutationCount - 1; i >= 0; i--) {
                Mutation mutation = species.GetMutation(i);
                if (mutation.Prerequisites.Contains(this)) {
                    species.RemoveMutation(mutation);
                }
            }
        }
    }
    
    [System.Serializable]
    public class StatModifier {
        public StatType Stat;
        public float Value;
    }
}