using UnityEngine;

namespace EvolvingWilds {
    [CreateAssetMenu(fileName = "Eater Type Mutation", menuName = "Mutations/Eater Type Mutation", order = 1)]
    public class EaterTypeMutation : Mutation {
        
        public EaterType Type;

        public override void OnAddToSpecies(Species species) {
            base.OnAddToSpecies(species);
            species.EaterType = Type;
        }
    }
}