using UnityEngine;

namespace EvolvingWilds {
    [RequireComponent(typeof(World))]
    public class Simulation : MonoBehaviour {

        public Mutation[] StartingMutations;
        public int NumStartingSpecies;
        public GameObject CreaturePrefab;

        private World _world;
        
        private void Start() {
            _world = GetComponent<World>();
            
            PlaceStarting();
        }

        private void PlaceStarting() {
            for (int i = 0; i < NumStartingSpecies; i++) {
                
                // TODO : Generate names randomly
                Species newSpecies = new Species("Species " + i);
                
                // TODO : Calculate count based on calorie consumption
                
                foreach (var mutation in StartingMutations) {
                    newSpecies.AddMutation(mutation);
                }
                
                PlaceCreatures(5, newSpecies);   
                
                // TODO : Add random mutation
            }
        }

        private void PlaceCreatures(int count, Species species) {
            for (int i = 0; i < count; i++) {
                Creature creature = Instantiate(CreaturePrefab, _world.RandomPosition(), Quaternion.identity).GetComponent<Creature>();
                creature.Initialize(species);
            }
        }
    }
}
