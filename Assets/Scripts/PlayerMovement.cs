using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Character : MonoBehaviour
{
    
    //Jump
    private bool isJumping = false;
    private float jumpCooldownTimer;
    private InputAction jumpAction;
    private Vector3 jumpVelocity;
    
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float jumpSpeed;
    
    //CharacterMovement
    private CharacterController controller;
    private InputAction moveAction;
    
    [SerializeField] private float characterSpeed;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private float dampening;
    private Vector3 characterMovement;
    
    //Gravity
    [SerializeField] private float gravity;
    private Vector3 characterGravity;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        this.jumpCooldownTimer = 0.0f;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }


    private void FixedUpdate()
    {
        this.HandleJumping();
        
        var inputMovement = moveAction.ReadValue<Vector2>();
        var inputForwardDirection = this.cameraTransform.forward;
        var inputRightDirection = this.cameraTransform.right;

        inputRightDirection.y = 0.0f;
        inputForwardDirection.y = 0.0f;
        inputForwardDirection.Normalize();
        inputRightDirection.Normalize();

        if (this.controller.isGrounded)
        {
            this.characterGravity.y = 0.0f;
        }
        
        this.characterGravity.y += this.gravity * Time.fixedDeltaTime;
        this.characterMovement += this.characterGravity * Time.fixedDeltaTime;
        this.characterMovement += this.jumpVelocity * Time.fixedDeltaTime;
        this.characterMovement += inputRightDirection * inputMovement.x * characterSpeed * Time.fixedDeltaTime;
        this.characterMovement += inputForwardDirection * inputMovement.y * characterSpeed * Time.fixedDeltaTime;
        
        GetPlatformVelocity();

        this.characterMovement *= (1.0f - this.dampening);

        Vector3 characterForward = this.characterMovement;
        characterForward.y = 0.0f;

        if (characterForward.sqrMagnitude > 0.0f && characterForward != Vector3.zero)
        {
            this.transform.forward = characterForward.normalized;
        }
        
        this.controller.Move(this.characterMovement);
    }

    void HandleJumping()
    {
        if (this.controller.isGrounded && this.isJumping && this.jumpCooldownTimer <= 0.0f)
        {
            this.jumpVelocity = Vector3.zero;
            this.isJumping = false;
        }

        if (this.controller.isGrounded && !this.isJumping && this.jumpAction.WasPressedThisFrame())
        {
            this.characterGravity = Vector3.zero;
            this.jumpVelocity = Vector3.zero;
            this.jumpVelocity.y = this.jumpSpeed;
            this.jumpCooldownTimer = this.jumpCooldown;
            this.isJumping = true;
        }

        if (this.jumpVelocity.y > 0.0f)
        {
            this.jumpVelocity.y -= Time.fixedDeltaTime;
        }
        else
        {
            this.jumpVelocity = Vector3.zero;
        }

        this.jumpCooldownTimer -= Time.fixedDeltaTime;
    }

    private void GetPlatformVelocity()
    {
        LayerMask mask = LayerMask.GetMask("Platforms");
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.0f, mask))
        {
            GameObject obj = hit.collider.gameObject;
            MovingPlatform platform = obj.GetComponent<MovingPlatform>();

            if (platform != null)
            {
                Vector3 platformVelocity = platform.GetVelocity();
            
                var combinedMovement = this.characterMovement + platformVelocity * Time.fixedDeltaTime;
                controller.Move(combinedMovement);
            }
            
        }
    }
}