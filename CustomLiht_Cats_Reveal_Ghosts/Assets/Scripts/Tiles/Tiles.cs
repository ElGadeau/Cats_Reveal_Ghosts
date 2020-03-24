using UnityEngine;

public class Tiles : MonoBehaviour
{
    public Sides[] sides;
    public Collider colliders;

    public Bounds TileBound
    {
        get { return colliders.bounds; }
    }
}