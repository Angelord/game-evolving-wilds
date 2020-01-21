using UnityEngine;

namespace EvolvingWilds {
    [CreateAssetMenu(fileName = "Size Mutation", menuName = "Mutations/Size Mutation", order = 1)]
    public class SizeMutation : Mutation {

        public float GrowAmount;
        
        public override void Apply(Creature creature) {
            Vector3 localScale = creature.transform.localScale;
            localScale.x += GrowAmount;
            localScale.y += GrowAmount;
            creature.transform.localScale = localScale;
        }

        public override void Unapply(Creature creature) {
            Vector3 localScale = creature.transform.localScale;
            localScale.x -= GrowAmount;
            localScale.y -= GrowAmount;
            creature.transform.localScale = localScale;
        }
    }
}