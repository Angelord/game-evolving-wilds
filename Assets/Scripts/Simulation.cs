using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EvolvingWilds {

    [RequireComponent(typeof(World))]
    public class Simulation : MonoBehaviour {

        private const int MAX_SPEED = 4;

        public static int DeathCount = 0;
        
        public Mutation[] HerbivoreStartingMutations;
        public Mutation[] CarnivoreStartingMutations;
        public Mutation[] AllMutations;
        public int BiomassPerStartingSpecies;
        public int NumStartingHerbivores;
        public int NumStartingCarnivores;
        public List<Species> Species = new List<Species>();
        public GameObject CreaturePrefab;

        private World _world;
        private bool _paused;
        private int _speed;

        public bool Paused {
            get {
                return _paused;
            }
            set {
                _paused = value;
                Time.timeScale = _paused ? 0.0f : 1.0f;
            }
        }

        public void IncreaseSpeed() {
            _speed = Mathf.Clamp(_speed + 1, 0, MAX_SPEED);
            Time.timeScale = Mathf.Pow(2.0f, _speed);
        }

        public void DecreaseSpeed() {
            _speed = Mathf.Clamp(_speed - 1, 0, MAX_SPEED);
            Time.timeScale = Mathf.Pow(2.0f, _speed);
        }

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
            
            for (int i = 0; i < NumStartingHerbivores; i++) {
                PlaceInitialSpecies(HerbivoreStartingMutations);   
            }
            
            for (int i = 0; i < NumStartingCarnivores; i++) {
                PlaceInitialSpecies(CarnivoreStartingMutations);   
            }
        }
        
        private void PlaceInitialSpecies(Mutation[] startingMutations) {
            // TODO : Generate names randomly
            Species newSpecies = new Species("Species ");
                
            Species.Add(newSpecies);

            foreach (var mutation in startingMutations) {
                newSpecies.AddMutation(mutation);
            }
            
            newSpecies.OnResearchComplete += OnResearchComplete;
            
            BeginRandomResearch(newSpecies);

            float spawnCount = Mathf.RoundToInt(BiomassPerStartingSpecies / newSpecies.CalorieConsumption);
            
            for (int i = 0; i < spawnCount; i++) {
                Creature creature = Instantiate(CreaturePrefab, _world.RandomPosition(), Quaternion.identity).GetComponent<Creature>();
                creature.Initialize(newSpecies);
            }
        }

        private void OnResearchComplete(Species species, Mutation research) {
            BeginRandomResearch(species);
        }

        private void BeginRandomResearch(Species species) {
            
            List<Mutation> missingMutations = AllMutations.Where(mutation => !species.HasMutation(mutation)).ToList();

            if(missingMutations.Count == 0) return;

            Mutation newMutation = missingMutations[Random.Range(0, missingMutations.Count)];
            
            species.BeginResearch(newMutation);
        }

        private void OnGUI() {
            GUI.Label(new Rect(20, 20, 300, 200), "Death Count " + DeathCount);
        }
    }
}
