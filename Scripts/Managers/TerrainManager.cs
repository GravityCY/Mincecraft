using System;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{

    public static TerrainManager instance;

    #region Serialized Fields

    [Header("Noise")]
    [Tooltip("Determines the frequency of the noise")]
    [Range(0, 1)]
    public float frequency;
    [Tooltip("Determines the scale of the noise")]
    [Range(0, 2)]
    public float scale;
    [Tooltip("A seed will output different noise results")]
    public int seed;

    [Header("Chunks")]
    [Tooltip("The amount of blocks to populate a chunk with vertically")]
    public int chunkHeight;
    [Tooltip("The amount of blocks to populate a chunk with horizontally")]
    public int chunkWidth;
    [Header("Material")]
    [Tooltip("The default material of a chunks mesh")]
    public UnityEngine.Material defMat;
    [Tooltip("The amount of items in the texture on the X axis")]
    public int itemCountX;
    [Tooltip("The amount of items in the texture on the Y axis")]
    public int itemCountY;

    [Header("Player")]
    [Tooltip("Distance of the chunks rendering around the player")]
    public int viewDistance = 4;
    [Space]
    public Transform playerTransform;

    #endregion

    private int playerPreviousChunkX = int.MinValue;
    private int playerPreviousChunkZ = int.MinValue;

    [NonSerialized]
    public Texture mainTexture;
    public List<Chunk> chunks { get; private set; } = new List<Chunk>();

    #region Public Methods

    public int ToChunkCoord(float point)
    {
        int chunkX = (int)(point / chunkWidth);
        if (point < 0)
            chunkX--;
        return chunkX;
    }
    public Chunk GetChunk(Vector3 point)
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
        Chunk chunk = GetChunk(point);
        if (chunk == null) return null;
        Vector3Int blockPosition = chunk.WorldToLocal(point);
        return chunk.GetBlock(blockPosition);
    }
    public bool BreakBlock(Vector3 point)
    {
        Chunk chunk = GetChunk(point);
        if (chunk == null) return false;
        return chunk.BreakBlock(chunk.WorldToLocal(point));
    }
    public bool PlaceBlock(Vector3 point, Material type)
    {
        Chunk chunk = GetChunk(point);
        if (chunk == null) return false;
        return chunk.PlaceBlock(chunk.WorldToLocal(point), type);
    }

    #endregion

    #region Private Methods

    private void Awake()
    {
        if (instance == null){ instance = this; } 
        else if(instance != this)
        {
            Destroy(gameObject);
            return;
        }
        mainTexture = defMat.GetTexture("_MainTex");
    }
    private void FixedUpdate()
    {
        int chunkX = ToChunkCoord(playerTransform.position.x);
        int chunkZ = ToChunkCoord(playerTransform.position.z);

        if (playerPreviousChunkX == chunkX && playerPreviousChunkZ == chunkZ) return;

        CreateChunksAroundPoint(playerTransform.position);
        UnloadChunksAroundPoint(playerTransform.position);

        playerPreviousChunkX = chunkX;
        playerPreviousChunkZ = chunkZ;
    }
    private void CreateChunksAroundPoint(Vector3 point)
    {
        int chunkX = ToChunkCoord(point.x);
        int chunkZ = ToChunkCoord(point.z);

        List<Chunk> localChunks = new List<Chunk>();
        
        for (int x = chunkX - viewDistance; x <= chunkX + viewDistance; x++)
        {
            for(int z = chunkZ - viewDistance; z <= chunkZ + viewDistance; z++)
            {
                Chunk chunk = CreateChunk(x, z);               
                if (!chunk.gameObject.activeSelf)
                    chunk.gameObject.SetActive(true);
                localChunks.Add(chunk);
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
    private Chunk CreateChunk(int x, int z)
    {
        Chunk exists = GetChunk(x, z);
        if (exists != null)
            return exists;

        GameObject chunkObject = new GameObject(x + ":" + z);
        chunkObject.layer = LayerMask.NameToLayer("Ground");
        chunkObject.transform.position = new Vector3(x * chunkWidth, 0, z * chunkWidth);
        Chunk chunk = chunkObject.AddComponent<Chunk>();
        chunk.ChunkX = x;
        chunk.ChunkZ = z;
        chunk.PopulateChunk();
        chunk.CalculateMesh();
        chunks.Add(chunk);
        return chunk;
    }
    private void UnloadChunk(int x, int z)
    {
        Chunk chunk = GetChunk(x, z);
        if(chunk != null)
            chunk.gameObject.SetActive(false);
    }

    #endregion
}
