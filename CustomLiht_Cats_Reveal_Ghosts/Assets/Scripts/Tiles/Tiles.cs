using UnityEngine;

namespace Tiles
{
    public class Tiles : MonoBehaviour
    {
        public Sides[] sides;
        public Collider colliders;

        public Bounds TileBound => colliders.bounds;

        public bool SpawnGhost { get; set; } = false;
    }
}