using Assets.Classes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
public class Chunk : MonoBehaviour
{
    public int ChunkX { get; set; }
    public int ChunkZ { get; set; }

    private int seed;
    private float frequency;
    private float scale;

    private int chunkWidth;
    private int chunkHeight;

    public Block[,,] blocks;

    MeshRenderer meshRenderer;
    MeshCollider meshCollider;
    MeshFilter meshFilter;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();

        chunkWidth = TerrainManager.instance.chunkWidth;
        chunkHeight = TerrainManager.instance.chunkHeight;
        seed = TerrainManager.instance.seed;
        frequency = TerrainManager.instance.frequency;
        scale = TerrainManager.instance.scale;

        blocks = new Block[chunkWidth, chunkHeight, chunkWidth];

        meshRenderer.material = TerrainManager.instance.defMat;

        PopulateChunk();
    }

    private void PopulateChunk()
    {
        for (int y = 0; y < chunkHeight; y++)
        {
            float noiseY = Mathf.Abs(y / 20f * frequency) + seed;
            for (int z = 0; z < chunkWidth; z++)
            {
                float noiseZ = Mathf.Abs((z + transform.position.z) / 20f * frequency) + seed;
                for (int x = 0; x < chunkWidth; x++)
                {
                    float noiseX = Mathf.Abs((x + transform.position.x) / 20f * frequency) + seed;
                    float noiseValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);
                    BlockType type = BlockType.Air;

                    noiseValue += (10-y)/10f;

                    if (noiseValue > 0.2f)
                        type = BlockType.Ground;

                    if (y == 0)
                        type = BlockType.Ground;

                    blocks[x, y, z] = new Block(type, ChunkX * chunkWidth + x, y, ChunkZ * chunkWidth + z);

                }
            }
        }
        CalculateMesh();
    }

    private void CalculateMesh()
    {

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> indices = new List<int>();
        
        for (int y = 0; y < chunkHeight; y++)
        {
            for (int z = 0; z < chunkWidth; z++) 
            {
                for (int x = 0; x < chunkWidth; x++)
                {
                    Block block = blocks[x, y, z];
                    if (block.IsAir())
                        continue;
                    // Top Face
                    if (IsAir(new Vector3Int(x, y + 1, z)))
                        BuildFace(block.type, new Vector3(x, y, z), Vector3.forward, Vector3.right, false, vertices, uvs, indices);
                    // Bottom Face
                    if (IsAir(new Vector3Int(x, y - 1, z)))
                        BuildFace(block.type, new Vector3(x, y - 1, z), Vector3.forward, Vector3.right, true, vertices, uvs, indices);
                    // Left Face
                    if (IsAir(new Vector3Int(x - 1, y, z)))
                        BuildFace(block.type, new Vector3(x, y - 1, z), Vector3.up, Vector3.forward, true, vertices, uvs, indices);
                    // Right Face
                    if (IsAir(new Vector3Int(x + 1, y, z)))
                        BuildFace(block.type, new Vector3(x + 1, y - 1, z), Vector3.up, Vector3.forward, false, vertices, uvs, indices);
                    // Back Face
                    if (IsAir(new Vector3Int(x, y, z - 1)))
                        BuildFace(block.type, new Vector3(x, y - 1, z), Vector3.up, Vector3.right, false, vertices, uvs, indices);
                    // Front Face
                    if (IsAir(new Vector3Int(x, y, z + 1)))
                        BuildFace(block.type, new Vector3(x, y - 1, z + 1), Vector3.up, Vector3.right, true, vertices, uvs, indices);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshFilter.mesh.Clear();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    private void BuildFace(BlockType type, Vector3 corner, Vector3 up, Vector3 right, bool reversed, List<Vector3> verts, List<Vector2> uvs, List<int> indices)
    {
        int index = verts.Count;

        verts.Add(corner); // Bottom Left
        verts.Add(corner + up); // Top Left
        verts.Add(corner + right); // Bottom Right
        verts.Add(corner + up + right); // Top Right

        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(1, 1));

        if (!reversed)
        {
            indices.Add(index + 0);
            indices.Add(index + 1);
            indices.Add(index + 2);
            indices.Add(index + 2);
            indices.Add(index + 1);
            indices.Add(index + 3);
        }
        else
        {
            indices.Add(index + 0);
            indices.Add(index + 2);
            indices.Add(index + 1);
            indices.Add(index + 1);
            indices.Add(index + 2);
            indices.Add(index + 3);
        }

    }

    private bool IsInArrayBounds(Vector3Int position)
    {
        return !(position.x < 0 || position.x >= chunkWidth || position.y < 0 || position.y >= chunkHeight || position.z < 0 || position.z >= chunkWidth);
    }

    public void RecalculateMesh()
    {
        CalculateMesh();
    }

    public Vector3Int LocalToWorld(Vector3Int position)
    {
        int newX = ChunkX * chunkWidth + position.x - 1;
        int newY = position.y;
        int newZ = ChunkZ * chunkWidth + position.z - 1;

        return new Vector3Int(newX, newY, newZ);
    }

    public Vector3Int WorldToLocal(Vector3 position)
    {
        int x = (int) (position.x - ChunkX * chunkWidth);
        int y = (int)  position.y;
        int z = (int) (position.z - ChunkZ * chunkWidth);
        return new Vector3Int(x, y, z);
    }

    public Chunk GetNeighbourChunk(BlockFace face)
    {
        int chunkX = this.ChunkX;
        int chunkZ = this.ChunkZ;

        if (face == BlockFace.left)
            chunkX--;
        else if (face == BlockFace.right)
            chunkX++;
        if (face == BlockFace.front)
            chunkZ++;
        else if (face == BlockFace.back)
            chunkZ--;       

        return TerrainManager.instance.GetChunk(chunkX, chunkZ);
    }

    public Block GetBlock(Vector3Int position)
    {
        if (!IsInArrayBounds(position))
            return null;
        return blocks[position.x,position.y,position.z];
    }

    public bool BreakBlock(Block block)
    {
        switch (block.type) 
        {
            case BlockType.Ground: break;
        }

        block.type = BlockType.Air;
        RecalculateMesh();
        return true;
    }

    public bool BreakBlock(Vector3Int position)
    {
        if (!IsInArrayBounds(position))
            return false;
        return BreakBlock(blocks[position.x, position.y, position.z]);
    }

    public bool IsAir(Vector3Int position)
    {
        if (!IsInArrayBounds(position))
            return true;
        return GetBlock(position).IsAir();
    }

}


