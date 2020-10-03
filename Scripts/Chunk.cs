using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
public class Chunk : MonoBehaviour
{
    public int ChunkX { get; set; }
    public int ChunkZ { get; set; }

    private static int seed;
    private static float frequency;
    private static float scale;

    private static int chunkWidth;
    private static int chunkHeight;

    public Block[,,] blocks;

    MeshRenderer meshRenderer;
    MeshCollider meshCollider;
    MeshFilter meshFilter;

    private void Awake()
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

    }
    public void PopulateChunk()
    {
        for (int y = 0; y < chunkHeight; y++)
        {
            for (int z = 0; z < chunkWidth; z++)
            {
                for (int x = 0; x < chunkWidth; x++)
                {
                    Material mat = Material.Air;
                    float noiseValue = CalculateNoiseValue(new Vector3(x, y, z), new Vector2(ChunkX * chunkWidth, ChunkZ * chunkWidth));

                    if (noiseValue > 0.2f)
                        mat = Material.Grass;
                    if (y == 0)
                        mat = Material.Grass;
                    if (mat == Material.Grass)
                    {
                        if (y > 0)
                            blocks[x, y - 1, z].material = Material.Dirt;
                    }

                    blocks[x, y, z] = new Block(mat, (int) transform.position.x, y, (int) transform.position.z);
                }
            }
        }
    }
    public void CalculateMesh()
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
                    // Up Face
                    if (IsAir(new Vector3Int(x, y + 1, z)))
                        BuildFace(block.material, BlockFace.up, new Vector3(x, y, z), Vector3.forward, Vector3.right, false, vertices, uvs, indices);
                    // Down Face
                    if (IsAir(new Vector3Int(x, y - 1, z)))
                        BuildFace(block.material, BlockFace.down, new Vector3(x, y - 1, z), Vector3.forward, Vector3.right, true, vertices, uvs, indices);
                    // Left Face
                    if (IsAir(new Vector3Int(x - 1, y, z)))
                        BuildFace(block.material, BlockFace.left, new Vector3(x, y - 1, z), Vector3.up, Vector3.forward, true, vertices, uvs, indices);
                    // Right Face
                    if (IsAir(new Vector3Int(x + 1, y, z)))
                        BuildFace(block.material, BlockFace.right, new Vector3(x + 1, y - 1, z), Vector3.up, Vector3.forward, false, vertices, uvs, indices);
                    // Back Face
                    if (IsAir(new Vector3Int(x, y, z - 1)))
                        BuildFace(block.material, BlockFace.back, new Vector3(x, y - 1, z), Vector3.up, Vector3.right, false, vertices, uvs, indices);
                    // Front Face
                    if (IsAir(new Vector3Int(x, y, z + 1)))
                        BuildFace(block.material, BlockFace.front, new Vector3(x, y - 1, z + 1), Vector3.up, Vector3.right, true, vertices, uvs, indices);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
    private bool IsInArrayBounds(Vector3Int position)
    {
        return !(position.x < 0 || position.x >= chunkWidth || position.y < 0 || position.y >= chunkHeight || position.z < 0 || position.z >= chunkWidth);
    }
    private BlockFace GetFaceFromDirection(Vector3 position)
    {
        if (position.x < 0)
            return BlockFace.left;
        else if (position.x >= chunkWidth)
            return BlockFace.right;
        if (position.z < 0)
            return BlockFace.back;
        else if (position.z >= chunkWidth)
            return BlockFace.front;
        return BlockFace.down;
    }
    public void RecalculateMesh()
    {
        if(meshFilter.mesh != null)
            meshFilter.mesh.Clear();
        CalculateMesh();
    }
    public Vector3 LocalToWorld(Vector3Int position)
    {
        float newX = ChunkX * chunkWidth + position.x;
        float newY = position.y;
        float newZ = ChunkZ * chunkWidth + position.z;

        return new Vector3(newX, newY, newZ);
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
    public Vector2Int GetNeighbourChunkCoords(BlockFace face)
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
        return new Vector2Int(chunkX, chunkZ);
    }
    public Block GetBlock(Vector3Int position)
    {
        if (!IsInArrayBounds(position))
            return null;
        return blocks[position.x,position.y,position.z];
    }
    public bool BreakBlock(Block block)
    {
        SoundManager.instance.PlayAudio(GetBlockSound(ActionType.Dig, block.material));
        block.material = Material.Air;
        RecalculateMesh();
        return true;
    }
    public bool BreakBlock(Vector3Int position)
    {
        if (!IsInArrayBounds(position))
            return false;
        return BreakBlock(GetBlock(position));
    }
    public bool PlaceBlock(Block block, Material material)
    {
        SoundManager.instance.PlayAudio(GetBlockSound(ActionType.Place, material));
        block.material = material;
        RecalculateMesh();
        return true;
    }
    public bool PlaceBlock(Vector3Int position, Material material)
    {
        if (!IsInArrayBounds(position))
            return false;
        return PlaceBlock(GetBlock(position), material);
    }
    public bool IsAir(Vector3Int position)
    {
        if (position.y < 0) return false;
        if (position.y >= chunkHeight) return true;

        if (!IsInArrayBounds(position))
        {
            Vector2Int coords = GetNeighbourChunkCoords(GetFaceFromDirection(position));
            Vector3 localBlockPosition = WorldToLocal(LocalToWorld(position), coords.x, coords.y);
            return GetTheoreticalBlockMaterial(localBlockPosition, coords) == Material.Air;
        }
        return GetBlock(position).IsAir();
    }
    public string GetBlockSound(ActionType action, Material mat)
    {
        switch (action)
        {
            case ActionType.Dig:
                {
                    switch (mat)
                    {
                        case Material.Dirt:
                        case Material.Grass:
                            {
                                return "dig_grass";
                            }
                        default: return null;
                    }
                }
            case ActionType.Place:
                {
                    switch (mat)
                    {
                        case Material.Dirt:
                        case Material.Grass:
                            {
                                return "place_grass";
                            }
                        default: return null;
                    }
                }
            default: return null;
        }
    }
    public static Vector3 LocalToWorld(Vector3Int position, int ChunkX, int ChunkZ)
    {
        float newX = ChunkX * chunkWidth + position.x;
        float newY = position.y;
        float newZ = ChunkZ * chunkWidth + position.z;

        return new Vector3(newX, newY, newZ);
    }
    public static Vector3Int WorldToLocal(Vector3 position, int ChunkX, int ChunkZ)
    {
        int x = (int)(position.x - ChunkX * chunkWidth);
        int y = (int)position.y;
        int z = (int)(position.z - ChunkZ * chunkWidth);
        return new Vector3Int(x, y, z);
    }
    public static Material GetTheoreticalBlockMaterial(Vector3 pos, Vector2Int chunkCoords)
    {
        Vector2 offset = new Vector2(chunkCoords.x * chunkWidth, chunkCoords.y * chunkWidth);
        float noiseValue = CalculateNoiseValue(pos, offset);
        Material mat = Material.Air;
        if (noiseValue > 0.2f)
            mat = Material.Grass;
        if (pos.y == 0)
            mat = Material.Grass;
        return mat;
    }
    private static float CalculateNoiseValue(Vector3 pos, Vector2 offset)
    {
        float noiseX = Mathf.Abs((pos.x + offset.x) / 20f * frequency) + seed;
        float noiseY = Mathf.Abs(pos.y / 20f * frequency) + seed;
        float noiseZ = Mathf.Abs((pos.z + offset.y) / 20f * frequency) + seed;

        float noiseValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);
        noiseValue += (10 - pos.y) / 10f * scale;
        return noiseValue;
    }
    private static float GetWidth(int totalItemCount, float eachItemWidth)
    {
        return eachItemWidth / (totalItemCount * eachItemWidth);
    }
    private static Vector2 GetCornerFromMaterial(Material material, BlockFace face)
    {

        switch (material)
        {
            case Material.Dirt: return new Vector2(32 / 48f, 0);
            case Material.Grass:
                {
                    if (face == BlockFace.up)
                        return new Vector2(0, 0);
                    if (face == BlockFace.down)
                        return new Vector2(32 / 48f, 0);
                    return new Vector2(16 / 48f, 0);
                }
            default: return Vector2.zero;
        }

    }
    private static void BuildFace(Material material, BlockFace face, Vector3 corner, Vector3 up, Vector3 right, bool reversed, List<Vector3> verts, List<Vector2> uvs, List<int> indices)
    {
        int index = verts.Count;

        verts.Add(corner); // Bottom Left
        verts.Add(corner + up); // Top Left
        verts.Add(corner + up + right); //Top Right
        verts.Add(corner + right); // Bottom Right

        Vector2 uvWidth = new Vector2(GetWidth(3, 16), 1);
        Vector2 uvCorner = GetCornerFromMaterial(material, face);

        uvs.Add(new Vector2(uvCorner.x, uvCorner.y));
        uvs.Add(new Vector2(uvCorner.x, uvCorner.y + uvWidth.y));
        uvs.Add(new Vector2(uvCorner.x + uvWidth.x, uvCorner.y + uvWidth.y));
        uvs.Add(new Vector2(uvCorner.x + uvWidth.x, uvCorner.y));

        if (!reversed)
        {
            indices.Add(index + 0);
            indices.Add(index + 1);
            indices.Add(index + 3);
            indices.Add(index + 1);
            indices.Add(index + 2);
            indices.Add(index + 3);
        }
        else
        {
            indices.Add(index + 0);
            indices.Add(index + 3);
            indices.Add(index + 1);
            indices.Add(index + 1);
            indices.Add(index + 3);
            indices.Add(index + 2);
        }

    }

}


