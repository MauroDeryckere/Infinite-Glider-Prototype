using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private List<GameObject> tilePrefabs = new();

    [Header("Generation Settings")]
    [SerializeField] private int tilesAhead = 6;
    [SerializeField] private int tilesBehind = 2;
    [SerializeField] private float initialZ = 0f;
    [SerializeField] private float verticalStep = -10; // how much Y decreases per tile

    private class TileEntry
    {
        public GameObject go;
        public float length;
        public float startZ;
    }

    // Pooling would be more optimal in future but prototype
    private readonly Queue<TileEntry> activeTiles = new();
    private float nextSpawnZ = 0f;
    private float currentY = 0f;

    void Start()
    {
        if (player == null)
        {
            var found = GameObject.FindGameObjectWithTag("Player");
            if (found)
            {
                player = found.transform;
            }
            else
            {
                Debug.LogError("MapGenerator: No player transform assigned and no GameObject with tag 'Player' found.");
            }
        }

        nextSpawnZ = initialZ;
        currentY = 0f;


        for (int i = 0; i < tilesAhead; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        while (activeTiles.Count < tilesAhead)
        {
            SpawnTile();
        }

        if (activeTiles.Count > 0)
        {
            var first = activeTiles.Peek();
            if (player.position.z > first.startZ + first.length * (1 + tilesBehind))
            {
                RemoveOldTile();
            }
        }
    }

    private void SpawnTile()
    {
        if (tilePrefabs == null || tilePrefabs.Count == 0)
        {
            Debug.LogWarning("MapGenerator: no tilePrefabs assigned.");
            return;
        }

        int idx = Random.Range(0, tilePrefabs.Count);
        var prefab = tilePrefabs[idx];
        if (prefab == null)
        {
            Debug.LogWarning($"MapGenerator: tilePrefabs[{idx}] is null.");
            return;
        }

        Quaternion spawnRot = prefab.transform.rotation;
        Vector3 spawnPos = new Vector3(0f, currentY, nextSpawnZ);

        var go = Instantiate(prefab, spawnPos, spawnRot, transform);

        float length = 20f;
        if (go.TryGetComponent<Tile>(out var tileComponent))
        {
            length = tileComponent.Length;
        }

        var entry = new TileEntry { go = go, length = length, startZ = nextSpawnZ };
        activeTiles.Enqueue(entry);

        currentY += verticalStep; // lower slightly for each new tile
        nextSpawnZ += length;
    }

    private void RemoveOldTile()
    {
        var old = activeTiles.Dequeue();
        if (old.go != null)
        {
            Destroy(old.go);
        }
    }
}
