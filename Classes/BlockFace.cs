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

    public static BlockFace GetFaceFromDirection(Vector3 start, Vector3 end)
    {
        if (start.x < end.x)
            return BlockFace.right;
        else if (start.x > end.x)
            return BlockFace.left;
        if (start.y < end.y)
            return BlockFace.up;
        else if (start.y > end.y)
            return BlockFace.down;
        if (start.z < end.z)
            return BlockFace.front;
        else if (start.z > end.z)
            return BlockFace.back;

        return BlockFace.down;
    }

    public static BlockFace GetFaceFromDirection(Vector2 start, Vector2 end)
    {
        float diffX = Mathf.Abs(start.x - end.x);
        float diffZ = Mathf.Abs(start.y - end.y);

        if(diffX > diffZ)
        {
            if (start.x < end.x)
                return BlockFace.right;
            else if (start.x > end.x)
                return BlockFace.left;
        } else
        {
            if (start.y < end.y)
                return BlockFace.front;
            else if (start.y > end.y)
                return BlockFace.back;
        }
        return BlockFace.down;
    }

    public static BlockFace GetOppositeFace(BlockFace face)
    {
        switch (face)
        {
            case BlockFace.up: return BlockFace.down;
            case BlockFace.down: return BlockFace.up;
            case BlockFace.right: return BlockFace.left;
            case BlockFace.left: return BlockFace.right;
            case BlockFace.front: return BlockFace.back;
            case BlockFace.back: return BlockFace.front;
            default: return BlockFace.up;
        }
    }
}