using UnityEngine;

public class Block
{
	public Material material;

	public int x, y, z;

	public Block(Material material, int x,int y, int z)
	{
		this.material = material;
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Chunk GetChunk()
    {
		Chunk chunk = TerrainManager.instance.GetChunkWorldSpace(new Vector3(x, y, z));
		return chunk;
    }

	public bool IsAir()
    {
		return material == Material.Air;
    }

	public void Break()
    {
		Chunk chunk = GetChunk();
		chunk.BreakBlock(new Vector3Int(x, y, z));
	}

}
