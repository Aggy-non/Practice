using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private bool chargeStarted=false;
    private Vector3 movementVector;
    private PlayerInputs playerInputs;

    public event Action<Vector3> GetMovementVector;
    public event Action NormalJump;
    public event Action ChargeStart;
    public event Action ChargeEnd;
    public event Action StartJumping;

    private void Awake()
    {
        playerInputs = new PlayerInputs();
        InputLinking();
    }
    private void Update()
    {
        // just log it for now we'll make it send event args later
        ReturnMovementVector();
    }

    private void JumpStarted(InputAction.CallbackContext context)
    {
        StartJumping?.Invoke();
    }
    private void JumpPerformed(InputAction.CallbackContext context)
    {

        if (chargeStarted)
        {
            //Release charge jump
            //To change charge jump threshold go to input system map to change it
                     
            ChargeEnd?.Invoke();
            //Debug.Log("Charge jump is performed ");
            chargeStarted = false;
        }

        else 
        {
            //normal jump
            //Debug.Log("Normal Jump " + context.phase);;
            NormalJump?.Invoke();
        }
        
    }


    private void StartJumpCharge(InputAction.CallbackContext context)
    {
        //start charging the charge jump
        chargeStarted = true;
        //Debug.Log("Charging");
        ChargeStart?.Invoke();
    }

    private void ReturnMovementVector()
    {         
        //return the direction in which player should move
        Vector2 inputVector = playerInputs.CharacterControls.Movement.ReadValue<Vector2>();
        movementVector = new(inputVector.x, 0, inputVector.y);
        
        GetMovementVector?.Invoke(movementVector);
    }

    private void CrouchOrRollInput(InputAction.CallbackContext context)
    {
        //crouch logic
    }

    private void InputLinking()
    {
        //link each input event to its method
        playerInputs.CharacterControls.Enable();
        playerInputs.CharacterControls.Jump.started += JumpStarted;
        playerInputs.CharacterControls.Jump.canceled += JumpPerformed;
        playerInputs.CharacterControls.Jump.performed += StartJumpCharge;
        playerInputs.CharacterControls.Crouch.performed += CrouchOrRollInput;         
    }
    private void OnDestroy()
    {
        // this line prevents memory leak
        playerInputs.CharacterControls.Disable();
    }
}