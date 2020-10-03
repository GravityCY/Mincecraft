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
		Chunk chunk = TerrainManager.instance.GetChunk(new Vector3(x, y, z));
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

	public Block GetNeighbourBlock(BlockFace towards)
    {
		int newX = x;
		int newY = y;
		int newZ = z;

		if (towards == BlockFace.left)
			newX--;
		else if (towards == BlockFace.right)
			newX++;
		else if (towards == BlockFace.front)
			newZ++;
		else if (towards == BlockFace.back)
			newZ--;
		else if (towards == BlockFace.up)
			newY++;
		else if (towards == BlockFace.down)
			newY--;
		return TerrainManager.instance.GetBlock(new Vector3(newX, newY, newZ));
    }

}
