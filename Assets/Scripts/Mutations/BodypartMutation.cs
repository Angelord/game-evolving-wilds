using UnityEngine;

namespace EvolvingWilds {
    [CreateAssetMenu(fileName = "Bodypart Mutation", menuName = "Mutations/Bodypart Mutation", order = 1)]
    public class BodypartMutation : Mutation {

        public GameObject Bodypart;
        
        public override void Apply(Creature creature) {
            creature.Builder.SetBodypart(Bodypart);
        }

        public override void Unapply(Creature creature) {
            creature.Builder.RemoveBodypart(Bodypart.GetComponent<Bodypart>().Joint);
        }
    }
}