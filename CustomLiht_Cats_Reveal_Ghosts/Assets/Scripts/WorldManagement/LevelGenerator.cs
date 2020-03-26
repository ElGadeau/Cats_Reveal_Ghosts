using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
// using UnityEditor.AI;
// using UnityEditor;

public class LevelGenerator : MonoBehaviour
{
    public bool _isPlaying = false;
    
    public Tiles StartTilePrefab, EndTilePrefab;
    public List<Tiles> TilesPrefabs = new List<Tiles>();
    public Vector2Int IterationRange = new Vector2Int(5, 10);
    // public NavMeshSurface surface = null;


    private Tiles startTile, endTile;
    private List<Tiles> placedTiles = new List<Tiles>();
    private List<Sides> availableSides = new List<Sides>();

    private LayerMask roomLayerMask;

    private void Start()
    {
        if (_isPlaying)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        roomLayerMask = LayerMask.GetMask("Tile");
        StartCoroutine(nameof(GenerateLevel));
    }
    
    IEnumerator GenerateLevel()
    {
        WaitForSeconds startup = new WaitForSeconds(1);
        WaitForFixedUpdate interval = new WaitForFixedUpdate();

        yield return startup;
        
        PlaceStartTile();
        yield return interval;

        int iterations = Random.Range(IterationRange.x, IterationRange.y);
        for (int i = 0; i < iterations; ++i)
        {
            PlaceRandomTile();
            yield return interval;
        }

        PlaceEndTile();
        yield return interval;
        
        GetComponent<NavMeshSurface>().BuildNavMesh();
        
        GetComponent<EntitySpawn>().CanSpawn = true;
        GetComponent<EntitySpawn>().SpawnEntity();
        
        yield return null;
    }

    private void PlaceStartTile()
    {
        startTile = Instantiate(StartTilePrefab, transform);
        
        //Add all sides to the side list
        AddSideToList(startTile, ref availableSides);
        var tileTransform = startTile.transform;
        tileTransform.position = Vector3.zero;
        tileTransform.rotation = Quaternion.identity;
    }

    private void PlaceRandomTile()
    {
        Tiles currentTile = Instantiate(TilesPrefabs[Random.Range(0, TilesPrefabs.Count)], transform);

        AddSideToList(currentTile, ref availableSides);
        if (PlaceTileInWorld(currentTile))
            return;
        
        Destroy(currentTile.gameObject);
        ResetLevelGenerator();
    }

    private void PlaceEndTile()
    {
        endTile = Instantiate(EndTilePrefab, transform);

        if (!PlaceTileInWorld(endTile))
            ResetLevelGenerator();
    }

    private bool PlaceTileInWorld(Tiles p_tile)
    {
        List<Sides> currentSides = new List<Sides>();
        AddSideToList(p_tile, ref currentSides);
        
        foreach (Sides availableSide in availableSides)
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

                placedTiles.Add(p_tile);
                //remove sides
                currentSide.gameObject.SetActive(false);
                availableSides.Remove(currentSide);
                
                availableSide.gameObject.SetActive(false);
                availableSides.Remove(availableSide);

                return true;
            }
        }
        
        return false;
    }
    
    private void ResetLevelGenerator()
    {
        GetComponent<EntitySpawn>().CanSpawn = false;
        
        StopCoroutine(nameof(GenerateLevel));
        
        if (startTile)
            Destroy(startTile.gameObject);
        
        if (endTile)
            Destroy(endTile.gameObject);

        foreach (Tiles tile in placedTiles)
        {
            Destroy(tile.gameObject);
        }
        
        placedTiles.Clear();
        availableSides.Clear();

        StartCoroutine(nameof(GenerateLevel));
    }
    
    private void AddSideToList(Tiles tile, ref List<Sides> sides)
    {
        foreach (var side in tile.sides)
        {
            int r = Random.Range(0, sides.Count);
            sides.Insert(r, side);
        }
    }

    private void PositionTileAtSide(ref Tiles tile, Sides tileSide, Sides targetSide)
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

    private bool CheckTileOverlap(Tiles tile)
    {
        Bounds bounds = tile.TileBound;
        bounds.center = tile.transform.position + Vector3.up;
        bounds.Expand(-0.1f);
        
        Collider[] colliders = Physics.OverlapBox(bounds.center, bounds.size / 2, tile.transform.rotation, roomLayerMask);

        //return true if no collision was made
        return colliders.Length > 0 && colliders.Any(c => !c.transform.parent.gameObject.Equals(tile.gameObject));
    }

    public void RegenerateWorld()
    {
        ResetLevelGenerator();
    }
    
}





























