using System.Collections.Generic;

namespace EvolvingWilds {
	public class Species {

		private static readonly float[] BASE_STATS = new float[(int)StatType.Count];
		private float _calorieConsumption = 100.0f;
		
		private List<Mutation> _mutations = new List<Mutation>();

		public float CalorieConsumption { get { return _calorieConsumption; } }

		public int MutationCount { get { return _mutations.Count; } }

		public Species Clone() {
			Species clone = new Species();
			clone._calorieConsumption = _calorieConsumption;
			clone._mutations = new List<Mutation>(_mutations);
			return clone;
		}

		public Mutation GetMutation(int index) {
			return _mutations[index];
		}

		public bool HasMutation(Mutation mutation) {
			return _mutations.Contains(mutation);
		}

		public void AddMutation(Mutation mutation) {
			mutation.OnAdd(this);
			_mutations.Add(mutation);
			_calorieConsumption += mutation.CalorieConsumption;
		}

		public void RemoveMutation(Mutation mutation) {
			mutation.OnRemove(this);
			_mutations.Remove(mutation);
			_calorieConsumption -= mutation.CalorieConsumption;
		}
		
		public float GetStat(StatType type) {
			float value = BASE_STATS[(int) type];

			foreach (var mutation in _mutations) {
				mutation.ModifyStat(type, ref value);
			}

			return value;
		}
	}
}
