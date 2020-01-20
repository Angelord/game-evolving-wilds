using System;
using System.Collections.Generic;
using System.Linq;
using EvolvingWilds;
using UnityEngine;

namespace Claw.AI.Steering {
    [RequireComponent(typeof(Rigidbody2D))]
    public class SteeringController : MonoBehaviour {

        private float speed = 12.0f;
        private Rigidbody2D rBody;
        private List<SteeringBehaviour> behaviours = new List<SteeringBehaviour>();
        private Vector2 accumForce;
        private World _world;
        private Creature _creature;

        public float Speed { get { return speed; } set { speed = value; } }
        
        public Creature Creature => _creature;

        private void Start() {
            _world = World.Instance;
            _creature = GetComponent<Creature>();
            rBody = GetComponent<Rigidbody2D>();
        }

        public T AddBehaviourBack<T>() where T : SteeringBehaviour {
            return AddBehaviour<T>(behaviours.Count);
        }

        public T AddBehaviourFront<T>() where T : SteeringBehaviour {
            return AddBehaviour<T>(0);
        }
        
        public T AddBehaviour<T>(int pos = 0) where T : SteeringBehaviour {
            T newBehaviour = gameObject.AddComponent<T>();

            if (pos < 0) { pos = 0; }
            else if (pos > behaviours.Count) { pos = behaviours.Count; }

            behaviours.Insert(pos, newBehaviour);
            newBehaviour.Initialize();
            return newBehaviour;
        }

        public T GetBehaviour<T>() where T : SteeringBehaviour {
            foreach (SteeringBehaviour steeringBehaviour in behaviours) {
                if (steeringBehaviour is T) {
                    return steeringBehaviour as T;
                }
            }

            return null;
        }

        public void DisableAll() {
            foreach (var steeringBehaviour in behaviours) {
                steeringBehaviour.enabled = false;
            }
        }
        
        public void EnableAll() {
            foreach (var steeringBehaviour in behaviours) {
                steeringBehaviour.enabled = true;
            }
        }
        
        public void MoveToFront(SteeringBehaviour behaviour) {
            behaviours.Remove(behaviour);
            behaviours.Insert(0, behaviour);
        }

        public void SortBehaviours() {
            behaviours = behaviours.OrderBy(behaviour => behaviour.Priority).ToList();
        }

        private void FixedUpdate() {
            accumForce = Vector2.zero;

            foreach (SteeringBehaviour behaviour in behaviours) {
                if (!behaviour.enabled) {
                    continue;
                }

                Vector2 force = behaviour.CalculateForce();
                if (!AccumulateForce(force)) {
                    break;
                }
            }
            
            rBody.AddForce(accumForce, ForceMode2D.Impulse);
            
            Wrap();

            if (Mathf.Abs(rBody.velocity.x) >= 0.5f * Speed) {
                Flip();
            }
        }

        private void Flip() {
            if (rBody.velocity.x > 0.0f) {
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            }
            else {
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }

        private bool AccumulateForce(Vector2 toAdd) {
            
            float magCurrent = accumForce.magnitude;

            float magRemaining = speed - magCurrent;
            if (magRemaining <= 0.0f) { return false; }

            float magToAdd = toAdd.magnitude;
            if (magToAdd > magRemaining) {
                accumForce += toAdd.normalized * magRemaining;
                return false;
            }
         
            accumForce += toAdd;
            return true;
        }

        private void Wrap() {
            if (transform.position.y > _world.Top) {
                transform.position = new Vector2(transform.position.x, _world.Bottom + 1.0f);
            }
            else if (transform.position.y < _world.Bottom) {
                transform.position = new Vector2(transform.position.x, _world.Top - 1.0f);
            }
            
            if (transform.position.x > _world.Right) {
                transform.position = new Vector2(_world.Left + 1.0f, transform.position.y);
            }
            else if(transform.position.x < _world.Left) {
                transform.position = new Vector2(_world.Right - 1.0f, transform.position.y);
            }
        }
    }
}