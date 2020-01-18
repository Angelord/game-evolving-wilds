using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolvingWilds {
    [RequireComponent(typeof(CircleCollider2D))]
    public class Senses : MonoBehaviour {

        private List<WildsEntity> _objectsInRange = new List<WildsEntity>();

        public List<WildsEntity> ObjectsInRange { get { return _objectsInRange; } }

        public event Action OnObjectsInSightChange;

        public float Range {
            set { GetComponent<CircleCollider2D>().radius = value; }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            _objectsInRange.Add(other.gameObject.GetComponent<WildsEntity>());
            if (OnObjectsInSightChange != null) {
                OnObjectsInSightChange();
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            _objectsInRange.Remove(other.gameObject.GetComponent<WildsEntity>());
            if (OnObjectsInSightChange != null) {
                OnObjectsInSightChange();
            }
        }
    }
}