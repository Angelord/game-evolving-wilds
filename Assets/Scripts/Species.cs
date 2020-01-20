using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EvolvingWilds {
	public class Species {
		
		public class MutationResearch {
			public Mutation Mutation;
			public float Progress;

			public MutationResearch(Mutation mutation) {
				Mutation = mutation;
				Progress = 0.0f;
			}

			public void Update() {
				Progress += Time.deltaTime;
			}

			public bool IsDone() {
				return Progress / Mutation.ResearchTime >= 1.0f;
			}
		}

		private static readonly float[] BASE_STATS = new float[(int) StatType.Count] {
			10.0f, // Foraging
			5.0f, // Damage
			0.5f, // Attack Speed
			0.5f, // Calorie Efficiency
			100.0f, // Health
			0.1f, // Health Regen
			1.2f, // Movement Speed
			3.0f, // Sight
			0.5f // Range
		};
		private float _calorieConsumption = 100.0f;
		private EaterType _eaterType;
		private string _name;
		
		private List<Mutation> _mutations = new List<Mutation>();
		private MutationResearch _research;

		public event Action<Species, Mutation> OnResearchComplete;
		public event Action<Mutation> OnMutationAdded;
		public event Action<Mutation> OnMutationRemoved;
		public Color Color;

		public string Name { get { return _name; } }

		public float CalorieConsumption { get { return _calorieConsumption; } }

		public int MutationCount { get { return _mutations.Count; } }

		public EaterType EaterType { get { return _eaterType; } set { _eaterType = value; } }

		public bool Researching { get { return _research != null; } }

		public Species(string name) {
			_name = name;
			Color = new Color(Random.Range(0.4f, 1.0f), Random.Range(0.4f, 1.0f), Random.Range(0.4f, 1.0f));
		}

		public Species Clone(string cloneName) {
			Species clone = new Species(cloneName);
			clone._calorieConsumption = _calorieConsumption;
			clone._mutations = new List<Mutation>(_mutations);
			return clone;
		}

		public bool CanEat(FoodType foodType) {
			return (_eaterType == EaterType.Omnivore
			        || (foodType == FoodType.Vegetable && _eaterType == EaterType.Herbivore)
			        || (foodType == FoodType.Meat && _eaterType == EaterType.Carnivore));
		}

		public Mutation GetMutation(int index) {
			return _mutations[index];
		}

		public bool HasMutation(Mutation mutation) {
			return _mutations.Contains(mutation);
		}

		public void AddMutation(Mutation mutation) {
			mutation.OnAddToSpecies(this);
			_mutations.Add(mutation);
			_calorieConsumption += mutation.CalorieConsumption;
			
			if (OnMutationAdded != null) {
				OnMutationAdded(mutation);
			}
		}

		public void RemoveMutation(Mutation mutation) {
			mutation.OnRemoveFromSpecies(this);
			_mutations.Remove(mutation);
			_calorieConsumption -= mutation.CalorieConsumption;
			
			if (OnMutationRemoved != null) {
				OnMutationRemoved(mutation);
			}
		}
		
		public float GetStat(StatType type) {
			float value = BASE_STATS[(int) type];

			foreach (var mutation in _mutations) {
				mutation.ModifyStat(type, ref value);
			}

			return value;
		}

		public void BeginResearch(Mutation mutation) {
			_research = new MutationResearch(mutation);
		}

		public void UpdateResearch() {
			if(_research == null) return;

			_research.Update();
			if (_research.IsDone()) {
				Debug.Log("Research complete!");
				Mutation newMutation = _research.Mutation;
				_research = null;
				if(OnResearchComplete != null)
					OnResearchComplete(this, newMutation);
				AddMutation(newMutation);
			}
		}
	}
}
