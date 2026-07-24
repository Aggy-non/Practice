using Unity.VisualScripting;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{          
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

    [Header("Physics settings")]
    [SerializeField] private float landDrag;
    [SerializeField] private float airDrag;


    [Header("Jump settings")]
    [SerializeField] private float jumpForce;
    private bool revertGroundCheck;

    private void Awake()
    {
        GetAllComponents();  
        inputManager.GetMovementVector += MoveCharacter;
        inputManager.NormalJump += PerformNormalJump;
    }
    
    private void Start()
    {
        
    }
           
    private void Update()
    {
        DragControl();
        GroundCheck();
        rb.maxLinearVelocity = maxSpeed;
        
    }

    private void MoveCharacter(Vector3 InputVector)
    {
        rb.AddForce(InputVector * moveSpeed, ForceMode.VelocityChange);
    }

    private void DragControl()
    {
        //speed limiter
        
        if (isGrounded)
        {
            //Ground Drag
            //rb.linearDamping = landDrag;
        }
        else
        {
            //Air Drag
            //rb.linearDamping = airDrag;
        }
    }

    private void GroundCheck()
    {                
        Vector3 origin = transform.TransformPoint(charCollider.center);
        isGrounded = Physics.SphereCast(origin, feetRadius, Vector3.down, out RaycastHit hitInfo, groundCheckDistance, groundLayer);// && revertGroundCheck;                
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
        //revertGroundCheck = false;
        if (!isGrounded) return;
        rb.linearVelocity = new(rb.linearVelocity.x, 0, rb.linearVelocity.z); 
        rb.AddForce(0,jumpForce,0,ForceMode.Impulse);
        
    }

    private void GetAllComponents()
    {
        charCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        inputManager = GetComponent<InputManager>();
    }
}
