using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawn : MonoBehaviour
{
    [SerializeField] private Vector2Int ghostNumberRange;
    [SerializeField] private Vector2Int catNumberRange;

    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private List<GameObject> worldTiles;

    [SerializeField] private int NumberGhost;
    private int NumberCats;
    
    // Start is called before the first frame update
    void Start()
    {
        if (ghostNumberRange.x >= worldTiles.Count || ghostNumberRange.y >= worldTiles.Count || ghostNumberRange.x < 0 || ghostNumberRange.x < 0)
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

        NumberGhost = Random.Range(ghostNumberRange.x, ghostNumberRange.y + 1);

        for (int i = 0; i < NumberGhost; i++)
        {
            int rng = Random.Range(0, worldTiles.Count);

            var tr = worldTiles[rng].transform.Find("GhostSpawnPoint");
            Instantiate(ghostPrefab, tr);
            
            worldTiles.RemoveAt(rng);
        }
    }
}
