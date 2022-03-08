using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TP_Controller : MonoBehaviour
{
    //Serialized Variables
    [SerializeField] private float playerWalkSpeed;
    [SerializeField] private float playerRunningSpeed;
    [SerializeField] private float playerPistolSpeed;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private float playerJumpHeight;
    [SerializeField] private float gravityValue = -9.8f;
    [SerializeField] [Range(0f,1f)] private float gravityScale = 0.2f;
    [SerializeField] private float groundedOffset;
    [Tooltip("The interpolation factor for input values but also for animation blending")]
    [SerializeField] [Range(0f, 2f)] private float inputSmoothTime = 0.1f;
    
    
    //private Variables
    private float angleSmoothVelocity;
    private float playerSpeed;
    private float playerVerticalVelocity;
    private int animForwardSpeedID;
    private int animhorizontalSpeedID;
    private int animForwardRunSpeedID;
    private int animHorizontalRunSpeedID;
    private int animIsRunningID;
    private int animPistolLayerIndexID;
    private int animIsPistolID;
    private int animIsRifleID;
    private Vector2 inputSmoothVelocity;
    private Vector3 groundCheckSpherePos;
    private Vector2 inputBlendVector;
    private Vector2 movementInputVector;
    private Vector2 gunInputBlendVector;
    private bool isPlayerGrounded;
    private bool isPlayerRunning;
    private bool isPlayerCrouched;
    private bool isPlayerJumping;
    private bool isUsingPistol;
    private bool isUsingRifle;
    InputAction sprintAction;
    //Serialized components
    [SerializeField] private Transform CameraTransform;
    [SerializeField] private LayerMask GroundLayerMask;
    [SerializeField] CharacterController playerCharacterController;
    //private Components
    Animator playerAnimController;
    PlayerInputActions playerInputActions;

    //Getters
    public bool IsUsingPistol()
    {
        return isUsingPistol;
    }
    public bool IsUsingRifle()
    {
        return isUsingRifle;
    }
    //Setters
    public void SetIsUsingPistol(bool isusingpistol)
    {
        isUsingPistol = isusingpistol;
    }
    public void SetIsUsingRifle(bool isusingrifle)
    {
        isUsingRifle = isusingrifle;
    }

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Movement.Enable();
    }

    void Start()
    {
        //playerCharacterController = GetComponent<CharacterController>();
        playerAnimController = GetComponent<Animator>();
        SetAnimationIDs();
        isPlayerGrounded = false;
        
        playerInputActions.Movement.Movement.performed += Movement_Performed;
    }

    // Update is called once per frame
    void Update()
    {
        movementInputVector = playerInputActions.Movement.Movement.ReadValue<Vector2>();
        if(!isUsingPistol) 
        {
            //HandlePlayerMovement(movementInputVector);
            HandlePlayerStrafe(movementInputVector);
            playerAnimController.SetBool(animIsPistolID, false);

            playerAnimController.SetBool(animIsRifleID, false);
        }        
        else if(isUsingPistol && !isUsingRifle)
        {
            playerAnimController.SetBool(animIsPistolID, true);
            playerAnimController.SetBool(animIsRifleID, false);
            HandlePlayerGunMovement(movementInputVector);
        }
        else if(isUsingRifle && !isUsingPistol) 
        {
            playerAnimController.SetBool(animIsRifleID, true);
            playerAnimController.SetBool(animIsPistolID, false);
            HandlePlayerGunMovement(movementInputVector);
        }
        
    }

    private void FixedUpdate() {
        GroundCheck();
        UpdateGravity();
    }

    void HandlePlayerMovement(Vector2 inputVector) {
        
        if (Input.GetKey(KeyCode.LeftShift)) {
            playerSpeed = playerRunningSpeed;
            isPlayerRunning = true;
            //playerAnimController.SetTrigger(animIsRunningID);
        }
        else {
            playerSpeed = playerWalkSpeed;
            isPlayerRunning = false;
            //playerAnimController.ResetTrigger(animIsRunningID);
        }
        inputBlendVector = Vector2.SmoothDamp(inputBlendVector, inputVector, ref inputSmoothVelocity, inputSmoothTime); 
        float horizontal = inputBlendVector.x;
        float forward = inputBlendVector.y;
        Vector3 playerMoveDirection = new Vector3(horizontal, 0, forward);
        playerMoveDirection.Normalize();
        if (playerMoveDirection.magnitude >= 0.05f) {

            float playerTargetAngle = Mathf.Atan2(playerMoveDirection.x, playerMoveDirection.z) * Mathf.Rad2Deg + CameraTransform.eulerAngles.y;
            float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, playerTargetAngle, ref angleSmoothVelocity, smoothTime);
            transform.rotation = Quaternion.Euler(0, smoothedAngle, 0);
            Vector3 playerDirection = Quaternion.Euler(0, playerTargetAngle, 0) * Vector3.forward;
            //playerAnimController.SetFloat(animForwardSpeedID, playerDirection.magnitude * Time.deltaTime);
            playerCharacterController.Move(playerDirection.normalized * (playerSpeed * Time.deltaTime) + new Vector3(0,playerVerticalVelocity,0) * Time.deltaTime);
            if (playerAnimController && !isPlayerRunning) {
                playerAnimController.SetFloat(animForwardSpeedID, forward / 2);
                playerAnimController.SetFloat(animhorizontalSpeedID, horizontal / 2);
            }
            else if (playerAnimController && isPlayerRunning) {
                Debug.Log("is running");
                playerAnimController.SetFloat(animForwardSpeedID, forward);
                playerAnimController.SetFloat(animhorizontalSpeedID, horizontal);
            }
        }
    }

    void HandlePlayerStrafe(Vector2 inputVector) {

        
        gunInputBlendVector = Vector2.SmoothDamp(gunInputBlendVector, inputVector, ref inputSmoothVelocity, inputSmoothTime);
        float horizontal = gunInputBlendVector.x;
        float forward = gunInputBlendVector.y;
        Vector3 playerGunMoveDirection = new Vector3(horizontal, 0, forward);
        playerGunMoveDirection.Normalize();
        if(playerGunMoveDirection.magnitude >= 0.05f) {
            if(Input.GetKey(KeyCode.LeftShift)) {
                playerAnimController.SetBool(animIsRunningID, true);
                EnableRotateToDirection(playerGunMoveDirection);
            } else {
                playerAnimController.SetBool(animIsRunningID, false);
                transform.rotation = Quaternion.Euler(0, CameraTransform.eulerAngles.y, 0);
                Vector3 playerGunDirection = Quaternion.Euler(0, CameraTransform.eulerAngles.y, 0) * Vector3.forward;
                UpdateGravity();
            }
            
            
            //playerCharacterController.Move(playerGunDirection.normalized * (playerPistolSpeed * Time.deltaTime) + new Vector3(0, playerVerticalVelocity, 0) * Time.deltaTime);
            playerAnimController.SetFloat(animForwardSpeedID, forward);
            playerAnimController.SetFloat(animhorizontalSpeedID, horizontal);
        }
        
    }
    void HandlePlayerGunMovement(Vector2 inputVector) {

        //playerAnimController.SetLayerWeight(animPistolLayerIndexID, 1);
        gunInputBlendVector = Vector2.SmoothDamp(gunInputBlendVector, inputVector, ref inputSmoothVelocity, inputSmoothTime);
        float horizontal = gunInputBlendVector.x;
        float forward = gunInputBlendVector.y;
        Vector3 playerGunMoveDirection = new Vector3(horizontal, 0, forward);
        playerGunMoveDirection.Normalize();
        if(playerGunMoveDirection.magnitude >= 0.05f) {
            
            transform.rotation = Quaternion.Euler(0, CameraTransform.eulerAngles.y, 0);
            Vector3 playerGunDirection = Quaternion.Euler(0, CameraTransform.eulerAngles.y, 0) * Vector3.forward;
            UpdateGravity();
            //playerCharacterController.Move(playerGunDirection.normalized * (playerPistolSpeed * Time.deltaTime) + new Vector3(0, playerVerticalVelocity, 0) * Time.deltaTime);
            playerAnimController.SetFloat(animForwardSpeedID, forward);
            playerAnimController.SetFloat(animhorizontalSpeedID, horizontal);
        }
        

    }

    void UpdateGravity() {
        playerVerticalVelocity += gravityValue* gravityScale * Time.deltaTime;
        playerCharacterController.Move(new Vector3(0, playerVerticalVelocity, 0) * Time.deltaTime);
    }

    void HandleJump() {

    }

    void GroundCheck() {
        groundCheckSpherePos = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        if (Physics.CheckSphere(groundCheckSpherePos, playerCharacterController.radius, GroundLayerMask)) {
            Debug.Log("Grounded");
            isPlayerGrounded = true;
        }
    }

    private void Movement_Performed(InputAction.CallbackContext context) {
        Debug.Log(context);
    }

    void SetAnimationIDs() {
        animForwardSpeedID = Animator.StringToHash("Forward_Speed");
        animhorizontalSpeedID = Animator.StringToHash("Horizontal_Speed");
        animIsRunningID = Animator.StringToHash("isRunning");
        animForwardRunSpeedID = Animator.StringToHash("Forward_Run_Speed");
        animHorizontalRunSpeedID = Animator.StringToHash("Horizontal_Run_Speed");
        animPistolLayerIndexID = playerAnimController.GetLayerIndex("Pistol");
        animIsPistolID = Animator.StringToHash("isPistol");
        animIsRifleID = Animator.StringToHash("IsRifle");
    }

    //Used for player to rotate toward the direction he is moving 
    // through the character controller
    private void EnableRotateToDirection(Vector3 playerDirection) {
        float playerTargetAngle = Mathf.Atan2(playerDirection.x, playerDirection.z) * Mathf.Rad2Deg + CameraTransform.eulerAngles.y;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, playerTargetAngle, ref angleSmoothVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0, smoothAngle, 0);
        //Vector3 playerMoveDirection = Quaternion.Euler(0, playerTargetAngle, 0) * Vector3.forward;
        //playerCharacterController.Move(playerMoveDirection * PlayerCurrentSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected() {
        if (isPlayerGrounded) Gizmos.color = Color.green;
        else Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckSpherePos, 0.3f);
    }
}
