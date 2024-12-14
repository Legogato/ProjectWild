using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;



public class PlayerMovementCC : MonoBehaviour
{

    //General variables
    [Header("Assign")]
    [SerializeField] private GameInput gameInput;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cam;
    [SerializeField] private GameObject visual;
    private float time;

    //Movement variables
    [Header("Movement")]
    private float smoothVelocity;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float originalStepOffset;
    [SerializeField] private float ySpeed;
    [SerializeField] private float currentVelocity;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float dashPower;
    [SerializeField] private float dashTime;
    private Vector3 moveDir;
    private float targetAngle;
    [SerializeField] private bool holdToRun = false;

    //Jump variables
    [Header("Jump")]
    [SerializeField] private float jumpForce = 3f;
    private bool canJump = true;
    [SerializeField] private int extraJumps = 1;
    private int extraJumpsAux;
    public float fallVelocity;

    [SerializeField] private MovementState currentState;

    private bool canDash = true;
    [SerializeField] private float dashCooldown;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air,
        idle
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        gameInput.OnJump += Jump;
        gameInput.OnCrouch += Crouch;
        gameInput.OnCrouchCanceled += CancelCrouch;
        gameInput.OnSprint += Sprint;
        gameInput.OnSprintCanceled += CancelSprint;
        gameInput.OnSuperJump += SuperJump;
        gameInput.OnDash += Dash;
        gameInput.OnAirWave += AirWave;
        extraJumpsAux = extraJumps;
        walkSpeed = moveSpeed;
    }

    private void AirWave(object sender, EventArgs e)
    {
        if(currentState == MovementState.air)
        {
            
        }
    }

    private void Dash(object sender, EventArgs e)
    {     
        if (canDash)
        {
            canDash = false;
            StartCoroutine(DashCoroutine());
            StartCoroutine(CoolDown(dashCooldown));
        }
        
    }
    private IEnumerator DashCoroutine()
    {
        float startTime = Time.time;
        
        while (Time.time < startTime + dashTime)
        {        
            controller.Move(Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * dashPower * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            yield return null;
        }
    }

    private IEnumerator AirWaveCoroutine()
    {
        float startTime = Time.time;
        float x = 0;
        float y = Mathf.Sqrt(x) + (4*x);

        while (Time.time < startTime + dashTime)
        {
            
            controller.Move(new Vector3(x, y, 0f));
            x += 1;
            yield return null;
        }
    }

    private IEnumerator CoolDown(float dashCoolDown)
    {
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true ;
        
    }
    
    private void Update()
    {
        ManageJumpAmount();
        StateHandler();
        MovePlayer();
        VerifySprint();
        Fall();
        if (controller.isGrounded)
        {
            controller.stepOffset = originalStepOffset;
            ySpeed = -0.5f;
        }
        else
        {
            controller.stepOffset = 0;
        }
    }

    private void StateHandler()
    {
        // Mode - Walking
        if (controller.isGrounded && moveSpeed == walkSpeed && moveDir.magnitude > 0)
        {
            currentState = MovementState.walking;
        }
        //Mode - crouching
        else if (controller.isGrounded && moveSpeed == crouchSpeed)
        {
            currentState = MovementState.crouching;
        }
        //Mode - Sprinting
        else if (controller.isGrounded && moveSpeed == sprintSpeed && moveDir.magnitude > 0)
        {
            currentState = MovementState.sprinting;
        }
        else if (!controller.isGrounded)
        {
            currentState = MovementState.air;
        }
        else if(controller.isGrounded && moveDir.magnitude == 0)
        {
            currentState = MovementState.idle;
        }

    }
    public void MovePlayer()
    {
        //Read the input
        Vector2 inputVector = gameInput.GetMovementNormalized();
        //Calculate the direction
        Vector3 direction = new Vector3(inputVector.x, 0f, inputVector.y);
        //Calcultes the camera rotation
        targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.rotation.eulerAngles.y;
        moveDir = Vector3.zero;
        //Calculates the angle for the character rotation
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothVelocity, 0.1f);


        if (direction.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        Vector3 velocity = moveDir * moveSpeed;
        velocity = AdjustVelocityToSlope(velocity);
        velocity.y += ySpeed;

        controller.Move(velocity * Time.deltaTime);
        

    }

    private void Crouch(object sender, System.EventArgs e)
    {
        if (controller.isGrounded)
        {
            moveSpeed = crouchSpeed;
        }
              
    }

    private void CancelCrouch(object sender, EventArgs e)
    {
        moveSpeed = walkSpeed;

    }

    private void Sprint(object sender, EventArgs e)
    {
        moveSpeed = sprintSpeed;
    }

    private void CancelSprint(object sender, EventArgs e)
    {
        if (holdToRun)
        {
            moveSpeed = walkSpeed;
        }
        
    }

    private void VerifySprint()
    {
        if (!holdToRun && currentState == MovementState.idle)
        {
            moveSpeed = walkSpeed;
        }
    }

    private void Jump(object sender, System.EventArgs e)
    {
        if (canJump)
        {
            ySpeed = jumpForce;
            extraJumps--;
        }

    }
    private void ResetJump()
    {
        canJump = true;
    }
    private void ManageJumpAmount()
    {
        if (controller.isGrounded)
        {
            extraJumps = extraJumpsAux;
            
            ResetJump();
        }

        if (extraJumps == 0) canJump = false;
    }

 
    private void SuperJump(object sender, EventArgs e)
    {
        if (canJump && currentState == MovementState.crouching)
        {
            ySpeed = jumpForce * 2;
            moveSpeed = walkSpeed;
            extraJumps--;
        }
    }
    private void Fall()
    {
        ySpeed += Physics.gravity.y * Time.deltaTime * fallVelocity;
    }

    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 0.2f))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelocity = slopeRotation * velocity;

            if (adjustedVelocity.y < 0)
            {
                return adjustedVelocity;
            }
        }

        return velocity;
    }

    public MovementState GetMovementState()
    {
        return currentState;
    }

}
