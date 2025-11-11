using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private List<GameObject> tilePrefabs = new();

    [Header("Generation Settings")]
    [SerializeField] private int tilesAhead = 6;
    [SerializeField] private float initialZ = 0f;

    private class TileEntry
    {
        public GameObject go;
        public float length;
        public float startZ;
    }

    // Pooling would be more optimal in future but prototype
    private readonly Queue<TileEntry> activeTiles = new();
    private float nextSpawnZ = 0f;

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
            if (player.position.z > first.startZ + first.length)
            {
                RecycleTile();
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

        // Randomly pick a prefab
        var prefab = tilePrefabs[Random.Range(0, tilePrefabs.Count)];
        var go = Instantiate(prefab, new Vector3(0f, 0f, nextSpawnZ), Quaternion.identity, transform);

        float length = 20f;
        if (go.TryGetComponent<Tile>(out var tileComponent))
        {
            length = tileComponent.Length;
        }

        var entry = new TileEntry { go = go, length = length, startZ = nextSpawnZ };
        activeTiles.Enqueue(entry);

        nextSpawnZ += length;
    }

    private void RecycleTile()
    {
        var old = activeTiles.Dequeue();

        old.go.transform.position = new Vector3(0f, 0f, nextSpawnZ);
        old.go.transform.rotation = Quaternion.identity;

        float length = old.length;
        if (old.go.TryGetComponent<Tile>(out var tileComp))
        {
            length = tileComp.Length;
        }

        old.length = length;
        old.startZ = nextSpawnZ;

        activeTiles.Enqueue(old);
    }

#if UNITY_EDITOR
    // gizmo: preview spawned ranges
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        if (activeTiles != null)
        {
            foreach (var t in activeTiles)
            {
                if (t?.go == null) continue;
                Gizmos.DrawWireCube(new Vector3(0f, 0.1f, t.startZ + t.length * 0.5f), new Vector3(10f, 0.2f, t.length));
            }
        }
    }
#endif
}
