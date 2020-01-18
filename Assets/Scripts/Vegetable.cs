using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EvolvingWilds {
	public class Vegetable : MonoBehaviour {

		public int MaxValue = 100;

		[HideInInspector] public float RemainingValue;

		private void Start() {
			RemainingValue = MaxValue;
			
			// TODO : Randomize
		}
	}
}
