using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Tiles StartTilePrefab, EndTilePrefab;
    public List<Tiles> TilesPrefabs = new List<Tiles>();
    public Vector2Int IterationRange = new Vector2Int(5, 10);
    
    private List<Sides> availableSides = new List<Sides>();
    private StartTile startTile;
    private EndTile endTile;
    private List<Tiles> placedTiles = new List<Tiles>();

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
            PlaceTile();
            yield return interval;
        }

        //place end tile
        PlaceEndTile();
        yield return interval;
        
        //Generator is finished
        StopCoroutine(nameof(GenerateLevel));
    }

    private void PlaceStartTile()
    {
        Debug.Log("Place Start Tile");

        startTile = Instantiate(StartTilePrefab, transform) as StartTile;
        
        //Add all sides to the side list
        AddSideToList(startTile, ref availableSides);
        startTile.transform.position = Vector3.zero;
        startTile.transform.rotation = Quaternion.identity;

    }

    private void PlaceTile()
    {
        Debug.Log("Place Random Tile");

        Tiles currentTile = Instantiate(TilesPrefabs[Random.Range(0, TilesPrefabs.Count)], transform) as Tiles;

        
        List<Sides> allAvailableSides = new List<Sides>(availableSides);
        List<Sides> currentSides = new List<Sides>();
        AddSideToList(currentTile, ref currentSides);
        
        //Get sides from current room and add them to the list of available sides
        AddSideToList(currentTile, ref availableSides);

        bool tilePlaced = false;

        //try all available side
        foreach (Sides availableSide in allAvailableSides)
        {
            //try all available side in current room
            foreach (Sides currentSide in currentSides)
            {
                //position the tile
                PositionTileAtSide(ref currentTile, currentSide, availableSide);

                if (CheckTileOverlap(currentTile))
                {
                    continue;
                }

                tilePlaced = true;
                
                placedTiles.Add(currentTile);
                //remove sides
                currentSide.gameObject.SetActive(false);
                availableSides.Remove(currentSide);
                
                availableSide.gameObject.SetActive(false);
                availableSides.Remove(availableSide);
                
                //exit the loop
                break;
            }
            //exit loop
            if (tilePlaced)
            {
                break;
            }
        }
        //Should be reset if room was not placed
        // ResetLevelGenerator();
        if (!tilePlaced)
        {
            Destroy(currentTile.gameObject);
            ResetLevelGenerator();
        }
    }

    private void PlaceEndTile()
    {
        Debug.Log("Place End Tile");
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
        
        // Debug.DrawRay(bounds.center, (tile.transform.rotation * bounds.size / 2), Color.blue, 1000.0f);
        
        Collider[] colliders =
            Physics.OverlapBox(bounds.center, bounds.size / 2, tile.transform.rotation, roomLayerMask);

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





























