using UnityEngine;

public class Player : MonoBehaviour
{
    #region SerializedFields
    [SerializeField] [Tooltip("The player's camera")] private Camera cam;
    [Space]
    [SerializeField] [Tooltip("The player's speed at which he walks")] private float moveSpeed = 5;
    [SerializeField] [Tooltip("The player's height at which he jumps")] private float jumpHeight = 1;
    [Space]
    [SerializeField] [Tooltip("The transform used to check whether the player is on the ground or not")] private Transform groundCheck;
    [SerializeField] [Tooltip("The distance at which the groundCheck will activate")] private float checkRadius;
    [Space]
    [SerializeField] [Tooltip("The passive gravity to apply to the player")] private float gravity = -9.81f;
    #endregion

    private CharacterController cc;

    private Vector3 velocity;

    private float upRotation;
    private Material matInHand = Material.Grass;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleInput();
        HandleMovement();
    }

    private void HandleInput()
    {
        if (!Input.anyKey) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            matInHand = Material.Grass;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            matInHand = Material.Dirt;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            matInHand = Material.Stone;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit result;
            if (Physics.Raycast(ray, out result, 5, LayerMask.GetMask("Ground")))
            {
                BreakBlock(result);
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit result;
            if (Physics.Raycast(ray, out result, 5, LayerMask.GetMask("Ground")))
            {
                PlaceBlock(result);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
        }
    }
    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");
        bool isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, LayerMask.GetMask("Ground"));
        upRotation += mouseY;
        upRotation = Mathf.Clamp(upRotation, -90, 90);

        velocity.y += gravity * Time.deltaTime;

        if (velocity.y < 0 && isGrounded)
            velocity.y = -2;

        Vector3 move = (horizontal * transform.right + vertical * transform.forward) * moveSpeed;

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        Vector3 camRot = cam.transform.rotation.eulerAngles;
        camRot.x = upRotation;

        cam.transform.rotation = Quaternion.Euler(camRot);
        transform.rotation *= Quaternion.Euler(0, mouseX, 0);

        cc.Move(velocity * Time.deltaTime);
        cc.Move(move * Time.deltaTime);
    }

    private void BreakBlock(RaycastHit result)
    {
        Chunk chunk = result.collider.GetComponent<Chunk>();
        if (chunk == null) return;

        Vector3 hitPoint = result.point - result.normal / 4;
        hitPoint.y = Mathf.Ceil(hitPoint.y);
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

        Vector3 hitPoint = result.point + result.normal / 4;
        hitPoint.y = Mathf.Ceil(hitPoint.y);

        if ((int)hitPoint.x == (int)transform.position.x && (int)hitPoint.y == (int)(transform.position.y + 0.15f) && (int)hitPoint.z == (int)transform.position.z)
            return;
        if ((int)hitPoint.x == (int)transform.position.x && (int)hitPoint.y == (int)(transform.position.y + transform.lossyScale.y / 2f + 0.2f) && (int)hitPoint.z == (int)transform.position.z)
            return;

        TerrainManager.instance.PlaceBlock(hitPoint, matInHand);
        chunk.RecalculateMesh();
    }
}
