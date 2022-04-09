using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{
    static World instance;

    public Tile[] tiles;
    public Tilemap map;
    public Transform parent;

    public Vector2Int mapSize = new Vector2Int(50, 50);

    NavMeshSurface2d surface;

    Vector2Int[] neibors = {
        new Vector2Int( 1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int( 0, 1),
        new Vector2Int( 0,-1),
        new Vector2Int( 1, 1),
        new Vector2Int( 1,-1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1,-1),
    };

    readonly Dictionary<Vector2Int, Tilemap> tilemap = new Dictionary<Vector2Int, Tilemap>();

    private void Awake()
    {
        instance = this;
        surface = GetComponent<NavMeshSurface2d>();

    }

    private void Start()
    {
        tilemap.Add(Vector2Int.zero, map);

        GenerateMap(map);



        surface.BuildNavMeshAsync();
    }
    private void Update()
    {
        Vector2Int player = new Vector2Int((int)Viking.position.x, (int)Viking.position.y);

        player.x = player.x / mapSize.x;
        player.y = player.y / mapSize.y;

        bool r = false;

        foreach (var chunk in tilemap)
        {
            if (Vector2Int.Distance(player, chunk.Key) > 2.5f)
            {
                chunk.Value.gameObject.SetActive(false);
            }
        }

        Vector2Int p;
        
        foreach (var n in neibors)
        {
            p = player + n;
            if (tilemap.ContainsKey(p))
            {
                if (!tilemap[p].gameObject.activeInHierarchy)
                {
                    tilemap[p].gameObject.SetActive(true);
                    r = true;
                }
                continue;
            }
            CreateMap(p);
            r = true;
        }

        if (r)
            Rebuild();
    }

    void CreateMap(Vector2Int pos)
    {
        Tilemap m = Instantiate(map, parent);
        m.transform.position = new Vector3(pos.x * mapSize.x, pos.y * mapSize.y, 0);
        m.gameObject.name = string.Format("map({0}:{1})", pos.x, pos.y);
        m.gameObject.SetActive(true);
        tilemap.Add(pos, m);
        GenerateMap(m);
    }

    void GenerateMap(Tilemap m)
    {
        Vector3Int pos;
        for (int i = 0; i < mapSize.x; i++)
            for (int j = 0; j < mapSize.y; j++)
            {
                pos = new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0);
                int id = Random.Range(0, tiles.Length);

                AddTile(m, pos, id);
            }
    }

    void AddTile(Tilemap m, Vector3Int pos, int tileID)
    {
        m.SetTile(pos, tiles[tileID]);
    }

    public static void Rebuild()
    {
        instance.surface.BuildNavMeshAsync();
    }
}
