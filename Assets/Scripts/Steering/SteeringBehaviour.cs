
using System;
using UnityEngine;

namespace Claw.AI.Steering {
    [RequireComponent(typeof(SteeringController))]
    public abstract class SteeringBehaviour : MonoBehaviour {

        [SerializeField] private int priority = 0;
        [SerializeField] private float multiplier = 1.0f;
        private Rigidbody2D _rigidbody;
        private SteeringController _controller;

        public int Priority {
            get {
                return priority;
            }
            set {
                priority = value;
                _controller.SortBehaviours();
            }
        }

        protected Rigidbody2D Rigidbody { get { return _rigidbody; } }
        protected SteeringController Controller { get { return _controller; } }

        public void Initialize() {
            _rigidbody = GetComponent<Rigidbody2D>();
            _controller = GetComponent<SteeringController>();
            OnInitialize();
        }

        public void MoveToFront() {
            _controller.MoveToFront(this);
        }

        public Vector2 CalculateForce() {
            return DoForceCalculation() * multiplier;
        }

        protected abstract Vector2 DoForceCalculation();
        
        protected virtual void OnInitialize() { }

        protected T RequireBehaviour<T>() where T : SteeringBehaviour {
            T behaviour = gameObject.AddComponent<T>();
            behaviour.Initialize();
            behaviour.enabled = false;
            return behaviour;
        }

        protected int GetNearbyObjects(float radius, Collider2D[] results, int layerMask) {
            return Physics2D.OverlapCircleNonAlloc(transform.position, radius, results, layerMask);
        }

        //Needed so we can set [enabled] in the inspector
        private void Update() { }
    }
}