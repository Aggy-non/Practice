using System;
using UnityEngine;

public class StopWatch : MonoBehaviour
{
    private InputManager inputManager;
    //private CharacterControl characterControl;
    private StateCharacterControl characterControl;

    public float stopwatchTime { get; private set; }

    private bool isRunning = false;
    public event Action<float> ChargeJumpHoldTime;
    public event Action<float> StopWatchTime;
    void Start()
    {
        inputManager = GetComponent<InputManager>();
        //characterControl = GetComponent<CharacterControl>();
        characterControl = GetComponent<StateCharacterControl>();
        inputManager.ChargeStart += StartStopwatch;
        inputManager.ChargeEnd += FinishStopWatch;
        inputManager.CancelJumping += CancelStopWatch;
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

    private void FinishStopWatch()
    {
        isRunning = false;
        ChargeJumpHoldTime?.Invoke(stopwatchTime);
        //print("Charge Jump Hold Time: " + stopwatchTime);
        ResetTimer();
    }

    private void CancelStopWatch()
    {
        isRunning=false;
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
