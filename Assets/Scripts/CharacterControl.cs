using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    #region fields
    [Header("Components")]
    private InputManager inputManager;
    private CapsuleCollider charCollider;
    private Rigidbody rb;

    [Header("Ground check settings")]
    [SerializeField] private float feetRadius;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded = true;

    [Header("Movement settings")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float moveSpeed;
    [Range(0f, 1f)]
    [SerializeField] private float chargeJumpMoveSpeed;
    private Vector3 InputDir;
    private Vector3 MoveDir;

    [Header("Physics settings")]    
    [Range(0f, 1f)]
    [SerializeField] private float landDrag;
    [Range (0f, 1f)]
    [SerializeField] private float airDrag;
    [Range(0f, 1f)]
    [SerializeField] private float chargeJumpAirDrag;
    private float gravityScale=1f;
    [SerializeField] private float landGravityScale; 
    [SerializeField] private float airGravityScale;

    [Header("Jump settings")]
    [SerializeField] private float normalJumpForce;
    [SerializeField] private float bufferTime;
    private float bufferTimeCounter;
    private enum LastJumpType { None, Normal, Charge }
    private LastJumpType lastJumpType = LastJumpType.None;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private Transform aimCameraPosition;
    private Vector3 viewDir;
    private Transform orientation;

    [Header("Charge Jump")]
    [SerializeField] private int chargeMultiplier;
    private StopWatch stopWatch;
    

    #endregion
    private void Awake()
    {
        GetAllComponents();
        AssignEvents();
    }
    
    private void Start()
    {
        CursorLock();
        //rb.maxLinearVelocity = maxSpeed;
    }

    private void Update()
    {        
        GroundCheck();
        CalculateMoveDir();
        
    }

    private void FixedUpdate()
    {
        //make a seperate class for these 2
        CustomGravity();
        DragControl();
        MoveCharacter();
        RotatePlayer();
    }

    private void CalculateMoveDir()
    {
        Vector3 camForward = Vector3.ProjectOnPlane(cameraPosition.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cameraPosition.right, Vector3.up).normalized;

        MoveDir = camForward * InputDir.z + camRight * InputDir.x;
    }
    private void MoveCharacter()
    {
        float multiplier;
        if (lastJumpType == LastJumpType.Charge)
        {
            multiplier=chargeJumpMoveSpeed;
        }
        else
        {
            multiplier = 1;
        }

        rb.AddForce(MoveDir * moveSpeed* multiplier, ForceMode.VelocityChange);
    }

    private void DragControl()
    {
        if (isGrounded)
        {
            gravityScale = landGravityScale;
            ApplyDrag(landDrag);
        }
        else
        {   
            gravityScale = airGravityScale;
            float drag = (lastJumpType == LastJumpType.Charge) ? chargeJumpAirDrag : airDrag;
            ApplyDrag(drag);
        }

        //seperate setting drag and applying drag       
    }

    private void ApplyDrag(float drag)
    {
        Vector3 vel = rb.linearVelocity;
        vel.x *= drag;
        vel.z *= drag;
        rb.linearVelocity = vel;
    }

    private void GroundCheck()
    {
        bool wasGrounded = isGrounded;
        Vector3 origin = transform.TransformPoint(charCollider.center);
        isGrounded = Physics.SphereCast(origin, feetRadius, Vector3.down, out RaycastHit hitInfo, groundCheckDistance, groundLayer);

        if (!wasGrounded && isGrounded)
        {
            lastJumpType = LastJumpType.None; // just landed
        }
    }


    private void OnDrawGizmos()
    {
        if (charCollider == null) return; // safety check
      
        // World-space origin (the correct way)
        Vector3 worldOrigin = transform.TransformPoint(charCollider.center);


        // --- Draw WORLD space visualization ---
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(worldOrigin, feetRadius);
        Gizmos.DrawLine(worldOrigin, worldOrigin + Vector3.down * groundCheckDistance);
        Gizmos.DrawWireSphere(worldOrigin + Vector3.down * groundCheckDistance, feetRadius);
    }

    private void PerformNormalJump()
    {
       
        if (isGrounded)
        {
            lastJumpType = LastJumpType.Normal;
            CharacterJump(normalJumpForce,Vector3.up);
        }
        else
        {
            StopCoroutine(nameof(BufferJump));
            bufferTimeCounter = bufferTime;            
            StartCoroutine(nameof(BufferJump));
        }
    }

    private void ChargeJump(float chargeJumpPower)
    {
        lastJumpType = LastJumpType.Charge;
        CharacterJump((chargeJumpPower*chargeMultiplier) + normalJumpForce,aimCameraPosition.forward);
        //Debug.Log("final jump power"+chargeJumpPower);
    }

    private void CharacterJump(float jumpForce,Vector3 jumpDirection)
    {
        rb.linearVelocity = new(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(jumpDirection*jumpForce, ForceMode.Impulse);
    }

    private void RotatePlayer()
    {
        float rotateSpeed = 10f;
        if (InputDir != Vector3.zero)
        {
            transform.forward=Vector3.Slerp(transform.forward, MoveDir, rotateSpeed*Time.deltaTime);
        }
    }

    private IEnumerator BufferJump()
    {    

        //make a seperate timer utility class
       while(bufferTimeCounter>0)
        {
            if (isGrounded)
            {
                lastJumpType = LastJumpType.Normal;
                CharacterJump(normalJumpForce, Vector3.up);
                yield break;
            }

            bufferTimeCounter-= Time.deltaTime;
            yield return null;
            
        }
    }

    private void CustomGravity()
    {
        rb.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);     
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
        stopWatch.ChargeJumpHoldTime += ChargeJump;
    }
    
}
