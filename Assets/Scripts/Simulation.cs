using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EvolvingWilds {

    [RequireComponent(typeof(World))]
    public class Simulation : MonoBehaviour {

        private const int MAX_SPEED = 4;

        public static int DeathCount = 0;

        [Range(0.0f, 1.0f)]public float NewSpeciesChance = 0.5f;
        public Mutation[] AllMutations;
        public int BiomassPerStartingSpecies;
        public int NumStartingHerbivores;
        public int NumStartingCarnivores;
        public float StartingHerbivoreLimit;
        public float StartingCarnivoreLimit;
        public List<Species> Species = new List<Species>();
        public GameObject CreaturePrefab;

        private List<Mutation> _torsos; // For quick lookup
        private static Simulation _instance;
        private World _world;
        private bool _paused;
        private int _speed;

        public event Action<Creature> OnCreatureDestroyed;
        public event Action<Food> OnFoodDestroyed;

        public static Simulation Instance { get { return _instance; } }

        public bool Paused {
            get {
                return _paused;
            }
            set {
                _paused = value;
                Time.timeScale = _paused ? 0.0f : 1.0f;
            }
        }
        private void Start() {
            _instance = this;
            _world = GetComponent<World>();
            
            PlaceStarting();
        }

        private void OnDestroy() {
            _instance = null;
        }

        private void Update() {
            foreach (var species in Species) {
                species.UpdateResearch();
            }
        }

        public void CreatureDied(Creature creature) {
            OnCreatureDestroyed?.Invoke(creature);
            DeathCount++;
        }

        public void FoodDestroyed(Food food) {
            OnFoodDestroyed?.Invoke(food);
        }

        public void IncreaseSpeed() {
            _speed = Mathf.Clamp(_speed + 1, 0, MAX_SPEED);
            Time.timeScale = Mathf.Pow(2.0f, _speed);
        }

        public void DecreaseSpeed() {
            _speed = Mathf.Clamp(_speed - 1, 0, MAX_SPEED);
            Time.timeScale = Mathf.Pow(2.0f, _speed);
        }

        public void Reproduce(Vector2 position, Species species) {
            // TODO : Random chance to create new species 

            Species childSpecies = species;
            
            float newSpeciesRn = Random.Range(0.0f, 1.0f);
            if (newSpeciesRn < NewSpeciesChance) {
                childSpecies = species.Clone("Child");
                childSpecies.OnResearchComplete += OnResearchComplete;
                Species.Add(childSpecies);
            }

            Vector2 childOffset = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * species.GetStat(StatType.Range) * 1.5f;
            Creature child = Instantiate(CreaturePrefab, position + childOffset, Quaternion.identity).GetComponent<Creature>();
            child.Initialize(childSpecies);
        }

        private void PlaceStarting() {

            _torsos = AllMutations.Where(mutation => mutation is TorsoMutation).ToList();
            
            for (int i = 0; i < NumStartingHerbivores; i++) {
                PlaceInitialSpecies(GenerateInitialSpecies(EaterType.Herbivore, StartingHerbivoreLimit));   
            }
            
            for (int i = 0; i < NumStartingCarnivores; i++) {
                PlaceInitialSpecies(GenerateInitialSpecies(EaterType.Carnivore, StartingCarnivoreLimit));   
            }
        }

        private Species GenerateInitialSpecies(EaterType eaterType, float calorieLimit) {
            
            List<Mutation> heads = AllMutations.Where(mutation => {
                HeadMutation head = mutation as HeadMutation;
                return head != null && head.Type == eaterType;
            }).ToList();
            
            List<Mutation> remainingMutations = AllMutations.Where(mutation => {
                return (!(mutation is HeadMutation) && !(mutation is TorsoMutation));
            }).ToList();
            
            Species newSpecies = new Species("Species");
            
            newSpecies.AddMutation(_torsos[Random.Range(0, _torsos.Count)]);
            newSpecies.AddMutation(heads[Random.Range(0, heads.Count)]);
            
            while (remainingMutations.Count > 0 && newSpecies.CalorieConsumption < calorieLimit) {

                Mutation newMutation = remainingMutations[Random.Range(0, remainingMutations.Count)];
                
                newSpecies.AddMutation(newMutation);

                for (int i = remainingMutations.Count - 1; i >= 0; i--) {
                    MutationCategory category = remainingMutations[i].Category;
                    if (category != null && category == newMutation.Category) {
                        remainingMutations.RemoveAt(i);
                    }
                }
            }
            
            newSpecies.OnResearchComplete += OnResearchComplete;
            Species.Add(newSpecies);
            BeginRandomResearch(newSpecies);

            return newSpecies;
        }

        private void PlaceInitialSpecies(Species initialSpecies) {

            float spawnCount = Mathf.RoundToInt(BiomassPerStartingSpecies / initialSpecies.CalorieConsumption);
            
            for (int i = 0; i < spawnCount; i++) {
                Creature creature = Instantiate(CreaturePrefab, _world.RandomPosition(), Quaternion.identity).GetComponent<Creature>();
                creature.Initialize(initialSpecies);
            }
        }

        private void OnResearchComplete(Species species, Mutation research) {
            BeginRandomResearch(species);
        }

        private void BeginRandomResearch(Species species) {
            
            List<Mutation> availableMutations = AllMutations.Where(mutation => mutation.CanObtain(species)).ToList();
            
            if(availableMutations.Count == 0) return;

            Mutation newMutation = availableMutations[Random.Range(0, availableMutations.Count)];
            
            species.BeginResearch(newMutation);
        }

        private void OnGUI() {
            GUI.Label(new Rect(20, 20, 300, 200), "Death Count " + DeathCount);
        }
    }
}
