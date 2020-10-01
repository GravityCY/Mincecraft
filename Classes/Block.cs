using Assets.Classes;
using UnityEngine;

public class Block
{
	public BlockType type;

	public int x, y, z;

	public Block(BlockType type, int x,int y, int z)
	{
		this.type = type;
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
		return type == BlockType.Air;
    }

	public void Break()
    {
		Chunk chunk = GetChunk();
		chunk.BreakBlock(new Vector3Int(x, y, z));
		chunk.RecalculateMesh();
	}

}
