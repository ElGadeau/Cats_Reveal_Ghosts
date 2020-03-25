using UnityEngine;

public class Tiles : MonoBehaviour
{
    public Sides[] sides;
    public Collider colliders;

    public Bounds TileBound => colliders.bounds;
}