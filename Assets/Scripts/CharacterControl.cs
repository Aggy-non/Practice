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
    public Vector3 InputDir;
    private Vector3 MoveDir;

    [Header("Physics settings")]    
    [Range(0f, 1f)]
    [SerializeField] private float landDrag;
    [Range (0f, 1f)]
    [SerializeField] private float airDrag;    
    private float gravityScale=1f;
    [SerializeField] private float landGravityScale; 
    [SerializeField] private float airGravityScale;

    [Header("Jump settings")]
    [SerializeField] private float normalJumpForce;
    [SerializeField] private float bufferTime;
    private float bufferTimeCounter;

    [Header("Camera Settings")]
    [SerializeField]private Transform cameraPosition;
    private Vector3 viewDir;
    private Transform orientation;

    [Header("Charge Jump")]
    private ChargeJumpHandler chargeJumpHandler;
    

    #endregion
    private void Awake()
    {
        GetAllComponents();
        inputManager.GetMovementVector += value => InputDir = value;
        inputManager.NormalJump += PerformNormalJump;
        chargeJumpHandler.ChargeJumpPower += ChargeJump;
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
    }

    private void CalculateMoveDir()
    {
        Vector3 camForward = cameraPosition.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraPosition.right;
        camRight.y = 0;
        camRight.Normalize();

        MoveDir = camForward * InputDir.z + camRight * InputDir.x;

    }
    private void MoveCharacter()
    {
        rb.AddForce(MoveDir * moveSpeed , ForceMode.VelocityChange);
    }

    private void DragControl()
    {
        float currentDrag;
        if (isGrounded)
        {
            currentDrag = landDrag;
            gravityScale = landGravityScale;
        }
        else
        {
            currentDrag = airDrag;    
            gravityScale = airGravityScale;
        }

        //seperate setting drag and applying drag 

        Vector3 vel = rb.linearVelocity;
        vel.x *= currentDrag;
        vel.z *= currentDrag;
        rb.linearVelocity = vel;
    }

    private void GroundCheck()
    {                
        Vector3 origin = transform.TransformPoint(charCollider.center);
        isGrounded = Physics.SphereCast(origin, feetRadius, Vector3.down, out RaycastHit hitInfo, groundCheckDistance, groundLayer);               
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
            CharacterJump(normalJumpForce);
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
        CharacterJump(chargeJumpPower + normalJumpForce);
        Debug.Log("final jump power"+chargeJumpPower);
    }

    private void CharacterJump(float jumpForce)
    {
        rb.linearVelocity = new(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);
    }

    private IEnumerator BufferJump()
    {    

        //make a seperate timer utility class
       while(bufferTimeCounter>0)
        {
            if (isGrounded)
            {
                CharacterJump(normalJumpForce);
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
        chargeJumpHandler = GetComponent<ChargeJumpHandler>();
    }

    private void CursorLock()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
}
