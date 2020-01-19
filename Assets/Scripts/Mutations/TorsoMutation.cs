using UnityEngine;

namespace EvolvingWilds {
    [CreateAssetMenu(fileName = "Torso Mutation", menuName = "Mutations/Torso Mutation", order = 1)]
    public class TorsoMutation : Mutation {
        
        public GameObject Torso;

        public override void Apply(Creature creature) {
            creature.Builder.SetTorso(Torso);
        }
    }
}