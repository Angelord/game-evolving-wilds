using UnityEngine;

namespace EvolvingWilds {
    public class World : MonoBehaviour {

        public float Width;
        public float Height;
        public float Left;
        public float Right;
        public float Bottom;
        public float Top;
        
        private static World _instance;

        public static World Instance { get { return _instance; } }

        private void Awake() {
            _instance = this;
            Transform transf = transform;
            Left = transf.position.x - Width / 2;
            Right = transf.position.x + Width / 2;
            Bottom = transf.position.y - Height / 2;
            Top = transf.position.y + Height / 2;
        }

        private void OnDrawGizmos() {
            Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height, 0.0f));
        }

        public Vector2 RandomPosition() {

            float x = Random.Range(Left, Right);
            float y = Random.Range(Bottom, Top);

            return new Vector2(x, y);
        }
    }
}