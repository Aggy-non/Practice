using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class StateCharacterControl : MonoBehaviour
{
    #region fields
    [Header("Components")]
    private InputManager inputManager;
    private CapsuleCollider charCollider;
    private Rigidbody rb;
    public Rigidbody Rb => rb;

    [Header("Ground check settings")]
    [SerializeField] private LayerMask movementLayer;
    private bool isGrounded;
    public bool IsGrounded => isGrounded;

    [Header("Movement settings")]
    private Vector3 InputDir;
    private Vector3 moveDir;
    public Vector3 MoveDir => moveDir;
    private Vector3 faceDir;
    public Vector3 FaceDir => faceDir;
    private float multiplier;

    [Header("Physics settings")]
    private float gravityScale = 1f;
    private float currentDrag;

    [Header("Jump settings")]
    private float bufferTimeCounter;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private Transform aimCameraPosition;

    [Header("Charge Jump")]
    private StopWatch stopWatch;

    [Header("Wall Climb")]
    [SerializeField] Transform wallCheckPoint;
    private bool wallDetected;
    public bool WallDetected => wallDetected;
    public enum Surface { Wall, Ground, Air }
    private Surface currentSurface = Surface.Air;
    private RaycastHit wallHit;

    //State Machine
    private CharacterStateMachine stateMachine;
    public CharacterStateMachine StateMachine => stateMachine;


    #endregion
    private void Awake()
    {
        GetAllComponents();
        
        stateMachine = new CharacterStateMachine(this,inputManager);
    }

    private void Start()
    {
        AssignEvents();
        CursorLock();
        stateMachine.Initialize(stateMachine.idleState);
        
    }

    private void Update()
    {
        GroundCheck();
        CalculateCurrentMoveDir();
        WallCheck();
        stateMachine.Exectue();
        //printTheValues();
    }

    private void printTheValues()
    {
        print("current drag" + currentDrag);
    }
    private void FixedUpdate()
    {
        //make a seperate class for these gravity and drag
        CustomGravity();
        MoveCharacter();
        RotatePlayer();
        ApplyDrag();
    }

    // noted under current move vector
    private void CalculateCurrentMoveDir()
    {
        if (currentSurface == Surface.Wall)
        {
            moveDir = CalculateWallMoveDir();
        }
        else
        {
            moveDir = CalculateFloorMoveDir();
        }
    }

    //noted under floor move direction
    private Vector3 CalculateFloorMoveDir()
    {
        Vector3 camForward = Vector3.ProjectOnPlane(cameraPosition.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cameraPosition.right, Vector3.up).normalized;

        //moveDir = camForward * InputDir.z + camRight * InputDir.x;
        Vector3 normalizedReturnVector = (camForward * InputDir.y + camRight * InputDir.x).normalized;
        if (normalizedReturnVector != Vector3.zero)
        {
            //faceDir = normalizedReturnVector;
            SetFaceDir(normalizedReturnVector);
        }
        return normalizedReturnVector;
    }


    //noted under wall move direction
    private Vector3 CalculateWallMoveDir()
    {
        //faceDir = -wallHit.normal;
        SetFaceDir(-wallHit.normal);
        Vector3 wallRight = Vector3.Cross(Vector3.up, faceDir).normalized;
        Vector3 wallForward = Vector3.Cross(faceDir, wallRight).normalized;
        Vector3 wallMoveDir = wallRight * InputDir.x + wallForward * InputDir.y;

        return wallMoveDir.normalized;
    }

    //noted under movement application
    private void MoveCharacter()
    {
        rb.AddForce(moveDir * CharacterConstants.MoveSpeed * multiplier, ForceMode.VelocityChange);
        if (currentSurface == Surface.Wall)
        {
            SnapToWall();
        }
    }

    //noted under movement application
    //public void SetMoveSpeed(float speed)
    //{
    //    moveSpeed = speed;
    //}

    public void SetMoveSpeedMultiplier(float mmultiplier)
    {
        multiplier = mmultiplier;
    }
    //noted under snap to wall
    private void SnapToWall()
    {
        Vector3 checkPointOffset = wallCheckPoint.position - transform.position;
        Vector3 desiredCheckPoint = wallHit.point + wallHit.normal * (charCollider.radius + CharacterConstants.WallClearance);
        Vector3 target = desiredCheckPoint - checkPointOffset;

        // only the into/out-of-wall component - leaves along-wall movement alone
        Vector3 error = Vector3.Project(target - rb.position, wallHit.normal);

        float t = 1f - Mathf.Exp(-CharacterConstants.SnapSharpness * Time.fixedDeltaTime);
        rb.position += error * t;

        // kill velocity pushing into or off the wall, keep velocity along it
        rb.linearVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, wallHit.normal);
    }

    public void SetSurfaceType(Surface surface)
    {
        currentSurface = surface;
    }

    //noted under drag application
    public void SetDrag(float drag)
    {
        currentDrag = drag;
    }
    //noted under drag application
    private void ApplyDrag()
    {
        Vector3 vel = rb.linearVelocity;
        float dampFactor = Mathf.Exp(-currentDrag * Time.fixedDeltaTime);
        if (currentSurface == Surface.Wall)
        {
            vel.y *= dampFactor;
        }
        vel.x *= dampFactor;
        vel.z *= dampFactor;
        rb.linearVelocity = vel;
    }

    //noted under ground check
    private void GroundCheck()
    {
        Vector3 origin = transform.TransformPoint(charCollider.center);
        isGrounded = Physics.SphereCast(origin, CharacterConstants.FeetRadius, Vector3.down, out RaycastHit hitInfo, CharacterConstants.GroundCheckDistance, movementLayer);
    }

    //noted under wall check
    private void WallCheck()
    {
        Vector3 shootDir;
        if (currentSurface == Surface.Wall)
        {
            shootDir = wallHit.normal * -1;
            //so that it only checks for wall in front of player and not to the side player is moving
        }
        else
        {
            shootDir = faceDir;
        }
        Vector3 origin = wallCheckPoint.position;
        wallDetected = Physics.SphereCast(origin, CharacterConstants.WallCheckRadius, shootDir, out wallHit, CharacterConstants.WallCheckDistance, movementLayer);
    }

    //noted under normal jump
    private void PerformNormalJump()
    {
        if (currentSurface == Surface.Ground)
        {
            if (moveDir.magnitude<0.1f)//wont work because the current state becomes `startJumpState` before the jump is performed, so this will never be true 
            {                
                Vector3 jumpOffset = faceDir.normalized * 0.01f;
                CharacterJump(CharacterConstants.NormalJumpForce, Vector3.up + jumpOffset);   //little bit of an offset to make sure wallcheck doesnt check directly upwards         
                stateMachine.TransitionToState(stateMachine.airState);
            }
            else if (moveDir.magnitude>0.1f)
            {
                float wallInFrontDistance = 0.1f; 
                bool isThereAWallInFront = Physics.SphereCast(wallCheckPoint.position, CharacterConstants.WallCheckRadius, faceDir, out RaycastHit wallOut, wallInFrontDistance, movementLayer);
                if (isThereAWallInFront)
                {
                    Vector3 jumpOffset = faceDir.normalized * 0.01f;
                    CharacterJump(CharacterConstants.NormalJumpForce, Vector3.up + jumpOffset);
                    stateMachine.TransitionToState(stateMachine.airState);
                }
                else 
                {
                Vector3 adjustJumpDirection = moveDir * CharacterConstants.JumpAngle; //0.75 so that the jump angle is pointed more upwards
                CharacterJump(CharacterConstants.NormalJumpForce, (Vector3.up + adjustJumpDirection).normalized);   //little bit of an offset to make jump diagonal         
                stateMachine.TransitionToState(stateMachine.airState);
                }
            }
        }
        else if (currentSurface == Surface.Wall)
        {
            DetachFromWall();
            CharacterJump(CharacterConstants.WallJumpForce, wallHit.normal + Vector3.up);
        }
        else
        {
            StopCoroutine(nameof(BufferJump));
            StartCoroutine(nameof(BufferJump));
        }       
    }

    

    //noted under charge jump
    private void ChargeJump(float chargeJumpPower)
    {

        if (currentSurface == Surface.Wall)
        {
            DetachFromWall();
        }
        CharacterJump((chargeJumpPower * CharacterConstants.ChargeMultiplier) + CharacterConstants.NormalJumpForce, aimCameraPosition.forward);
        CharacterJump(1, Vector3.up);
    }

    //noted under character jump
    private void CharacterJump(float jumpForce, Vector3 jumpDirection)
    {   
        //print("jumpforce " + jumpForce);
        rb.linearVelocity = new(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
        SetFaceDir(Vector3.ProjectOnPlane(jumpDirection, Vector3.up).normalized);
    }


    // noted under rotate player
    private void RotatePlayer()
    {
        float rotateSpeed = 16f;
        transform.forward = Vector3.Slerp(transform.forward, faceDir, rotateSpeed * Time.fixedDeltaTime);
    }

    //noted under rotate player
    private void SetFaceDir(Vector3 direction)
    {
        faceDir = direction.normalized;
    }

    //noted under buffer jump
    private IEnumerator BufferJump()
    {
        bufferTimeCounter = CharacterConstants.BufferTime;
        while (bufferTimeCounter > 0)
        {
            if (isGrounded)
            {
                Vector3 jumpOffset = faceDir.normalized * 0.01f;
                CharacterJump(CharacterConstants.NormalJumpForce, Vector3.up + jumpOffset); //little bit of an offset to make sure wallcheck doesnt check directly upwards  
                yield break;
            }

            bufferTimeCounter -= Time.deltaTime;
            yield return null;
        }
    }

    //noted under gravity application
    //these set methods are correct just set them to public later
    public void SetGravity(float gravity)
    {
        gravityScale = gravity;
    }
    //noted under gravity application
    private void CustomGravity()
    {
        //uses gravityScale for the actual gravity value  
        rb.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
    }

    //noted under cancel jump
    private void CancelJumping()
    {
        if (currentSurface == Surface.Ground)
        {
            stateMachine.TransitionToState(stateMachine.idleState);
        }
    }


    //noted under start jump
    private void StartJumping()
    {
        if (currentSurface == Surface.Ground || currentSurface == Surface.Wall)
        {
            stateMachine.TransitionToState(stateMachine.startJumpState);
        }
        else
        {
            return;
        }
    }


    //noted under toggle wall climb
    private void ToggleWallClimb()
    {
        if (currentSurface == Surface.Wall)
        {
            DetachFromWall();
        }
        else if (wallDetected)
        {
            AttachToWall();
        }
    }

    //noted under detach from wall
    private void DetachFromWall()
    {
        //faceDir = wallHit.normal;
        stateMachine.TransitionToState(stateMachine.airState);
        SetFaceDir(wallHit.normal);
    }

    //noted under attach to wall 
    private void AttachToWall()
    {
        Vector3 checkPointOffset = wallCheckPoint.position - transform.position;
        Vector3 stickPosition = wallHit.point + wallHit.normal * charCollider.radius;

        float offset = (currentSurface == Surface.Ground) ? 1 : 0; // little upward nudge when climbing from ground to wall
        Vector3 latchTarget = stickPosition - checkPointOffset + new Vector3(0, offset, 0);

        Vector3 latchDirection = (latchTarget - rb.position).normalized;

        CharacterJump(CharacterConstants.NormalJumpForce, latchDirection);
        stateMachine.TransitionToState(stateMachine.wallState);
    }

    private void OnDrawGizmos()
    {
        if (charCollider == null) return; // safety check

        // World-space origin (the correct way)
        Vector3 worldOrigin = transform.TransformPoint(charCollider.center);

        // --- Draw WORLD space visualization ---
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(worldOrigin, CharacterConstants.FeetRadius);
        Gizmos.DrawLine(worldOrigin, worldOrigin + Vector3.down * CharacterConstants.GroundCheckDistance);
        Gizmos.DrawWireSphere(worldOrigin + Vector3.down * CharacterConstants.GroundCheckDistance, CharacterConstants.FeetRadius);

        if (wallCheckPoint == null) return;

        Vector3 origin = wallCheckPoint.position;
        Vector3 shootDir = (currentSurface == Surface.Wall) ? wallHit.normal * -1f : faceDir;
        Vector3 endPoint = origin + shootDir * CharacterConstants.WallCheckDistance;

        Gizmos.color = wallDetected ? Color.green : Color.red;
        Gizmos.DrawWireSphere(origin, CharacterConstants.WallCheckRadius);
        Gizmos.DrawLine(origin, endPoint);
        Gizmos.DrawWireSphere(endPoint, CharacterConstants.WallCheckRadius);
    }

    private void GetAllComponents()
    {
        charCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        inputManager = GetComponent<InputManager>();
        stopWatch = GetComponent<StopWatch>();
    }

    private void CursorLock()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void AssignEvents()
    {
        inputManager.GetMovementVector += value => InputDir = value;
        inputManager.NormalJump += PerformNormalJump;
        inputManager.StartJumping += StartJumping;
        inputManager.CancelJumping += CancelJumping;
        inputManager.ToggleWallClimb += ToggleWallClimb;
        stopWatch.ChargeJumpHoldTime += ChargeJump;
    }
}
