using System.Net;
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

    private float dbgJumpForce;
    private Vector3 dbgLaunchDir;
    private Vector3 dbgStartVel;
    private Vector3 dbgImpulseVel;
    private Vector3 dbgEndPoint;
    private int dbgPointCount;

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

        // the real jump fires along aimCameraPosition.forward at the instant of release,
        // so predict with the raw camera forward, not the smoothed currentDirection
        Vector3 launchDir = aimCamera.forward;

        Vector3 startVel = rb.linearVelocity;
        startVel.y = 0f;                                          // CharacterJump zeroes vertical velocity first
        Vector3 impulseVel = (launchDir * jumpForce) / rb.mass;   // ForceMode.Impulse = force / mass

        Vector3 simVel = startVel;                                // velocity BEFORE the impulse lands
        Vector3 simPos = startPoint.position;

        // minus sign matches ApplyDrag(); AirDrag because DefaultAirState is what runs after release
        float dragDecay = Mathf.Exp(-CharacterConstants.AirDrag * Time.fixedDeltaTime);
        float gravityAccel = Physics.gravity.y * CharacterConstants.AirGravityScale;

        int pointCount = 0;
        arcPoints[pointCount++] = simPos;
        bool hitSomething = false;

        for (int i = 1; i < maxSimSteps; i++)
        {
            Vector3 prevPos = simPos;

            // same order as the real FixedUpdate: ApplyDrag() runs first, then the physics step
            // adds the impulse and gravity, then position integrates
            simVel.x *= dragDecay;
            simVel.z *= dragDecay;
            if (i == 1) simVel += impulseVel;
            simVel.y += gravityAccel * Time.fixedDeltaTime;
            simPos += simVel * Time.fixedDeltaTime;

            Vector3 segment = simPos - prevPos;
            float segmentLength = segment.magnitude;

            if (segmentLength > 0.0001f)
            {
                Vector3 segmentDir = segment / segmentLength;

                if (Physics.Raycast(prevPos, segmentDir, out RaycastHit hit, segmentLength, blockingLayers))
                {
                    hitSomething = true;
                    UpdateTargetPosition(hit.normal, hit.point);
                    arcPoints[pointCount++] = hit.point - segmentDir * backoffDistance;
                    break;
                }
            }

            arcPoints[pointCount++] = simPos;
        }

        if (targetVisual.activeSelf != hitSomething)
        {
            targetVisual.SetActive(hitSomething);
        }

        lr.positionCount = pointCount;
        lr.SetPositions(arcPoints);

        // DEBUG snapshot
        dbgJumpForce = jumpForce;
        dbgLaunchDir = launchDir;
        dbgStartVel = startVel;
        dbgImpulseVel = impulseVel;
        dbgEndPoint = arcPoints[pointCount - 1];
        dbgPointCount = pointCount;
    }

    private void LineErase()
    {
        //if (lineExtending) // this also fires from CancelJumping, so only log real releases
        //{
        //    Debug.Log(
        //        $"[PREVIEW @release] chargeTime={chargeTime:F3} jumpForce={dbgJumpForce:F2}\n" +
        //        $"  dir={dbgLaunchDir} (smoothed currentDirection={currentDirection})\n" +
        //        $"  startVel={dbgStartVel} impulseVel={dbgImpulseVel}\n" +
        //        $"  dragDecay={Mathf.Exp(-CharacterConstants.AirDrag * Time.fixedDeltaTime):F4} gravityAccel={Physics.gravity.y * CharacterConstants.AirGravityScale:F2}\n" +
        //        $"  origin(startPoint)={startPoint.position} rb.position={rb.position}\n" +
        //        $"  predictedEnd={dbgEndPoint} steps={dbgPointCount}");
        //}

        targetVisual.SetActive(false);
        lineExtending = false;
        lr.positionCount = 0;
    }

    private void LineExtend()
    {
        chargeTime = 0f; // StopWatch only reports the new time next frame; don't simulate with the previous charge
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