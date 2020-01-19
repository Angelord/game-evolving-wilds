using System.Linq;
using UnityEngine;

namespace EvolvingWilds {
    public abstract class Mutation : ScriptableObject {
        public string Name;
        public Sprite Icon;
        public float ResearchTime;
        public float CalorieConsumption;
        public Mutation[] Prerequisites;
        public Mutation[] Incompatibles;
		
        public StatModifier[] Modifiers;
        
        public void ModifyStat(StatType stat, ref float value) {
            foreach (StatModifier modifier in Modifiers) {
                if (modifier.Stat == stat) {
                    value += modifier.Value;
                }
            }
        }

        public virtual void OnAddToSpecies(Species species) {
            // Remove any non-compatible mutations
            foreach (Mutation mutation in Incompatibles) {
                if(!species.HasMutation(mutation)) continue;
                species.RemoveMutation(mutation);
            }	
        }

        public virtual void OnRemoveFromSpecies(Species species) {
            // Remove any mutations that are prerequisites for us
            for (int i = species.MutationCount - 1; i >= 0; i--) {
                Mutation mutation = species.GetMutation(i);
                if (mutation.Prerequisites.Contains(this)) {
                    species.RemoveMutation(mutation);
                }
            }
        }

        public virtual void Apply(Creature creature) {
        }

        public virtual void Unapply(Creature creature) {
        }
    }
    
    [System.Serializable]
    public class StatModifier {
        public StatType Stat;
        public float Value;
    }
}