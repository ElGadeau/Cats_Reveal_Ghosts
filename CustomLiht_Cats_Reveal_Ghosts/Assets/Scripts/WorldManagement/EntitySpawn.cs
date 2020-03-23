using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntitySpawn : MonoBehaviour
{
    [SerializeField] private Vector2Int ghostNumberRange;
    [SerializeField] private Vector2Int catNumberRange;

    [SerializeField] private GameObject ghostPrefab = null;
    [SerializeField] private GameObject catPrefab = null;

    //TODO remove this list and use Tags instead. At start, the manager will find GO with tag and will add them to a list
    [SerializeField] private List<GameObject> worldTiles = null;

    [SerializeField] private int NumberGhost;
    [SerializeField] private int NumberCats;
    
    // Start is called before the first frame update
    private void Start()
    {
        CheckData();
        SpawnGhost();
        SpawnCats();
    }

    private void CheckData()
    {
        if (ghostNumberRange.x > worldTiles.Count || ghostNumberRange.y > worldTiles.Count || ghostNumberRange.x < 0 || ghostNumberRange.x < 0)
        {
            Debug.Log("The number of ghost asked is not valid");
            Debug.Break();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_WEBPLAYER
            Application.OpenURL(webplayerQuitURL);
            #else
            Application.Quit();
            #endif
        }
    }

    private void SpawnCats()
    {
        NumberCats = Random.Range(catNumberRange.x, catNumberRange.y + 1);

        List<GameObject> catSpawns = GameObject.FindGameObjectsWithTag("CatSpawn").ToList();
        
        Debug.Log(catSpawns.Count);
        
        for (int i = 0; i < NumberCats; i++)
        {
            int rng = Random.Range(0, catSpawns.Count);
            Transform spawnPoint = catSpawns[rng].transform;
            GameObject cat = Instantiate(catPrefab, spawnPoint);
            catSpawns.RemoveAt(rng);
        }
    }
    
    
    private void SpawnGhost()
    {
        //TODO make the ghost spawn at position [0] of the selected path
        
        NumberGhost = Random.Range(ghostNumberRange.x, ghostNumberRange.y + 1);

        for (int i = 0; i < NumberGhost; i++)
        {
            int rng = Random.Range(0, worldTiles.Count);

            var targets = FindTargetPoints(worldTiles[rng].transform.Find("GhostData"));
            GameObject ghost = Instantiate(ghostPrefab, targets[0].list[0]);
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
