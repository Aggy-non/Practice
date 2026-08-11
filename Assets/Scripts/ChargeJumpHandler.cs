using System;
using UnityEngine;

public class ChargeJumpHandler : MonoBehaviour
{
    InputManager inputManager;
    private float chargeStartTime;
    [SerializeField] private int chargeMultiplier;
    public event Action<float> ChargeJumpPower;
    

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        inputManager.ChargeStart += value => chargeStartTime = value;
        inputManager.ChargeEndTime += CalculateChargeTime;
    }

    private void CalculateChargeTime(float time)
    {        
        float chargeEndTime=time;
        float TotalChargeTime = chargeEndTime - chargeStartTime;
        Debug.Log(TotalChargeTime*chargeMultiplier);
        ChargeJumpPower?.Invoke(TotalChargeTime*chargeMultiplier);
    }

}
