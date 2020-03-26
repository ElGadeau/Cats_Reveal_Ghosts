using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class GhostsMovement : MonoBehaviour
{
    [System.Serializable]
    public class Target
    {
        public Target()
        {
            list = new List<Transform>();
        }
        
        public List<Transform> list;
    }
    
    [SerializeField] private List<Target> targets = new List<Target>();
    private NavMeshAgent Agent = null;
    [SerializeField] private Vector3 NextLocation = Vector3.zero;
    [SerializeField] private int LocationIndex = 0;
    [SerializeField] private int SelectedPath = 0;
    
    private bool _isAgentNull, _isLocationNull;

    private void Awake()
    {
        Agent = gameObject.GetComponent<NavMeshAgent>();

        _isAgentNull = Agent == null;
        _isLocationNull = targets.Count <= 0;

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
    }
}