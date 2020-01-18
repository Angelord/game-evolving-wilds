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
		public GameObject[] prefabs;

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

			Vector2 pos = _world.RandomPosition();

			Instantiate(prefabs[Random.Range(0, prefabs.Length)], pos, Quaternion.identity);
		}
		
		private float GetSpawnDelay() {
			return Random.Range(MinSpawnRate, MaxSpawnRate);
		}
	}
}
