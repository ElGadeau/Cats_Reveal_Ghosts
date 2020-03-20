using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatsMovement : MonoBehaviour
{
    [SerializeField] private GameObject[] Ghosts;

    private GameObject LookedGhost = null;
    private float speed = 2.0f;
    
    // Start is called before the first frame update
    private void Awake()
    {
        Ghosts = GameObject.FindGameObjectsWithTag("Ghost");
    }

    // Update is called once per frame
    private void Update()
    {
        FindClosestGhost();
        
        //Look at the ghost
        if (LookedGhost != null)
        {
            // transform.LookAt(LookedGhost.transform);
            
            Vector3 ghostPos = new Vector3(LookedGhost.transform.position.x, transform.position.y, LookedGhost.transform.position.z);
            Quaternion lookOnLook = Quaternion.LookRotation(ghostPos - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * speed);
        }
    }

    void FindClosestGhost()
    {
        foreach (var ghost in Ghosts)
        {
            if (LookedGhost == null)
                LookedGhost = ghost;

            if (Vector3.Distance(transform.position, ghost.transform.position) <
                Vector3.Distance(transform.position, LookedGhost.transform.position))
                LookedGhost = ghost;
        }
    }
}