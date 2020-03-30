using UnityEngine;

namespace EntityAI
{
    public class CatsBehavior : MonoBehaviour
    {
        private float speed = 2.0f;
        private GameObject _target;

        public GameObject Target
        {
            get => _target;
            set => _target = value;
        }

        private void Update()
        {
            if (_target == null)
                return;
        
            //ignore the 'Y' position of the ghost
            Vector3 ghostPos = new Vector3(_target.transform.position.x, transform.position.y, _target.transform.position.z);
            Quaternion lookOnLook = Quaternion.LookRotation(ghostPos - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * speed);
        }
    }
}