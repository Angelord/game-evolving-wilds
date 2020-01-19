using Claw.AI.Steering;
using UnityEngine;

namespace EvolvingWilds {
    public abstract class CreatureState {

        private Creature _creature;
        private float _utility;
        private bool _done;
        private bool _isCurrent;

        public abstract WildsEntity Target { get; }

        public Creature Creature { get { return _creature; } }

        public Species Species { get { return _creature.Species; } }

        public SteeringController Steering { get { return _creature.Steering; } }

        public float Utility { get { return _utility; } }

        public bool Done { get { return _done; } protected set { _done = value; } }

        public bool IsCurrent { get { return _isCurrent; } }

        public CreatureState(Creature creature) {
            _creature = creature;
        }

        public void CalculateUtility() {
            _utility = DoUtilityCalculation();
            if (Creature.Species.EaterType == EaterType.Carnivore) {
                Debug.Log("utility for " + this.GetType() + " : " + _utility);
            }
        }

        protected abstract float DoUtilityCalculation();

        public void Enter() {
            _isCurrent = true;
            OnEnter();
        }

        public void Exit() {
            _isCurrent = false;
            OnExit();
        }

        protected virtual void OnEnter() { }

        protected virtual void OnExit() { }

        public virtual void Update() { }
    }
}