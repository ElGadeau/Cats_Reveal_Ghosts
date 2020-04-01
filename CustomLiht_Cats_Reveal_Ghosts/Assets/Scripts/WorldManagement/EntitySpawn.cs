using System;
using System.Collections.Generic;
using Character;
using EntityAI;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace WorldManagement
{
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
        
        private List<GameObject> _tileList = new List<GameObject>();
    
        public void SpawnEntity(List<GameObject> p_tiles)
        {
            _tileList = p_tiles;
            
            //Create new empty objects for sorting purposes
            _ghosts = new GameObject("GHOSTS");
            _cats = new GameObject("CATS");

            //Destroy all existing ghosts and cats before spawning new ones
            DestroyEntitys();
        
            //first -> choose randomly '%' of tile to spawn ghost
            ChooseSpawnableTile(_tileList);
        
            //the spawn will be made based on the tile instead
        
            foreach (var tile in _tileList)
            {
                GameObject ghost = null;
                if (tile.GetComponent<Tiles.Tiles>().SpawnGhost)
                    ghost = SpawnGhost(tile);
            
                SpawnCats(tile, ghost);
            }
        
            // SpawnGhost();
            // SpawnCats();

            //spawn the player
            SpawnPlayer();
        }
    
        //Look for all possible spawn location and choose randomly x% of them
        private void SpawnCats(GameObject p_tile, GameObject p_ghost)
        {
            if (!_canSpawn)
                return;

            // List<GameObject> catSpawns = GameObject.FindGameObjectsWithTag("CatSpawn").ToList();
            var catSpawns = FindCatSpawns(p_tile.transform.Find("CatData"));

            if (catSpawns == null)
                return;

            // _numberCats = (int) (catSpawns.Count * percentageOfCats);
            _numberCats = (int)Math.Ceiling(catSpawns.Count * percentageOfCats);
        
            for (int i = 0; i < _numberCats; i++)
            {
                int rng = Random.Range(0, catSpawns.Count);
                GameObject cat = Instantiate(catPrefab, _cats.transform);
                cat.transform.position = catSpawns[rng].transform.position;
                cat.GetComponent<CatsBehavior>().Target = p_ghost;
                catSpawns.RemoveAt(rng);
            }
        }
    
        //Look for all possible spawn location and choose randomly x% of them
        private GameObject SpawnGhost(GameObject p_tile)
        {
            if (!_canSpawn)
                return null;

            var targets = FindTargetPoints(p_tile.transform.Find("GhostData"));
        
            GameObject ghost = Instantiate(ghostPrefab, _ghosts.transform);
            ghost.GetComponent<NavMeshAgent>().Warp(targets[0].list[0].position);
            
            // ghost.transform.position = targets[0].list[0].position;
            ghost.GetComponent<GhostsBehavior>().SetTargets(targets);

            return ghost;
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

            var point = GameObject.FindGameObjectWithTag("PlayerSpawn").transform;
            _player.transform.position = point.position;
            _player.transform.rotation = Quaternion.Euler(-90, -90, 0);
            _player.SetActive(true);
            _player.GetComponent<PlayerMovement>()._isDead = false;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControl>().FadeFromBlack();
        }
    
        //Find the position of the navigation path for the selected ghost
        private List<GhostsBehavior.Target> FindTargetPoints(Transform p_transform)
        {
            var targetList = new List<GhostsBehavior.Target>();
        
            foreach (Transform child in p_transform)
            {
                if (child.CompareTag("TargetPoints"))
                {
                    GhostsBehavior.Target tmpTr = new GhostsBehavior.Target();

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

        private List<Transform> FindCatSpawns(Transform p_transform)
        {
            var spawnList = new List<Transform>();

            if (p_transform == null)
                return null;
            
            foreach (Transform child in p_transform)
            {
                if (child.CompareTag("CatSpawn"))
                {
                    spawnList.Add(child);
                }
            }
        
            return spawnList;
        }

        private void ChooseSpawnableTile(List<GameObject> p_tiles)
        {
            foreach (GameObject tile in p_tiles)
            {
                tile.GetComponent<Tiles.Tiles>().SpawnGhost = false;
            }
            
            int numberTiles = (int) Math.Ceiling(p_tiles.Count * percentageOfGhost);

            var tileList = new List<GameObject>();

            for (int i = 0; i < numberTiles; ++i)
            {
                int rng = Random.Range(0, p_tiles.Count);

                p_tiles[rng].GetComponent<Tiles.Tiles>().SpawnGhost = true;
                tileList.Add(p_tiles[rng]);
                p_tiles.RemoveAt(rng);
            }

            foreach (var tile in tileList)
            {
                p_tiles.Add(tile);
            }
        }

        public void RegeneratePlayer()
        {
            _player.SetActive(false);
            SpawnPlayer();
        }
    
        // public void RegenerateGhosts()
        // {
        //     DestroyGhosts();
        //     SpawnGhost();
        // }
        //
        // public void RegenerateCats()
        // {
        //     DestroyCats();
        //     SpawnCats();
        // }

        public void RegenerateEntitys()
        {
            DestroyEntitys();
            
            ChooseSpawnableTile(_tileList);
            foreach (var tile in _tileList)
            {
                GameObject ghost = null;
                if (tile.GetComponent<Tiles.Tiles>().SpawnGhost)
                    ghost = SpawnGhost(tile);
            
                SpawnCats(tile, ghost);
            }
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
}
