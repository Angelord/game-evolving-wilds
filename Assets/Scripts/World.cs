using UnityEngine;

namespace EvolvingWilds {
    public class World : MonoBehaviour {

        public float Width;
        public float Height;

        public float Left { get { return transform.position.x - Width/2; } }
        public float Right { get { return transform.position.x + Width/2; } }
        public float Bottom { get { return transform.position.y - Height/2; } }
        public float Top { get { return transform.position.y + Height/2; } }

        private void OnDrawGizmos() {
            Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height, 0.0f));
        }
		
    }
}