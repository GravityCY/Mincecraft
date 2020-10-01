using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit result;
            if (Physics.Raycast(ray, out result, 5, LayerMask.GetMask("Ground")))
            {
                BreakBlock(result);
            }   
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit result;
            if (Physics.Raycast(ray, out result, 5, LayerMask.GetMask("Ground")))
            {
                PlaceBlock(result);
            }
        }
    }

    private void BreakBlock(RaycastHit result)
    {
        Chunk chunk = result.collider.GetComponent<Chunk>();
        if (chunk == null) return;

        BlockFace face = BlockFaceUtils.GetBlockFaceFromNormal(result.normal);
        Vector3 hitPoint = BlockFaceUtils.Normalize(result.point, face);
        hitPoint.y += 1;
        TerrainManager.instance.BreakBlock(hitPoint);

        float hitXF = hitPoint.x;
        float hitZF = hitPoint.z;
        int hitXI = Mathf.Abs((int)hitXF);
        int hitZI = Mathf.Abs((int)hitZF);

        if (hitXI % TerrainManager.instance.chunkWidth == 0)
        {
            BlockFace towards = BlockFace.left;
            if (hitXF < 0)
                towards = BlockFace.right;
            Chunk neighbourChunk = chunk.GetNeighbourChunk(towards);
            if (neighbourChunk != null)
            {
                neighbourChunk.RecalculateMesh();
            }
        }
        else if (hitXI % TerrainManager.instance.chunkWidth == TerrainManager.instance.chunkWidth - 1)
        {
            BlockFace towards = BlockFace.right;
            if (hitXF < 0)
                towards = BlockFace.left;
            Chunk neighbourChunk = chunk.GetNeighbourChunk(towards);
            if (neighbourChunk != null)
            {
                neighbourChunk.RecalculateMesh();
            }
        }
        if (hitZI % TerrainManager.instance.chunkWidth == 0)
        {
            BlockFace towards = BlockFace.back;
            if (hitZF < 0)
                towards = BlockFace.front;
            Chunk neighbourChunk = chunk.GetNeighbourChunk(towards);
            if (neighbourChunk != null)
            {
                neighbourChunk.RecalculateMesh();
            }
        }
        else if (hitZI % TerrainManager.instance.chunkWidth == TerrainManager.instance.chunkWidth - 1)
        {
            BlockFace towards = BlockFace.front;
            if (hitZF < 0)
                towards = BlockFace.back;
            Chunk neighbourChunk = chunk.GetNeighbourChunk(towards);
            if (neighbourChunk != null)
            {
                neighbourChunk.RecalculateMesh();
            }
        }        
    }
    private void PlaceBlock(RaycastHit result)
    {
        Chunk chunk = result.collider.GetComponent<Chunk>();
        if (chunk == null) return;

        BlockFace face = BlockFaceUtils.GetBlockFaceFromNormal(result.normal);
        Vector3 hitPoint = BlockFaceUtils.NormalizeOpposite(result.point, face);
        hitPoint.y += 1;

        if ((int) hitPoint.x == (int) transform.position.x && (int) hitPoint.y == (int) (transform.position.y + 0.15f) && (int) hitPoint.z == (int) transform.position.z)
            return;
        if ((int) hitPoint.x == (int) transform.position.x && (int) hitPoint.y == (int) (transform.position.y + transform.lossyScale.y / 2f + 0.2f) && (int) hitPoint.z == (int) transform.position.z)
            return;

        TerrainManager.instance.PlaceBlock(hitPoint, Material.Grass);
        chunk.RecalculateMesh();
    }
}
