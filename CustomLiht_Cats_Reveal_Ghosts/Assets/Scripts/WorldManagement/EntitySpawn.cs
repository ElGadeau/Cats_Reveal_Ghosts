using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class EntitySpawn : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject ghostPrefab = null;
    [SerializeField] private GameObject catPrefab = null;
    [SerializeField] private GameObject playerPrefab = null;

    [Header("Entity Data")]
    [SerializeField] [Range(0, 1)] private float percentageOfGhost = 0.75f;
    [SerializeField] [Range(0, 1)] private float percentageOfCats = 0.75f;

    private List<GameObject> worldTiles = null;
    private int NumberGhost = 0;
    private int NumberCats = 0;
    private GameObject ghosts, cats;

    public void SpawnEntity()
    {
        FindAllTiles();
        
        ghosts = new GameObject("GHOSTS");
        cats = new GameObject("CATS");

        SpawnGhost();
        SpawnCats();
        SpawnPlayer();
    }

    private void FindAllTiles()
    {
        worldTiles = GameObject.FindGameObjectsWithTag("Tile").ToList();
    }

    private void SpawnCats()
    {
        List<GameObject> catSpawns = GameObject.FindGameObjectsWithTag("CatSpawn").ToList();
        NumberCats = (int) (catSpawns.Count * percentageOfCats);

        for (int i = 0; i < NumberCats; i++)
        {
            int rng = Random.Range(0, catSpawns.Count);
            GameObject cat = Instantiate(catPrefab, cats.transform);
            cat.transform.position = catSpawns[rng].transform.position;
            catSpawns.RemoveAt(rng);
        }
    }
    
    
    private void SpawnGhost()
    {
        NumberGhost = (int) (worldTiles.Count * percentageOfGhost);
        
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

    private void SpawnPlayer()
    {
        var point = GameObject.FindGameObjectsWithTag("StartTile")[0].transform;

        // Debug.LogError("point size:" + point.Length);
        
        GameObject witch = Instantiate(playerPrefab);
        witch.transform.position = point.position;
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
