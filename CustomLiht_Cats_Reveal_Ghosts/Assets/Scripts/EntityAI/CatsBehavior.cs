using System;
using UnityEngine;
using Random = System.Random;

namespace EntityAI
{
    public class CatsBehavior : MonoBehaviour
    {
        [SerializeField] private Animator myAnimator = null;
        
        private float speed = 2.0f;
        public GameObject _target;
        
        public GameObject Target
        {
            get => _target;
            set
            {
                _target = value;
                if (_target != null)
                    myAnimator.SetBool("Angry", true);
            }
        }

        private void Awake()
        {
            if (myAnimator == null)
                myAnimator = GetComponent<Animator>();

            if (_target != null)
                myAnimator.SetBool("Angry", true);
            else
            {
                myAnimator.SetBool("Angry", false);
                myAnimator.speed = UnityEngine.Random.Range(0.5f, 1.5f);
                transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0);
            }
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