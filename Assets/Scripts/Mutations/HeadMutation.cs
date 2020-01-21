using UnityEngine;

namespace EvolvingWilds {
    [CreateAssetMenu(fileName = "Head Mutation", menuName = "Mutations/Head Mutation", order = 1)]
    public class HeadMutation : BodypartMutation {
        
        public EaterType Type;

        public override void BeforeAddToSpecies(Species species) {
            base.BeforeAddToSpecies(species);
            species.EaterType = Type;
        }
    }
}