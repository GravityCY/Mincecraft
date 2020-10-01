using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    #region SerializedFields
    [SerializeField] private Camera playerCam;
    [Space]
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float jumpHeight = 1;
    [Space]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius;
    [Space]
    [SerializeField] private float gravity = -9.81f;
    #endregion

    private CharacterController cc;

    private Vector3 velocity;

    private float upRotation;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
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

        Vector3 camRot = playerCam.transform.rotation.eulerAngles;
        camRot.x = upRotation;

        playerCam.transform.rotation = Quaternion.Euler(camRot);
        transform.rotation *= Quaternion.Euler(0, mouseX, 0);

        cc.Move(velocity * Time.deltaTime);
        cc.Move(move * Time.deltaTime);
    }
}
