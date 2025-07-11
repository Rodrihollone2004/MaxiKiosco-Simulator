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
    //[SerializeField] private AudioSource audioSource;
    //[SerializeField] private AudioClip jumpSound, landSound, walkSound, sprintSound;

    private bool isWalking = false;

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
        wasGrounded = grounded;
    }

    // verifica contacto con el suelo, procesa inputs, maneja estados y aplica drag si esta entierra
    private void Update()
    {
        float groundCheckDistance = (transform.localScale.y / startYScale) * (playerHeight * 0.5f + 0.2f);
        grounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        if (grounded)
        {
            rb.drag = groundDrag;

            if (!wasGrounded)
            {
                //audioSource.PlayOneShot(landSound);
                wasGrounded = true;
            }
        }
        else
        {
            rb.drag = 0;
            wasGrounded = false;
        }
        //HandleFootsteps();
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
        //audioSource.PlayOneShot(jumpSound);
    }

    // reactiva el salto
    private void ResetJump()
    {
        readyToJump = true;
    }

    // para sonidos
    //private void HandleFootsteps()
    //{
    //    // No reproducir pasos si estamos en el aire
    //    if (!grounded) return;

    //    if (grounded && (horizontalInput != 0 || verticalInput != 0))
    //    {
    //        if (state == MovementState.walking)
    //        {
    //            if (!audioSource.isPlaying || audioSource.clip != walkSound)
    //            {
    //                audioSource.clip = walkSound;
    //                audioSource.Play();
    //            }
    //        }
    //        else if (state == MovementState.sprinting)
    //        {
    //            if (!audioSource.isPlaying || audioSource.clip != sprintSound)
    //            {
    //                audioSource.clip = sprintSound;
    //                audioSource.Play();
    //            }
    //        }
    //        isWalking = true;
    //    }
    //    else if (isWalking)
    //    {
    //        audioSource.Stop();
    //        isWalking = false;
    //    }
    //}

}
