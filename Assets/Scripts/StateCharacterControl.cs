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
    private bool isGrounded = true;

    [Header("Movement settings")]
    private Vector3 InputDir;
    private Vector3 moveDir;
    private Vector3 faceDir;
    public Vector3 FaceDir => faceDir;
    private float multiplier;

    [Header("Physics settings")]
    [Range(0f, 100f)]
    [SerializeField] private float landDrag;
    [Range(0f, 100f)]
    [SerializeField] private float airDrag;
    [Range(0f, 100f)]
    [SerializeField] private float wallDrag;
    [Range(0f, 100f)]
    [SerializeField] private float chargeJumpAirDrag;
    public float ChargeJumpAirDrag => chargeJumpAirDrag;
    private float gravityScale = 1f;
    private float currentDrag;
    [SerializeField] private float wallGravityScale;
    [SerializeField] private float landGravityScale;
    [SerializeField] private float airGravityScale;
    public float AirGravityScale => airGravityScale;

    [Header("Jump settings")]
    [SerializeField] private float normalJumpForce;
    [SerializeField] private float wallJumpForce;
    public float NormalJumpForce => normalJumpForce;
    [SerializeField] private float bufferTime;
    private float bufferTimeCounter;
    //private bool jumpStart;
    private enum LastJumpType { None, Normal, Charge }
    private LastJumpType lastJumpType = LastJumpType.None;

    [Header("Camera Settings")]
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private Transform aimCameraPosition;
    [SerializeField] private CinemachineCamera aimCamera;
    [SerializeField] private Camera mainCamera;
    private Vector3 viewDir;
    private Transform orientation;

    [Header("Charge Jump")]
    [SerializeField] private int chargeMultiplier;
    public int ChargeMultiplier => chargeMultiplier;
    private StopWatch stopWatch;
    private bool IsChargeJumpActive => currentSurface == Surface.Air && lastJumpType == LastJumpType.Charge;

    [Header("Wall Climb")]
    [SerializeField] Transform wallCheckPoint;
    [SerializeField] float wallCheckDistance;
    [SerializeField] float wallCheckRadius;
    [SerializeField] private float wallClearance = 0.05f;
    [SerializeField] private float snapSharpness = 20f;
    private bool wallDetected;
    public enum Surface { Wall, Ground, Air }
    private Surface currentSurface = Surface.Air;
    private bool prevGroundedState = true;
    private RaycastHit wallHit;

    //State Machine
    private CharacterStateMachine stateMachine;
    public CharacterStateMachine StateMachine => stateMachine;
    private IState currentState;
    public IState CurrentState => currentState;


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
        //rb.maxLinearVelocity = maxSpeed;
        stateMachine.Initialize(stateMachine.idleState);
    }

    private void Update()
    {
        GroundCheck();
        CalculateCurrentMoveDir();
        WallCheck();
        stateMachine.Exectue();
        //print(rb.linearVelocity);
        //
    }

    private void FixedUpdate()
    {
        //make a seperate class for these gravity and drag
        CustomGravity();
        UpdatePhysicsState();
        MoveCharacter();
        RotatePlayer();
        ApplyDrag();
        //print("wallcheck: " + isWallClimbing + " groundcheck: " + isGrounded);
        //print(" surface: " + currentSurface);
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
    //    if (jumpStart && isGrounded)
    //    {
    //        multiplier = 0;
    //        rb.linearVelocity = Vector3.zero;
    //    }
    //    else
    //    {
    //        multiplier = IsChargeJumpActive ? chargeJumpMoveSpeed : 1f;
    //    }


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
        Vector3 desiredCheckPoint = wallHit.point + wallHit.normal * (charCollider.radius + wallClearance);
        Vector3 target = desiredCheckPoint - checkPointOffset;

        // only the into/out-of-wall component - leaves along-wall movement alone
        Vector3 error = Vector3.Project(target - rb.position, wallHit.normal);

        float t = 1f - Mathf.Exp(-snapSharpness * Time.fixedDeltaTime);
        rb.position += error * t;

        // kill velocity pushing into or off the wall, keep velocity along it
        rb.linearVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, wallHit.normal);
    }

    //noted under physics handling
    private void UpdatePhysicsState()
    {
        bool justLanded = isGrounded && !prevGroundedState;

        if (currentSurface == Surface.Wall)
        {
            if (justLanded)
            {
                DetachFromWall();
                currentSurface = Surface.Ground; // climbed down and touched the floor
            }
            else if (!wallDetected)
            {
                currentSurface = Surface.Air; // climbed off the wall's edge
            }
            // else: stay on the wall - entry/exit while touching is button-driven now, not automatic
        }
        else if (isGrounded)
        {
            currentSurface = Surface.Ground;
        }
        else
        {
            currentSurface = Surface.Air;
        }

        switch (currentSurface)
        {
            case Surface.Ground:
                gravityScale = landGravityScale;
                ApplyDrag(landDrag);
                break;
            case Surface.Wall:
                gravityScale = wallGravityScale;
                lastJumpType = LastJumpType.None;
                ApplyDrag(wallDrag);
                cameraManager.WallCameraActive();
                break;
            case Surface.Air:
                //jumpStart = false;
                gravityScale = airGravityScale;
                float drag = (lastJumpType == LastJumpType.Charge) ? chargeJumpAirDrag : airDrag;
                ApplyDrag(drag);
                break;
        }


        prevGroundedState = isGrounded;
    }

    public void SetSurfaceType(Surface surface)
    {
        currentSurface = surface;
    }

    //noted under drag application
    private void ApplyDrag(float drag)
    {
        //delete this when refactoring is done
        Vector3 vel = rb.linearVelocity;
        float dampFactor = Mathf.Exp(-drag * Time.fixedDeltaTime);
        if (currentSurface == Surface.Wall)
        {
            vel.y *= dampFactor;
        }
        vel.x *= dampFactor;
        vel.z *= dampFactor;
        //Change Drag systems to accomodate Y axis as well
        rb.linearVelocity = vel;
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
        bool wasGrounded = isGrounded;
        Vector3 origin = transform.TransformPoint(charCollider.center);
        isGrounded = Physics.SphereCast(origin, CharacterConstants.FeetRadius, Vector3.down, out RaycastHit hitInfo, CharacterConstants.GroundCheckDistance, movementLayer);

        //print("Was Grouned : " + wasGrounded + " Is Grounded : " + isGrounded);

        if (!wasGrounded && isGrounded)
        {
            lastJumpType = LastJumpType.None; // just landed
        }
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
        wallDetected = Physics.SphereCast(origin, wallCheckRadius, shootDir, out wallHit, wallCheckDistance, movementLayer);
    }

    //noted under normal jump
    private void PerformNormalJump()
    {

        if (currentSurface == Surface.Ground)
        {

            lastJumpType = LastJumpType.Normal;
            Vector3 jumpOffset = faceDir.normalized * 0.01f;
            CharacterJump(normalJumpForce, Vector3.up + jumpOffset);   //little bit of an offset to make sure wallcheck doesnt check directly upwards         
        }
        else if (currentSurface == Surface.Wall)
        {
            lastJumpType = LastJumpType.Normal;
            DetachFromWall();
            float upwardsOffset = 1f;
            CharacterJump(wallJumpForce, wallHit.normal + new Vector3(0, upwardsOffset, 0));
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
        lastJumpType = LastJumpType.Charge;
        CharacterJump((chargeJumpPower * chargeMultiplier) + normalJumpForce, aimCameraPosition.forward);

    }

    //noted under character jump
    private void CharacterJump(float jumpForce, Vector3 jumpDirection)
    {
        //jumpStart = false;
        rb.linearVelocity = new(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
        //faceDir = Vector3.ProjectOnPlane(jumpDirection, Vector3.up).normalized;
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
        bufferTimeCounter = bufferTime;
        while (bufferTimeCounter > 0)
        {
            if (isGrounded)
            {
                lastJumpType = LastJumpType.Normal;
                Vector3 jumpOffset = faceDir.normalized * 0.01f;
                CharacterJump(normalJumpForce, Vector3.up + jumpOffset); //little bit of an offset to make sure wallcheck doesnt check directly upwards  
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
        //jumpStart = false;
        //switch to previous state if jump is cancelled
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
        SetFaceDir(wallHit.normal);
        currentSurface = Surface.Air;
        cameraManager.MainCameraActive();
    }

    //noted under attach to wall 
    private void AttachToWall()
    {
        //faceDir = -wallHit.normal;
        SetFaceDir(-wallHit.normal);

        Vector3 checkPointOffset = wallCheckPoint.position - transform.position;
        Vector3 stickPosition = wallHit.point + wallHit.normal * charCollider.radius;

        float offset = (currentSurface == Surface.Ground) ? 1 : 0; // to amke player move a little bit up when climbing from ground to wall
        Vector3 position = stickPosition - checkPointOffset + new Vector3(0, offset, 0);
        float stickTime = 1 * Time.deltaTime;
        rb.position = Vector3.MoveTowards(rb.position, position, stickTime);
        rb.linearVelocity = Vector3.zero;
        currentSurface = Surface.Wall;
        //print("surface: " + currentSurface + " offset: " + offset);
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
        Vector3 endPoint = origin + shootDir * wallCheckDistance;

        Gizmos.color = wallDetected ? Color.green : Color.red;
        Gizmos.DrawWireSphere(origin, wallCheckRadius);
        Gizmos.DrawLine(origin, endPoint);
        Gizmos.DrawWireSphere(endPoint, wallCheckRadius);
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
        stateMachine.OnStateChanged += value => currentState = value;
    }
}
