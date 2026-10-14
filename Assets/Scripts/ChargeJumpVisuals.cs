using UnityEditor;
using UnityEngine;

public class ChargeJumpVisuals : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform aimCamera;
    [SerializeField] private Transform startPoint;

    private InputManager inputManager;
    private StopWatch stopWatch;
    //private CharacterControl characterControl;
    //private StateCharacterControl characterControl;
    private Rigidbody rb;
    private LineRenderer lr;

    private bool lineExtending = false;
    private float chargeTime;
    private Vector3 currentDirection = Vector3.forward;

    [Header("Arc preview settings")]
    [SerializeField] private int maxSimSteps = 120;
    private Vector3[] arcPoints;

    [Header("Collision settings")]
    [SerializeField] private LayerMask blockingLayers; 
    [SerializeField] private float backoffDistance = 0.1f;
    [SerializeField] private GameObject targetVisual;

    private void Start()
    {
        GetComponents();
        lr.useWorldSpace = true;
        arcPoints = new Vector3[maxSimSteps];
        SubscribeEvents();
        targetVisual=Instantiate(targetVisual);
        targetVisual.SetActive(false);
    }

    private void Update()
    {
        RotateDirection();
        //UpdateTargetPosition(rotatio, positio);

        if (!lineExtending)
        {
            return;
        }
        else
        {
            SimulateArc();
        }
        //UpdateTargetPosition();
    }


    private void RotateDirection()
    {
        currentDirection = Vector3.Slerp(currentDirection, aimCamera.forward, 10f * Time.deltaTime).normalized;
    }

    private void SimulateArc()
    {
        float jumpForce = (chargeTime * CharacterConstants.ChargeMultiplier) + CharacterConstants.NormalJumpForce;

        Vector3 startVel = rb.linearVelocity; // shift the rb linear velocity to a new class to avoid errors
        startVel.y = 0f;

        Vector3 simVel = startVel + (currentDirection * jumpForce) / rb.mass;
        Vector3 simPos = startPoint.position;

        float dragDecay = Mathf.Exp(CharacterConstants.ChargeJumpAirDrag * Time.fixedDeltaTime);
        float gravityAccel = Physics.gravity.y * CharacterConstants.AirGravityScale;

        int pointCount = 0;
        arcPoints[pointCount++] = simPos;

        for (int i = 1; i < maxSimSteps; i++)
        {
            Vector3 prevPos = simPos;

            simVel.y += gravityAccel * Time.fixedDeltaTime;
            simVel.x *= dragDecay;
            simVel.z *= dragDecay;
            simPos += simVel * Time.fixedDeltaTime;

            Vector3 segment = simPos - prevPos;
            float segmentLength = segment.magnitude;

            if (segmentLength > 0.0001f)
            {
                Vector3 segmentDir = segment / segmentLength;

                if (Physics.Raycast(prevPos, segmentDir, out RaycastHit hit, segmentLength, blockingLayers))
                {
                    targetVisual.SetActive(true);
                    Vector3 targetNormal = hit.normal;
                    Vector3 targetLocation = hit.point;
                    UpdateTargetPosition(hit.normal, hit.point);
                    arcPoints[pointCount++] = hit.point - segmentDir * backoffDistance;
                    break;
                }
                else
                {
                    targetVisual.SetActive(false);
                }
            }

            arcPoints[pointCount++] = simPos;           
        }

        lr.positionCount = pointCount;
        lr.SetPositions(arcPoints);
    }

    private void LineErase()
    {
        targetVisual.SetActive(false);
        lineExtending = false;
        lr.positionCount = 0;
    }

    private void LineExtend()
    {
        lineExtending = true;
    }

    private void UpdateTargetPosition(Vector3 normal, Vector3 position)
    {
        targetVisual.transform.SetPositionAndRotation(position, Quaternion.FromToRotation(Vector3.up, normal));
        //Debug.Log("normal:" + normal +"position:" + position);
    }

    private void GetComponents()
    {
        stopWatch = GetComponent<StopWatch>();
        lr = GetComponent<LineRenderer>();
        inputManager = GetComponent<InputManager>();
        //characterControl = GetComponent<CharacterControl>();
        //characterControl = GetComponent<StateCharacterControl>();
        rb = GetComponent<Rigidbody>();
    }

    private void SubscribeEvents()
    {
        stopWatch.StopWatchTime += value => chargeTime = value;
        inputManager.ChargeStart += LineExtend;
        inputManager.ChargeEnd += LineErase;
        inputManager.CancelJumping += LineErase;
    }
}