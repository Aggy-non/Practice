using System.Collections;
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
    [Range(0f, 1f)]
    [SerializeField] private float landDrag;
    [Range (0f, 1f)]
    [SerializeField] private float airDrag;
    private float currentDrag;
    [SerializeField] private float gravityScale; 


    [Header("Jump settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float bufferTime;
    private float bufferTimeCounter;
    private bool revertGroundCheck;

    private void Awake()
    {
        GetAllComponents();  
        inputManager.GetMovementVector += MoveCharacter;
        inputManager.NormalJump += PerformNormalJump;
    }
    
    private void Start()
    {
        rb.maxLinearVelocity = maxSpeed;
    }

    private void Update()
    {        
        GroundCheck();
    }

    private void FixedUpdate()
    {
        CustomGravity();
        DragControl();
    }
    private void MoveCharacter(Vector3 InputVector)
    {
        rb.AddForce(InputVector * moveSpeed , ForceMode.VelocityChange);
    }

    private void DragControl()
    {
        float currentDrag;
        if (isGrounded)
        {
            currentDrag = landDrag;
        }
        else
        {
            currentDrag = airDrag;    
        }
        Vector3 vel = rb.linearVelocity;
        vel.x *= currentDrag;
        vel.z *= currentDrag;
        rb.linearVelocity = vel;
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
        if (isGrounded)
        {
            NormalJump();
        }
        else
        {
            bufferTimeCounter = bufferTime;
            StartCoroutine(BufferJump());
        }
    }

    private void NormalJump()
    {
        rb.linearVelocity = new(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);
    }

    private IEnumerator BufferJump()
    {    
       while(bufferTimeCounter>0)
        {
            if (isGrounded)
            {
                NormalJump();
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
    }

    
}
