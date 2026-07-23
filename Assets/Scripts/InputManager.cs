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
    private void JumpInput(InputAction.CallbackContext context)
    {

        if (chargeStarted)
        {
            //Release charge jump
            //To change charge jump threshold go to input system map to change it
            Debug.Log("Charge jump is performed ");
            chargeStarted = false;
        }

        else 
        {
            //normal jump
            Debug.Log("Normal Jump " + context.phase);
        }
        
    }

    private void StartJumpCharge(InputAction.CallbackContext context)
    {
        //start charging the charge jump
        chargeStarted = true;
        Debug.Log("Charging");
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
        //Crouch and dodge roll logic
        if (movementVector == Vector3.zero)
        {
            //perform crouch
            Debug.Log("Crouch Pressed");

        }
        else
        {
            //perform dodge roll in the direction of the movement vector
            Debug.Log("Dodge roll is performed");
        }
    }

    private void SprintInput(InputAction.CallbackContext context)
    {
        //Sprint toggle
        Debug.Log("Sprint Pressed");
    }

    private void InputLinking()
    {
        //link each input event to its method
        playerInputs.CharacterControls.Enable();
        playerInputs.CharacterControls.Jump.canceled += JumpInput;
        playerInputs.CharacterControls.Jump.performed += StartJumpCharge;
        playerInputs.CharacterControls.Crouch.performed += CrouchOrRollInput;        
        playerInputs.CharacterControls.Sprint.performed += SprintInput;
    }
    private void OnDestroy()
    {
        // this line prevents memory leak
        playerInputs.CharacterControls.Disable();
    }
}