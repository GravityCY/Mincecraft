using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public float frequency;
    public float scale;

    public int seed;
    public int chunkHeight;
    public int chunkWidth;

    public Material defMat;

    public static TerrainManager instance;   

    [SerializeField] private int viewDistance = 4;
    [SerializeField] private Transform playerTransform;
    public List<Chunk> chunks { get; private set; } = new List<Chunk>();

    // Private

    private void Start()
    {
        if (instance == null){ instance = this; } 
        else if(instance != this)
        {
            Destroy(gameObject);
            return;
        }
        ;
    }

    private void FixedUpdate()
    {
        CreateChunksAroundPoint(playerTransform.position);
        UnloadChunksAroundPoint(playerTransform.position);
    }

    private void CreateChunksAroundPoint(Vector3 point)
    {
        int chunkX = ToChunkCoord(point.x);
        int chunkZ = ToChunkCoord(point.z);

        for (int x = chunkX - viewDistance; x <= chunkX + viewDistance; x++)
        {
            for(int z = chunkZ - viewDistance; z <= chunkZ + viewDistance; z++)
            {
                Chunk chunk = GetChunk(x, z);

                if (chunk == null)
                    CreateChunk(x, z);
                else if (!chunk.gameObject.activeSelf)
                    chunk.gameObject.SetActive(true);
            }
        }
    }

    private void UnloadChunksAroundPoint(Vector3 point)
    {
        int chunkX = ToChunkCoord(point.x);
        int chunkZ = ToChunkCoord(point.z);

        for (int i = 0; i < chunks.Count; i++)
        {
            Chunk chunk = chunks[i];
            if (!chunk.enabled)
                continue;

            if (Mathf.Abs(chunk.ChunkX - chunkX) >= viewDistance + 1 || Mathf.Abs(chunk.ChunkZ - chunkZ) >= viewDistance + 1)
                chunk.gameObject.SetActive(false);
        }
    }

    private void CreateChunk(int x, int z)
    {
        if (GetChunk(x, z) != null)
            return;

        GameObject chunkObject = new GameObject(x + "," + z);
        chunkObject.layer = LayerMask.NameToLayer("Ground");
        chunkObject.transform.position = new Vector3(x * chunkWidth, 0, z * chunkWidth);
        Chunk chunk = chunkObject.AddComponent<Chunk>();
        chunk.ChunkX = x;
        chunk.ChunkZ = z;
        chunks.Add(chunk);
    }

    private void UnloadChunk(int x, int z)
    {
        Chunk chunk = GetChunk(x, z);
        if(chunk != null)
            chunk.gameObject.SetActive(false);
    }

    private int ToChunkCoord(float point)
    {
        int chunkX = (int)(point / chunkWidth);
        if (point < 0)
            chunkX--;
        return chunkX;
    }

    public Chunk GetChunkWorldSpace(Vector3 point)
    {
        return GetChunk(ToChunkCoord(point.x), ToChunkCoord(point.z));
    }

    public Chunk GetChunk(float x, float z)
    {

        for (int i = 0; i < TerrainManager.instance.chunks.Count; i++)
        {
            Chunk chunk = TerrainManager.instance.chunks[i];
            if (chunk.ChunkX == (int)x && chunk.ChunkZ == (int)z)
                return chunk;
        }
        return null;
    }

    public Block GetBlock(Vector3 point)
    {
        Chunk chunk = GetChunkWorldSpace(point);
        if (chunk == null) return null;
        Vector3Int blockPosition = chunk.WorldToLocal(point);
        print(blockPosition);
        return chunk.GetBlock(blockPosition);
    }

    public bool BreakBlock(Vector3 point)
    {
        Chunk chunk = GetChunkWorldSpace(point);
        if (chunk == null) return false;
        return chunk.BreakBlock(chunk.WorldToLocal(point));
    }

    // Public

}
