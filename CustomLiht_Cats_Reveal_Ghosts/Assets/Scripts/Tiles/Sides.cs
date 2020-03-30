using UnityEngine;

namespace Tiles
{
    public class Sides : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Ray ray = new Ray(transform.position, transform.forward);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray);
        }
    }
}
