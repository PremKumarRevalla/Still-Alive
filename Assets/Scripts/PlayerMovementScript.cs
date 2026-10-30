using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    public InputActionAsset InputActions;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    private CinemachineOrbitalFollow orbitalFollow;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask ItemsLayer;
    [SerializeField] private PlayerEquipmentScript playerEquipmentScript;
    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private LightStickController lightStickController;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private PlayerStats playerStats;

    private bool isGrounded;
    public bool isRunning = false;

    private InputAction move_action;
    private InputAction run_action;
    private InputAction jump_action;
    private InputAction look_action;
    private InputAction equip_action;
    private InputAction attack_action;
    private InputAction interact_action;
    private InputAction take_lightstick_action;
    private InputAction eating_action;

    private Vector2 moveValue;
    private Vector2 lookValue;
    private Animator animator;
    private Rigidbody rb;

    public float walkSpeed = 1f;
    public float runSpeed = 3f; 
    public float jumpForce = 2.5f;
    public float lookSensitivity = 5f;
    public float moveSpeed;
    private bool isMoving = false;
    private bool holdingWeapon = false;
    public bool isEating = false;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }
    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        move_action = InputSystem.actions.FindAction("Move");
        look_action = InputSystem.actions.FindAction("Look");
        jump_action = InputSystem.actions.FindAction("Jump");
        run_action = InputSystem.actions.FindAction("Sprint");
        equip_action = InputSystem.actions.FindAction("PickWeapon");
        attack_action = InputSystem.actions.FindAction("Attack");
        interact_action = InputSystem.actions.FindAction("Interact");
        take_lightstick_action = InputSystem.actions.FindAction("TakeLightStick");
        eating_action = InputSystem.actions.FindAction("Eating");
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerEquipmentScript = GetComponent<PlayerEquipmentScript>();
        orbitalFollow = cinemachineCamera.GetComponent<CinemachineOrbitalFollow>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveValue = move_action.ReadValue<Vector2>();
        lookValue = look_action.ReadValue<Vector2>();
        checkMovement();
        RotateCamera();
        CheckGround();
        if (jump_action.WasPressedThisFrame())
        {
            Jump();
        }
        if (equip_action.WasPressedThisFrame())
        {
            if(isMoving)
            {
                return;
            }
            holdingWeapon = !holdingWeapon;
            EquipWeapon();
            animator.SetBool("HoldingWeapon", holdingWeapon);
        }
        if (attack_action.WasPressedThisFrame())
        {
            if (!holdingWeapon || isMoving)
            {
                return;
            }
            animator.SetTrigger("Hit");
        }
        if(take_lightstick_action.WasPressedThisFrame())
        {
            if (isMoving)
                return;
            if (dayNightCycle.IsNight == true)
            {
                if(lightStickController.isLightStickActive == false)
                {
                    lightStickController.TakeLightStick();
                    animator.SetBool("HoldingLightStick", true);
                }
                else
                {
                    lightStickController.StoreLightStick();
                    animator.SetBool("HoldingLightStick", false);
                }
            }
            
        }
        Eating();
        if (interact_action.WasPressedThisFrame())
        {
            if (isMoving)
                return;

            RaycastHit hit;

            if (Physics.SphereCast(
                Camera.main.transform.position,
                0.5f,
                Camera.main.transform.forward,
                out hit,
                4f,
                ItemsLayer,
                QueryTriggerInteraction.Collide))
            {
                Debug.Log("Hit Object: " + hit.collider.name);

                WorldItemPickUp pickup =
                    hit.collider.GetComponentInParent<WorldItemPickUp>();

                if (pickup != null)
                {
                    Debug.Log("Pickup detected!");
                    pickup.Interact();
                    animator.SetTrigger("PickItem");
                }
            }
        }
        animator.SetFloat("Health", playerStats.currentHealth);
    }
    private void FixedUpdate()
    {
        Walking();
        Rotating();
    }

    public void Jump()
    {
        if (!isGrounded)
            return;
        if (playerStats.currentStamina < playerStats.jumpingStaminaCost)
            return;

        playerStats.UseStamina(playerStats.jumpingStaminaCost);

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animator.SetTrigger("Jump");

        isGrounded = false;
    }
    public void EquipWeapon()
    {
        if (holdingWeapon)
        {
            playerEquipmentScript.TakeWeapon();
        }
        else
        {
            playerEquipmentScript.StoreWeapon();
        }
    }
    private void Walking()
    {

        moveSpeed = (run_action.IsPressed() && !lightStickController.isLightStickActive && playerStats.currentStamina > 10f) ? runSpeed : walkSpeed;
        // Get camera directions
        Transform cam = Camera.main.transform;

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        // Ignore camera's vertical angle
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Move relative to camera direction
        Vector3 movement =
            forward * moveValue.y +
            right * moveValue.x;

        movement = Vector3.ClampMagnitude(movement, 1f);

        animator.SetFloat("MoveSpeed", moveSpeed);

        rb.MovePosition(
            rb.position + movement * moveSpeed * Time.fixedDeltaTime
        );
        if (moveValue.x != 0f && moveValue.y != 0f && playerStats.currentStamina > 0f)
        {
            isRunning = true;

            playerStats.UseStamina(
                playerStats.runningStaminaCost * Time.deltaTime
            );
        }
        else
        {
            isRunning = false;
        }
    }
    private void checkMovement()
    {
        if(moveValue.x != 0 || moveValue.y != 0)
        {
            isMoving = true;
            animator.SetBool("isMoving", isMoving);
        }
        else
        {
            isMoving = false;
            animator.SetBool("isMoving", isMoving);
        }
    }

    public void Eating()
    {
        if (eating_action.IsPressed())
        {
            if (isMoving)
            {
                isEating = false;
                animator.SetBool("Eating", false);
                return;
            }

            if (!isEating)
            {
                if (inventoryManager.mushrooms <= 0)
                    return;

                // Consume one mushroom
                inventoryManager.mushrooms--;

                // Restore hunger and health
                playerStats.RestoreHunger(35f);
                playerStats.Heal(20f);

                Debug.Log("Mushroom consumed! Health and hunger restored.");
            }

            isEating = true;
            animator.SetBool("Eating", true);
        }
        else
        {
            isEating = false;
            animator.SetBool("Eating", false);
        }
    }

    private void Rotating()
    {

        if (moveValue.x != 0 || moveValue.y != 0)
        {
            Transform cam = Camera.main.transform;

            Vector3 forward = cam.forward;
            Vector3 right = cam.right;

            // Ignore the camera's vertical angle
            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            // Calculate the same direction as Walking()
            Vector3 movementDirection =
                forward * moveValue.y +
                right * moveValue.x;

            movementDirection.Normalize();

            Quaternion targetRotation =
                Quaternion.LookRotation(movementDirection);

            rb.MoveRotation(
                Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    lookSensitivity * Time.fixedDeltaTime
                )
            );
        }
    }
    private void RotateCamera()
    {
        orbitalFollow.HorizontalAxis.Value += lookValue.x * lookSensitivity * Time.deltaTime;
        //orbitalFollow.VerticalAxis.Value += lookValue.y * lookSensitivity * Time.deltaTime;
        //orbitalFollow.VerticalAxis.Value = Mathf.Clamp(orbitalFollow.VerticalAxis.Value, -20f, 70f);

    }
    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);
    }
}
