using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolvingWilds {
    [RequireComponent(typeof(CircleCollider2D))]
    public class Senses : MonoBehaviour {

        private List<WildsEntity> _objectsInRange = new List<WildsEntity>();

        public List<WildsEntity> ObjectsInRange { get { return _objectsInRange; } }

        public event Action<WildsEntity> OnObjectEnterSight;
        public event Action<WildsEntity> OnObjectLeaveSight;

        public float Range {
            set { GetComponent<CircleCollider2D>().radius = value; }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            WildsEntity entity = other.gameObject.GetComponent<WildsEntity>();
            _objectsInRange.Add(entity);
            if (OnObjectEnterSight != null) {
                OnObjectEnterSight(entity);
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            WildsEntity entity = other.gameObject.GetComponent<WildsEntity>();
            _objectsInRange.Remove(entity);
            if (OnObjectLeaveSight != null) {
                OnObjectLeaveSight(entity);
            }
        }
    }
}