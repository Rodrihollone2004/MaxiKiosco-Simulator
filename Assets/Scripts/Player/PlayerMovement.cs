using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;

    [SerializeField] private float groundDrag;

    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    private bool readyToJump;

    [Header("Crouching")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    private bool grounded;
    private bool wasGrounded;

    //[Header("Sounds")]
    //[SerializeField] private AudioSource movementAudioSource;
    //[SerializeField] private AudioSource jumpAudioSource;
    //[SerializeField] private AudioClip[] footstepSounds;
    //[SerializeField] private AudioClip jumpSound;
    //[SerializeField] private AudioClip landSound;
    //[SerializeField] private float footstepIntervalWalk = 0.5f;
    //[SerializeField] private float footstepIntervalRun = 0.3f;
    //private float footstepTimer;

    [SerializeField] private Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rb;

    [SerializeField] private MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;
        //wasGrounded = grounded;
    }

    // verifica contacto con el suelo, procesa inputs, maneja estados y aplica drag si esta entierra
    private void Update()
    {
        float groundCheckDistance = (transform.localScale.y / startYScale) * (playerHeight * 0.5f + 0.2f);
        grounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, whatIsGround);

        //if (grounded && !wasGrounded)
        //{
        //    PlayLandingSound();
        //}
        //wasGrounded = grounded;

        MyInput();
        SpeedControl();
        StateHandler();

        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    // mueve al jugador con fisicas
    private void FixedUpdate()
    {
        MovePlayer();
    }

    // captura los inputs y maneja la logica de salto con un cooldown, ademas de cambiar la escala si se agacha
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();
            //PlayJumpSound();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    // determina el estado actual
    private void StateHandler()
    {
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
            return;
        }
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    // calcula la direccion relativa a la orientacion
    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    // limita la velocidad horizontal maxima
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    // resetea la velocidad vertical
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    // reactiva el salto
    private void ResetJump()
    {
        readyToJump = true;
    }

    // para sonidos
    //private void HandleFootsteps()
    //{
    //    if (!grounded || (horizontalInput == 0 && verticalInput == 0))
    //    {
    //        footstepTimer = 0;
    //        return;
    //    }

    //    float currentInterval = state == MovementState.sprinting ? footstepIntervalRun : footstepIntervalWalk;
    //    footstepTimer += Time.deltaTime;

    //    if (footstepTimer >= currentInterval)
    //    {
    //        PlayFootstepSound();
    //        footstepTimer = 0;
    //    }
    //}

    //private void PlayFootstepSound()
    //{
    //    if (footstepSounds.Length == 0 || !movementAudioSource) return;

    //    AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
    //    movementAudioSource.pitch = Random.Range(0.9f, 1.1f);
    //    movementAudioSource.PlayOneShot(clip);
    //}

    //private void PlayJumpSound()
    //{
    //    if (jumpSound && jumpAudioSource)
    //    {
    //        jumpAudioSource.PlayOneShot(jumpSound);
    //    }
    //}

    //private void PlayLandingSound()
    //{
    //    if (landSound && movementAudioSource)
    //    {
    //        movementAudioSource.PlayOneShot(landSound);
    //    }
    //}
}
