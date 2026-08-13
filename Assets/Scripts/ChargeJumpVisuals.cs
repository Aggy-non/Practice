using UnityEngine;

public class ChargeJumpVisuals : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform aimCamera;
    
    private InputManager inputManager;
    private StopWatch stopWatch;
    private LineRenderer lr;


    private bool lineExtending=false;


    [SerializeField] private Transform targetPoint;
    [SerializeField]private Transform startPoint;
    private Vector3 line;
    private float chargeTime;
    private Vector3 lineDirection;


    private Vector3 currentDirection = Vector3.forward;
    private float currentLength;


    void Start()
    {
        GetComponents();
        

        lr.positionCount = 2;
        lr.useWorldSpace = true;
        

        SubscribeEvents();
        
    }

    private void LineErase()
    {
        lineExtending = false;
    }

    private void LineExtend()
    {
        lineExtending = true;
    }

    private void Update()
    {        
        CalculateLineDirection();
        RotatePoint();
        lr.SetPosition(0, startPoint.position);
                
        currentLength = lineExtending
        ? currentLength + chargeTime * Time.deltaTime
        : 0f;

        Vector3 tip = startPoint.position + currentDirection * currentLength;
        lr.SetPosition(1, tip);
    }

    private void RotatePoint()
    {        
        currentDirection = Vector3.Slerp(currentDirection, lineDirection, 10f * Time.deltaTime).normalized;
    }
    private void CalculateLineDirection()
    {
        lineDirection = aimCamera.forward;
    }
    private void GetComponents()
    {
        stopWatch = GetComponent<StopWatch>();
        lr = GetComponent<LineRenderer>();
        inputManager = GetComponent<InputManager>();
    }
   
    private void SubscribeEvents()
    {
        stopWatch.StopWatchTime += value => chargeTime = value;
        inputManager.ChargeStart += LineExtend;
        inputManager.ChargeEnd += LineErase;
    }
}
