using System;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace EvolvingWilds {
	[RequireComponent(typeof(World))]
	public class Vegetation : MonoBehaviour {

		public int InitialSpawn;	// How many are spawned initially
		public float MinSpawnRate;
		public float MaxSpawnRate;
		public GameObject Vegetable;

		private World _world;
		private float _nextSpawn;

		private void Start() {
			_world = GetComponent<World>();
			_nextSpawn = GetSpawnDelay();
			
			for (int i = 0; i < InitialSpawn; i++) {
				Spawn();
			}
		}

		private void Update() {

			if (Time.time >= _nextSpawn) {
				
				Spawn();
				
				_nextSpawn = Time.time + GetSpawnDelay();
			}
		}

		private void Spawn() {

			Vector2 pos = RandomSpawnPos();

			Instantiate(Vegetable, pos, Quaternion.identity);
		}

		private Vector2 RandomSpawnPos() {

			float x = Random.Range(_world.Left, _world.Right);
			float y = Random.Range(_world.Bottom, _world.Top);
			
			return new Vector2(x, y);
		}

		private float GetSpawnDelay() {
			return Random.Range(MinSpawnRate, MaxSpawnRate);
		}
	}
}
