using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class GhostsMovement : MonoBehaviour
{
    [System.Serializable]
    public struct Target
    {
        public List<Transform> list;
    }
    
    [SerializeField] private List<Target> targets = null;
    
    private NavMeshAgent agent = null;
    private Vector3 NextLocation = Vector3.zero;
    private int LocationIndex = 0;
    [SerializeField] private int SelectedPath = 0;
    
    private bool _isAgentNull, _isLocationNull;

    // Update is called once per frame
    private void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        SelectedPath = UnityEngine.Random.Range(0, targets.Count);
        
        _isAgentNull = agent == null;
        _isLocationNull = targets.Count <= 0;

        if (!_isLocationNull)
        {
            NextLocation = targets[SelectedPath].list[0].position;
            agent.SetDestination(NextLocation);
        }
    }

    void Update()
    {
        if (_isLocationNull || _isAgentNull)
            return;
        
        MoveToNextLocation();
    }

    void MoveToNextLocation()
    {
        agent.SetDestination(NextLocation);
        
        IsArrived();
    }

    void IsArrived()
    {
        float dist=agent.remainingDistance;
        if (!agent.pathPending)
        {
            if (!float.IsPositiveInfinity(dist) && agent.pathStatus == NavMeshPathStatus.PathComplete &&
                Math.Abs(agent.remainingDistance) < 0.5f)
            {
                if (LocationIndex < targets[SelectedPath].list.Count - 1)
                    LocationIndex++;
                else
                    LocationIndex = 0;

                NextLocation = targets[SelectedPath].list[LocationIndex].position;
            }
        }
    }
}































