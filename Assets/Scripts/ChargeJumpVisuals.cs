using UnityEngine;

public class ChargeJumpVisuals : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform aimCamera;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform targetPoint;

    private InputManager inputManager;
    private StopWatch stopWatch;
    private CharacterControl characterControl;
    private Rigidbody rb;
    private LineRenderer lr;

    private bool lineExtending = false;
    private float chargeTime;
    private Vector3 currentDirection = Vector3.forward;

    [Header("Arc preview settings")]
    [SerializeField] private int maxSimSteps = 120;

    private Vector3[] arcPoints;

    private void Start()
    {
        GetComponents();
        lr.useWorldSpace = true;
        arcPoints = new Vector3[maxSimSteps];
        SubscribeEvents();
    }

    private void Update()
    {
        RotateDirection();

        if (!lineExtending)
        {
            return;
        }

        SimulateArc();
        //UpdateTargetPosition();
    }

    private void FixedUpdate()
    {
        //SimulateArc();
    }

    private void RotateDirection()
    {
        currentDirection = Vector3.Slerp(currentDirection, aimCamera.forward, 10f * Time.deltaTime).normalized;
    }

    private void SimulateArc()
    {
        float jumpForce = chargeTime * characterControl.ChargeMultiplier + characterControl.NormalJumpForce;

        Vector3 startVel = rb.linearVelocity;
        startVel.y = 0f; // matches CharacterJump zeroing vertical velocity before the impulse

        Vector3 simVel = startVel + (currentDirection * jumpForce) / rb.mass; // ForceMode.Impulse = force / mass
        Vector3 simPos = startPoint.position;

        float dragDecay = Mathf.Exp(-characterControl.ChargeJumpAirDrag * Time.fixedDeltaTime);
        float gravityAccel = Physics.gravity.y * characterControl.AirGravityScale;

        int pointCount = 0;
        arcPoints[pointCount++] = simPos;

        for (int i = 1; i < maxSimSteps; i++)
        {
            simVel.y += gravityAccel * Time.fixedDeltaTime;
            simVel.x *= dragDecay;
            simVel.z *= dragDecay;
            simPos += simVel * Time.fixedDeltaTime;

            arcPoints[pointCount++] = simPos;

            if (simPos.y <= startPoint.position.y)
            {
                break;
            }
        }

        lr.positionCount = pointCount;
        lr.SetPositions(arcPoints);
    }

    private void LineErase()
    {
        lineExtending = false;
        lr.positionCount = 0;
    }

    private void LineExtend()
    {
        lineExtending = true;
    }

    private void UpdateTargetPosition()
    {

        if (!lineExtending)
        {
            targetPoint.position = transform.position;
        }
        else
        {

        }
    }

    private void GetComponents()
    {
        stopWatch = GetComponent<StopWatch>();
        lr = GetComponent<LineRenderer>();
        inputManager = GetComponent<InputManager>();
        characterControl = GetComponent<CharacterControl>();
        rb = GetComponent<Rigidbody>();
    }

    private void SubscribeEvents()
    {
        stopWatch.StopWatchTime += value => chargeTime = value;
        inputManager.ChargeStart += LineExtend;
        inputManager.ChargeEnd += LineErase;
    }
}