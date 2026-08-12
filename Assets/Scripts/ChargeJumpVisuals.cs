using System.Drawing;
using UnityEngine;

public class ChargeJumpVisuals : MonoBehaviour
{
    private InputManager inputManager;
    private StopWatch stopWatch;
    private LineRenderer lr;
    private bool flagStartVisual=false;
    [SerializeField]private Transform origin;
    private Vector3 line;
    private float chargeTime;
    private Vector3 lineDirection;
    void Start()
    {
        stopWatch = GetComponent<StopWatch>();
        lr = GetComponent<LineRenderer>();
        inputManager = GetComponent<InputManager>();


        lr.positionCount = 2;
        lr.useWorldSpace = true;
        


        stopWatch.StopWatchTime += value => chargeTime = value;
        inputManager.ChargeStart += LineExtend;
        inputManager.ChargeEnd += LineErase;
    }

    private void LineErase()
    {
        flagStartVisual = false;
    }

    private void LineExtend()
    {
        flagStartVisual = true;
    }

    private void Update()
    {
        lr.SetPosition(0, origin.position);
        lineDirection = Vector3.up;
        if (flagStartVisual)
        {
            line += lineDirection * chargeTime* Time.deltaTime;
        }
        else
        {
            line=origin.position;
        }
        lr.SetPosition(1, line);
        
    }

   
}
