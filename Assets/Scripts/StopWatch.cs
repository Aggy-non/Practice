using System;
using UnityEngine;

public class StopWatch : MonoBehaviour
{
    private InputManager inputManager;

    public float stopwatchTime { get; private set; }

    private bool isRunning = false;
    public event Action<float> ChargeJumpHoldTime;
    public event Action<float> StopWatchTime;
    void Start()
    {
        inputManager = GetComponent<InputManager>();
        inputManager.ChargeStart += StartStopwatch;
        inputManager.ChargeEnd += StopStopWatch;
    }

    void Update()
    {
        RunTimer();
    }

    private void StartStopwatch()
    {
        ResetTimer();
        isRunning = true;
    }

    private void StopStopWatch()
    {
        isRunning = false;
        ChargeJumpHoldTime?.Invoke(stopwatchTime);
        ResetTimer();
    }
    
    private void RunTimer()
    {
        if (isRunning)
        {
            stopwatchTime += Time.deltaTime;           
            StopWatchTime?.Invoke(stopwatchTime);
        }
    }

    private void ResetTimer()
    {
        isRunning = false;        
        stopwatchTime = 0;
    }
}
