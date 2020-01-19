using UnityEngine;

namespace EvolvingWilds.UserInterface {
    [RequireComponent(typeof(Simulation))]
    public class SimulationControls : MonoBehaviour {


        private Simulation _simulation;

        private void Start() {
            _simulation = GetComponent<Simulation>();
        }

        private void Update() {

            if (Input.GetKeyDown(KeyCode.Space)) {
                _simulation.Paused = !_simulation.Paused;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                _simulation.DecreaseSpeed();
            }
            
            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                _simulation.IncreaseSpeed();
            }
        }
    }
}