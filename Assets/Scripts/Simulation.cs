using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EvolvingWilds {
    [RequireComponent(typeof(World))]
    public class Simulation : MonoBehaviour {

        public Mutation[] StartingMutations;
        public Mutation[] AllMutations;
        public int NumStartingSpecies;
        public List<Species> Species = new List<Species>();
        public GameObject CreaturePrefab;

        private World _world;
        
        private void Start() {
            _world = GetComponent<World>();
            
            PlaceStarting();
        }

        private void Update() {
            foreach (var species in Species) {
                species.UpdateResearch();
            }
        }

        private void PlaceStarting() {
            for (int i = 0; i < NumStartingSpecies; i++) {
                
                // TODO : Generate names randomly
                Species newSpecies = new Species("Species " + i);
                
                foreach (var mutation in StartingMutations) {
                    newSpecies.AddMutation(mutation);
                }
                
                Species.Add(newSpecies);

                newSpecies.OnResearchComplete += OnResearchComplete;
                
                BeginRandomResearch(newSpecies);

                // TODO : Calculate count based on calorie consumption

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

        private void OnResearchComplete(Species species, Mutation research) {
            BeginRandomResearch(species);
        }

        private void BeginRandomResearch(Species species) {
            
            List<Mutation> missingMutations = AllMutations.Where(mutation => !species.HasMutation(mutation)).ToList();

            Mutation newMutation = missingMutations[Random.Range(0, missingMutations.Count)];
            
            species.BeginResearch(newMutation);
        }
    }
}
