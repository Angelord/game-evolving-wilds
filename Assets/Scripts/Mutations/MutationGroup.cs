using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EvolvingWilds {
    public class MutationGroup : ScriptableObject {
        public string Name;
        public Mutation[] Mutations;
        public bool Required;
    }
}