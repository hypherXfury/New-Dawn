using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float crouchSpeed = 2.5f;
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;

    private CharacterController controller;

    [Header("Head Bobbing Settings")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;

    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.1f;

    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.03f;

    [SerializeField] private Transform cameraTransform;
    private float defaultYPos;
    private float bobTimer;


    private Vector3 velocity;
    private bool isGrounded;

    [Header("Footstep Settings")]
    public AudioSource footstepSource;

    public AudioClip[] walkClips;
    public AudioClip[] sprintClips;
    public AudioClip[] crouchClips;

    public float walkStepDelay = 0.5f;
    public float sprintStepDelay = 0.3f;
    public float crouchStepDelay = 0.7f;

    private float footstepTimer = 0f;


    private void Start()
    {
        controller = GetComponent<CharacterController>();

        defaultYPos = cameraTransform.localPosition.y;
    }

    private void Update()
    {
        Move();
        HandleJumping();
        ApplyGravity();
        HeadBob();
        HandleFootsteps();
    }

    void Move()
    {
        isGrounded = controller.isGrounded;
        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        float speed = walkSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = sprintSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            controller.height = crouchHeight;
            speed = crouchSpeed;
        }
        else
        {
            controller.height = standingHeight;
        }

        controller.Move(move * speed * Time.deltaTime);
    }

    void HandleJumping()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HeadBob()
    {
        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        if (!isGrounded || !isMoving)
        {
            bobTimer = 0f;
            cameraTransform.localPosition = new Vector3(
                cameraTransform.localPosition.x,
                Mathf.Lerp(cameraTransform.localPosition.y, defaultYPos, Time.deltaTime * 5f),
                cameraTransform.localPosition.z
            );
            return;
        }

        // Determine bob speed and amount based on state
        float bobSpeed;
        float bobAmount;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            bobSpeed = sprintBobSpeed;
            bobAmount = sprintBobAmount;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            bobSpeed = crouchBobSpeed;
            bobAmount = crouchBobAmount;
        }
        else
        {
            bobSpeed = walkBobSpeed;
            bobAmount = walkBobAmount;
        }

        bobTimer += Time.deltaTime * bobSpeed;
        float newY = defaultYPos + Mathf.Sin(bobTimer) * bobAmount;

        cameraTransform.localPosition = new Vector3(
            cameraTransform.localPosition.x,
            newY,
            cameraTransform.localPosition.z
        );
    }

    void HandleFootsteps()
{
    bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

    if (!isGrounded || !isMoving)
    {
        footstepTimer = 0f;
        return;
    }

    footstepTimer -= Time.deltaTime;

    if (footstepTimer <= 0f)
    {
        AudioClip selectedClip = null;

        if (Input.GetKey(KeyCode.LeftShift) && sprintClips.Length > 0)
        {
            selectedClip = sprintClips[Random.Range(0, sprintClips.Length)];
            footstepTimer = sprintStepDelay;
        }
        else if (Input.GetKey(KeyCode.LeftControl) && crouchClips.Length > 0)
        {
            selectedClip = crouchClips[Random.Range(0, crouchClips.Length)];
            footstepTimer = crouchStepDelay;
        }
        else if (walkClips.Length > 0)
        {
            selectedClip = walkClips[Random.Range(0, walkClips.Length)];
            footstepTimer = walkStepDelay;
        }

        if (selectedClip != null)
        {
            footstepSource.PlayOneShot(selectedClip);
        }
    }
}



}
