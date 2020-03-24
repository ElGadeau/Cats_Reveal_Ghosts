using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntitySpawn : MonoBehaviour
{

    [SerializeField] [Range(0, 1)] private float PourcentageOfGhost = 75;
    [SerializeField] [Range(0, 1)] private float PourcentageOfCats = 75;

    [SerializeField] private GameObject ghostPrefab = null;
    [SerializeField] private GameObject catPrefab = null;


    private List<GameObject> worldTiles = null;
    private int NumberGhost;
    private int NumberCats;
    
    private GameObject ghosts, cats;

    public void SpawnEntity()
    {
        FindAllTiles();
        
        ghosts = new GameObject("GHOSTS");
        cats = new GameObject("CATS");

        SpawnGhost();
        SpawnCats();
    }

    private void FindAllTiles()
    {
        worldTiles = GameObject.FindGameObjectsWithTag("Tile").ToList();
    }

    private void SpawnCats()
    {
        List<GameObject> catSpawns = GameObject.FindGameObjectsWithTag("CatSpawn").ToList();
        // NumberCats = Random.Range(catNumberRange.x, catNumberRange.y + 1);
        NumberCats = (int) (catSpawns.Count * PourcentageOfCats);

        for (int i = 0; i < NumberCats; i++)
        {
            int rng = Random.Range(0, catSpawns.Count);
            // Transform spawnPoint = catSpawns[rng].transform;
            GameObject cat = Instantiate(catPrefab, cats.transform);
            cat.transform.position = catSpawns[rng].transform.position;
            catSpawns.RemoveAt(rng);
        }
    }
    
    
    private void SpawnGhost()
    {
        NumberGhost = (int) (worldTiles.Count * PourcentageOfGhost);
        
        for (int i = 0; i < NumberGhost; i++)
        {
            int rng = Random.Range(0, worldTiles.Count);

            var targets = FindTargetPoints(worldTiles[rng].transform.Find("GhostData"));
            
            GameObject ghost = Instantiate(ghostPrefab, ghosts.transform);
            ghost.transform.position = targets[0].list[0].position;
            ghost.GetComponent<GhostsMovement>().SetTargets(targets);
            worldTiles.RemoveAt(rng);
        }
    }

    private List<GhostsMovement.Target> FindTargetPoints(Transform p_transform)
    {
        List<GhostsMovement.Target> targetList = new List<GhostsMovement.Target>();
        
        foreach (Transform child in p_transform)
        {
            if (child.CompareTag("TargetPoints"))
            {
                GhostsMovement.Target tmpTr = new GhostsMovement.Target();

                foreach (Transform point in child.transform)
                {
                    tmpTr.list.Add(point);
                }
                    
                if (tmpTr.list.Count > 0)
                    targetList.Add(tmpTr);
            }
        }

        return targetList;
    }
}
