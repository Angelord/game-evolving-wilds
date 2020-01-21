using System.Linq;
using UnityEngine;

namespace EvolvingWilds {
    public abstract class Mutation : ScriptableObject {
        public string Name;
        public Sprite Icon;
        public float ResearchTime;
        public float CalorieConsumption;
        public Mutation[] Prerequisites;
        public MutationCategory Category;
        
        public StatModifier[] Modifiers;
        
        public void ModifyStat(StatType stat, ref float value) {
            foreach (StatModifier modifier in Modifiers) {
                if (modifier.Stat == stat) {
                    value += modifier.Value;
                }
            }
        }

        public bool CanObtain(Species species) {
            if (species.HasMutation(this)) return false;

            foreach (var prerequisite in Prerequisites) {
                if (!species.HasMutation(prerequisite)) return false;
            }

            return true;
        }

        public virtual void BeforeAddToSpecies(Species species) {

            if (Category == null) return;

            for (int i = species.MutationCount - 1; i >= 0; i--) {
                Mutation mutation = species.GetMutation(i);
                if (mutation.Category == Category) {
                    species.RemoveMutation(mutation);
                }
            }
        }

        public virtual void BeforeRemoveFromSpecies(Species species) {
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