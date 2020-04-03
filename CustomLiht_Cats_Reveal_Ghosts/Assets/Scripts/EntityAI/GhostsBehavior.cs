using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace EntityAI
{
    public class GhostsBehavior : MonoBehaviour
    {
        [Serializable]
        public class Target
        {
            public Target()
            {
                list = new List<Transform>();
            }
        
            public List<Transform> list;
        }
    
        [SerializeField] private List<Target> targets = new List<Target>();
        [SerializeField] private Vector3 NextLocation = Vector3.zero;
        [SerializeField] private int LocationIndex = 0;
        [SerializeField] private int SelectedPath = 0;
        [SerializeField] private SkinnedMeshRenderer _meshRenderer = null;
        [SerializeField] private Animator myAnimator = null;
        [SerializeField] private ParticleSystem myParticle = null;
        
        private NavMeshAgent Agent = null;
        private bool _isAgentNull, _isLocationNull;

        private void Awake()
        {
            myParticle.Stop();
            Agent = gameObject.GetComponent<NavMeshAgent>();
            _meshRenderer.enabled = false;
            
            _isAgentNull = Agent == null;
            _isLocationNull = targets.Count <= 0;

            // GetComponent<MeshRenderer>().enabled = false;
            
            if (!_isLocationNull)
            {
                NextLocation = targets[SelectedPath].list[0].position;
                Agent.SetDestination(NextLocation);
            }
        }

        private void Update()
        {
            if (_isLocationNull || _isAgentNull)
                return;

            if (targets[SelectedPath].list.Count <= 1)
            {
                GetComponent<MeshRenderer>().enabled = true;
            }
            
            MoveToNextLocation();
        }

        private void MoveToNextLocation()
        {
            Agent.SetDestination(NextLocation);
        
            IsArrived();
        }

        private void IsArrived()
        {
            float dist=Agent.remainingDistance;
            if (!Agent.pathPending)
            {
                if (!float.IsPositiveInfinity(dist) && Agent.pathStatus == NavMeshPathStatus.PathComplete &&
                    Math.Abs(Agent.remainingDistance) < 0.5f)
                {
                    if (LocationIndex < targets[SelectedPath].list.Count - 1)
                        LocationIndex++;
                    else
                        LocationIndex = 0;

                    if (targets[SelectedPath].list[LocationIndex] != null)
                        NextLocation = targets[SelectedPath].list[LocationIndex].position;
                }
            }
        }

        public void SetTargets(List<Target> p_targets)
        {
            if (p_targets.Count <= 0)
                return;
        
            targets = p_targets;
            SelectedPath = UnityEngine.Random.Range(0, targets.Count);
            NextLocation = targets[SelectedPath].list[0].position;
            _isLocationNull = targets.Count <= 0;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Furniture"))
            {
                Physics.IgnoreCollision(other.collider, GetComponent<Collider>());
            }

            if (other.gameObject.CompareTag("Player"))
            {
                if (myParticle != null)
                    myParticle.Play();
                
                _meshRenderer.enabled = true;
                myAnimator.SetBool("Idle", false);
                Invoke(nameof(ReturnToNormal), 3.0f);
            }
        }

        private void ReturnToNormal()
        {
            _meshRenderer.enabled = false;
            myAnimator.SetBool("Idle", true);
        }
    }
}