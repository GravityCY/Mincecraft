using UnityEngine;

public enum BlockFace
{
    up, down, left, right, front, back
}

public class BlockFaceUtils
{
    public static BlockFace GetBlockFaceFromNormal(Vector3 normal)
    {
        if (normal.x == 1)
            return BlockFace.right;
        else if (normal.x == -1)
            return BlockFace.left;
        else if (normal.y == 1)
            return BlockFace.up;
        else if (normal.y == -1)
            return BlockFace.down;
        else if (normal.z == 1)
            return BlockFace.front;
        else if (normal.z == -1)
            return BlockFace.back;

        return BlockFace.up;
    }

    public static Vector3 Normalize(Vector3 point, BlockFace face)
    {
        switch (face)
        {
            case BlockFace.up: return new Vector3(point.x,point.y - 0.1f,point.z);
            case BlockFace.down: return new Vector3(point.x, point.y + 0.1f, point.z);
            case BlockFace.right: return new Vector3(point.x - 0.1f, point.y, point.z);
            case BlockFace.left: return new Vector3(point.x + 0.1f, point.y, point.z);
            case BlockFace.front: return new Vector3(point.x, point.y, point.z - 0.1f);
            case BlockFace.back: return new Vector3(point.x, point.y, point.z + 0.1f);
            default: return Vector3.zero;
        }
    }

    public static Vector3 NormalizeOpposite(Vector3 point, BlockFace face)
    {
        switch (face)
        {
            case BlockFace.up: return new Vector3(point.x, point.y + 0.1f, point.z);
            case BlockFace.down: return new Vector3(point.x, point.y - 0.1f, point.z);
            case BlockFace.right: return new Vector3(point.x + 0.1f, point.y, point.z);
            case BlockFace.left: return new Vector3(point.x - 0.1f, point.y, point.z);
            case BlockFace.front: return new Vector3(point.x, point.y, point.z + 0.1f);
            case BlockFace.back: return new Vector3(point.x, point.y, point.z - 0.1f);
            default: return Vector3.zero;
        }
    }
}