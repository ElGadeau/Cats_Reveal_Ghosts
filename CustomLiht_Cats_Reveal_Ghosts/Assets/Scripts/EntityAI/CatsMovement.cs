using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatsMovement : MonoBehaviour
{
    [SerializeField] private GameObject[] Ghosts;

    private GameObject LookedGhost = null;
    private float speed = 2.0f;
    
    private void Awake()
    {
        FindAllGhosts();
    }

    private void FindAllGhosts()
    {
        Ghosts = GameObject.FindGameObjectsWithTag("Ghost");
    }

    private void Update()
    {
        FindClosestGhost();
        
        //Look at the ghost
        if (LookedGhost != null)
        {
            Vector3 ghostPos = new Vector3(LookedGhost.transform.position.x, transform.position.y, LookedGhost.transform.position.z);
            Quaternion lookOnLook = Quaternion.LookRotation(ghostPos - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * speed);
        }
    }

    void FindClosestGhost()
    {
        foreach (var ghost in Ghosts)
        {
            if (ghost == null)
            {
                FindAllGhosts();
                break;
            }
            
            if (LookedGhost == null)
                LookedGhost = ghost;

            if (Vector3.Distance(transform.position, ghost.transform.position) <
                Vector3.Distance(transform.position, LookedGhost.transform.position))
                LookedGhost = ghost;
        }
    }
}