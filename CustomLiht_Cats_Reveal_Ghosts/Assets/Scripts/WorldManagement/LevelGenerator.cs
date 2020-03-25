using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AI;
// using UnityEditor.AI;
// using UnityEditor;

public class LevelGenerator : MonoBehaviour
{
    public Tiles StartTilePrefab, EndTilePrefab;
    public List<Tiles> TilesPrefabs = new List<Tiles>();
    public Vector2Int IterationRange = new Vector2Int(5, 10);

    private Tiles startTile, endTile;
    private List<Tiles> placedTiles = new List<Tiles>();
    private List<Sides> availableSides = new List<Sides>();

    private LayerMask roomLayerMask;

    private void Start()
    {
        roomLayerMask = LayerMask.GetMask("Tile");
        StartCoroutine(nameof(GenerateLevel));
    }
    
    IEnumerator GenerateLevel()
    {
        WaitForSeconds startup = new WaitForSeconds(1);
        WaitForFixedUpdate interval = new WaitForFixedUpdate();

        yield return startup;
        
        //place start tile
        PlaceStartTile();
        yield return interval;

        //place some tiles
        int iterations = Random.Range(IterationRange.x, IterationRange.y);

        for (int i = 0; i < iterations; ++i)
        {
            PlaceRandomTile();
            yield return interval;
        }

        //place end tile
        PlaceEndTile();
        yield return interval;
        
        //update navmesh
        NavMeshBuilder.BuildNavMesh();
        
        
        //Spawn every entity
            //now it is time to start the entity spawn
            GetComponent<EntitySpawn>().SpawnEntity();
        //-----------------
            
        //Generator is finished
        StopCoroutine(nameof(GenerateLevel));
    }

    private void PlaceStartTile()
    {
        Debug.Log("Placing Start Tile");

        startTile = Instantiate(StartTilePrefab, transform);
        
        //Add all sides to the side list
        AddSideToList(startTile, ref availableSides);
        startTile.transform.position = Vector3.zero;
        startTile.transform.rotation = Quaternion.identity;
        
        Debug.Log("Start Tile was placed");
    }

    private void PlaceRandomTile()
    {
        Debug.Log("Placing Random Tile");

        Tiles currentTile = Instantiate(TilesPrefabs[Random.Range(0, TilesPrefabs.Count)], transform);

        AddSideToList(currentTile, ref availableSides);

        if (!PlaceTileInWorld(currentTile))
        {
            Destroy(currentTile.gameObject);
            ResetLevelGenerator();
        }
        
        Debug.Log("Tile was placed");
    }

    private void PlaceEndTile()
    {
        Debug.Log("Placing End Tile");

        endTile = Instantiate(EndTilePrefab, transform);

        if (!PlaceTileInWorld(endTile))
            ResetLevelGenerator();
        
        Debug.Log("End Tile was placed");
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
        Debug.LogError("Reset Level Generator");
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
        tile.transform.position = Vector3.zero;
        tile.transform.rotation = Quaternion.identity;
        
        //rotate tile so that both sides are facing each other
        Vector3 targetSideEuler = targetSide.transform.eulerAngles;
        Vector3 tileSideEuler = tileSide.transform.eulerAngles;

        float deltaAngle = Mathf.DeltaAngle(tileSideEuler.y, targetSideEuler.y);
        Quaternion currentTileTargetRotation = Quaternion.AngleAxis(deltaAngle, Vector3.up);
        tile.transform.rotation = currentTileTargetRotation * Quaternion.Euler(0.0f, 180.0f, 0.0f);

        Vector3 tilePositionOffset = tileSide.transform.position - tile.transform.position;
        tile.transform.position = targetSide.transform.position - tilePositionOffset;
    }

    private bool CheckTileOverlap(Tiles tile)
    {
        Bounds bounds = tile.TileBound;
        bounds.center = tile.transform.position + Vector3.up;
        bounds.Expand(-0.5f);
        
        Collider[] colliders = Physics.OverlapBox(bounds.center, bounds.size / 2, tile.transform.rotation, roomLayerMask);

        if (colliders.Length > 0)
        {
            //ignore collider with current tile
            foreach (Collider c in colliders)
            {
                if (c.transform.parent.gameObject.Equals(tile.gameObject))
                {
                    continue;
                }
                else
                {
                    Debug.Log("There was a collision");
                    return true;
                }
            }
        }

        return false;
    }
    
}





























