using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tiles;
using UnityEngine;
using UnityEngine.AI;

namespace WorldManagement
{
    public class LevelGenerator : MonoBehaviour
    {
        public bool isPlaying = false;
    
        public Tiles.Tiles startTilePrefab, endTilePrefab, firstTilePrefab;
        // public Tiles.Tiles endTilePrefab;
        public List<Tiles.Tiles> tilesPrefabs = new List<Tiles.Tiles>();
        public Vector2Int iterationRange = new Vector2Int(5, 10);

        private Tiles.Tiles _startTile, _endTile, _firstTile;
        public List<GameObject> _placedTiles = new List<GameObject>();
        private List<Sides> _availableSides = new List<Sides>();

        private LayerMask _roomLayerMask;

        private void Start()
        {
            if (isPlaying)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            _roomLayerMask = LayerMask.GetMask("Tile");
            StartCoroutine(nameof(GenerateLevel));
        }
    
        IEnumerator GenerateLevel()
        {
            WaitForSeconds startup = new WaitForSeconds(1);
            WaitForFixedUpdate interval = new WaitForFixedUpdate();

            yield return startup;
        
            PlaceStartTile();
            yield return interval;

            //TODO the first tile need to be a blank floor without ghost
            PlaceFirstTile();
            yield return interval;

            int iterations = Random.Range(iterationRange.x, iterationRange.y);
            for (int i = 0; i < iterations; ++i)
            {
                PlaceRandomTile();
                yield return interval;
            }

            PlaceEndTile();
            yield return interval;
        
            GetComponent<NavMeshSurface>().BuildNavMesh();
            
            GetComponent<EntitySpawn>().CanSpawn = true;
            GetComponent<EntitySpawn>().SpawnEntity(_placedTiles);
        
            yield return null;
        }

        private void PlaceStartTile()
        {
            _startTile = Instantiate(startTilePrefab, transform);
        
            //Add all sides to the side list
            AddSideToList(_startTile, ref _availableSides);
            var tileTransform = _startTile.transform;
            tileTransform.position = Vector3.zero;
            tileTransform.rotation = Quaternion.identity;
        }

        private void PlaceFirstTile()
        {
            _firstTile = Instantiate(firstTilePrefab, transform);
            
            AddSideToList(_firstTile, ref _availableSides);
            if (PlaceTileInWorld(_firstTile, false))
                return;
        
            Destroy(_firstTile.gameObject);
            ResetLevelGenerator();
        }

        private void PlaceRandomTile()
        {
            Tiles.Tiles currentTile = Instantiate(tilesPrefabs[Random.Range(0, tilesPrefabs.Count)], transform);

            AddSideToList(currentTile, ref _availableSides);
            if (PlaceTileInWorld(currentTile))
                return;
        
            Destroy(currentTile.gameObject);
            ResetLevelGenerator();
        }

        private void PlaceEndTile()
        {
            _endTile = Instantiate(endTilePrefab, transform);

            if (!PlaceTileInWorld(_endTile, false))
                ResetLevelGenerator();
        }

        private bool PlaceTileInWorld(Tiles.Tiles p_tile, bool placeInList = true)
        {
            List<Sides> currentSides = new List<Sides>();
            AddSideToList(p_tile, ref currentSides);
        
            foreach (Sides availableSide in _availableSides)
            {
                //try all available side in current room
                foreach (Sides currentSide in currentSides)
                {
                    //position the tile
                    PositionTileAtSide(ref p_tile, currentSide, availableSide);

                    if (CheckTileOverlap(p_tile))
                    {
                        continue;
                    }

                    if (placeInList)
                        _placedTiles.Add(p_tile.gameObject);
                
                    //remove sides
                    currentSide.gameObject.SetActive(false);
                    _availableSides.Remove(currentSide);
                
                    availableSide.gameObject.SetActive(false);
                    _availableSides.Remove(availableSide);

                    return true;
                }
            }
        
            return false;
        }
    
        private void ResetLevelGenerator()
        {
            GetComponent<EntitySpawn>().CanSpawn = false;
        
            StopCoroutine(nameof(GenerateLevel));
        
            if (_startTile)
                Destroy(_startTile.gameObject);
        
            if (_firstTile)
                Destroy(_firstTile.gameObject);
            
            if (_endTile)
                Destroy(_endTile.gameObject);

            foreach (var tile in _placedTiles)
            {
                Destroy(tile.gameObject);
            }
        
            _placedTiles.Clear();
            _availableSides.Clear();

            StartCoroutine(nameof(GenerateLevel));
        }
    
        private void AddSideToList(Tiles.Tiles tile, ref List<Sides> sides)
        {
            foreach (var side in tile.sides)
            {
                int r = Random.Range(0, sides.Count);
                sides.Insert(r, side);
            }
        }

        private void PositionTileAtSide(ref Tiles.Tiles tile, Sides tileSide, Sides targetSide)
        {
            var tileTransform = tile.transform;
            tileTransform.position = Vector3.zero;
            tileTransform.rotation = Quaternion.identity;
        
            //rotate tile so that both sides are facing each other
            Vector3 targetSideEuler = targetSide.transform.eulerAngles;
            Vector3 tileSideEuler = tileSide.transform.eulerAngles;

            float deltaAngle = Mathf.DeltaAngle(tileSideEuler.y, targetSideEuler.y);
            Quaternion currentTileTargetRotation = Quaternion.AngleAxis(deltaAngle, Vector3.up);
            tileTransform.rotation = currentTileTargetRotation * Quaternion.Euler(0.0f, 180.0f, 0.0f);

            Vector3 tilePositionOffset = tileSide.transform.position - tileTransform.position;
            tileTransform.position = targetSide.transform.position - tilePositionOffset;
        }

        private bool CheckTileOverlap(Tiles.Tiles tile)
        {
            Bounds bounds = tile.TileBound;
            bounds.center = tile.transform.position + Vector3.up;
            bounds.Expand(-0.1f);
        
            Collider[] colliders = Physics.OverlapBox(bounds.center, bounds.size / 2, tile.transform.rotation, _roomLayerMask);

            //return true if no collision was made
            return colliders.Length > 0 && colliders.Any(c => !c.transform.parent.gameObject.Equals(tile.gameObject));
        }

        public void RegenerateWorld()
        {
            ResetLevelGenerator();
        }
    
    }
}





























