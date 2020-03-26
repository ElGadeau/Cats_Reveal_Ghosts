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

    private bool _canSpawn = false;
    private int _numberGhost = 0;
    private int _numberCats = 0;
    private GameObject _ghosts, _cats, _player;
    
    public void SpawnEntity()
    {
        //Create new empty objects for sorting purposes
        _ghosts = new GameObject("GHOSTS");
        _cats = new GameObject("CATS");

        //Destroy all existing ghosts and cats befor spawning new ones
        DestroyEntitys();
        
        SpawnGhost();
        SpawnCats();
        SpawnPlayer();
    }
    
    //Look for all possible spawn location and choose randomly x% of them
    private void SpawnCats()
    {
        if (!_canSpawn)
            return;

        List<GameObject> catSpawns = GameObject.FindGameObjectsWithTag("CatSpawn").ToList();
        _numberCats = (int) (catSpawns.Count * percentageOfCats);

        for (int i = 0; i < _numberCats; i++)
        {
            int rng = Random.Range(0, catSpawns.Count);
            GameObject cat = Instantiate(catPrefab, _cats.transform);
            cat.transform.position = catSpawns[rng].transform.position;
            catSpawns.RemoveAt(rng);
        }
    }
    
    //Look for all possible spawn location and choose randomly x% of them
    private void SpawnGhost()
    {
        if (!_canSpawn)
            return;
        
        List<GameObject> worldTiles = GameObject.FindGameObjectsWithTag("Tile").ToList();
        _numberGhost = (int) (worldTiles.Count * percentageOfGhost);
        
        for (int i = 0; i < _numberGhost; i++)
        {
            int rng = Random.Range(0, worldTiles.Count);

            var targets = FindTargetPoints(worldTiles[rng].transform.Find("GhostData"));
            
            GameObject ghost = Instantiate(ghostPrefab, _ghosts.transform);
            ghost.transform.position = targets[0].list[0].position;
            ghost.GetComponent<GhostsMovement>().SetTargets(targets);
            worldTiles.RemoveAt(rng);
        }
    }

    //Find the starting tile of the map and spawn a player at this location
    private void SpawnPlayer()
    {
        if (_player == null)
            _player = Instantiate(playerPrefab);

        if (!_canSpawn)
        {
            _player.SetActive(false);
            return;
        }

        var point = GameObject.FindGameObjectsWithTag("StartTile")[0].transform;
        _player.transform.position = point.position;
        _player.SetActive(true);
    }
    
    //Find the position of the navigation path for the selected ghost
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

    public void RegeneratePlayer()
    {
        _player.SetActive(false);
        SpawnPlayer();
    }
    
    public void RegenerateGhosts()
    {
        DestroyGhosts();
        SpawnGhost();
    }

    public void RegenerateCats()
    {
        DestroyCats();
        SpawnCats();
    }

    private void DestroyEntitys()
    {
        DestroyGhosts();
        DestroyCats();
    }

    private void DestroyGhosts()
    {
        var allGhosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (var ghost in allGhosts)
        {
            Destroy(ghost);
        }
    }

    private void DestroyCats()
    {
        var allCats = GameObject.FindGameObjectsWithTag("Cat");
        
        foreach (var cat in allCats)
        {
            Destroy(cat);
        }
    }

    public bool CanSpawn
    {
        get => _canSpawn;
        set => _canSpawn = value;
    }
}
