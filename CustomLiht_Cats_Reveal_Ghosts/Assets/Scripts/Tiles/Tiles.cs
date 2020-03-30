using UnityEngine;

namespace Tiles
{
    public class Tiles : MonoBehaviour
    {
        public Sides[] sides;
        public Collider colliders;
        private bool spawnGhost = false;

        public Bounds TileBound => colliders.bounds;

        public bool SpawnGhost
        {
            get => spawnGhost;
            set => spawnGhost = value;
        }
    }
}