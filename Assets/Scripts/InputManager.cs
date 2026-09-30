using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private bool chargeStarted=false;
    private bool cancelJump=false;
    private Vector3 movementVector;
    private PlayerInputs playerInputs;
    private StateCharacterControl characterControl;
    private IState currentState;

    public event Action<Vector3> GetMovementVector;
    public event Action NormalJump;
    public event Action ChargeStart;
    public event Action ChargeEnd;
    public event Action StartJumping;
    public event Action CancelJumping;
    public event Action ToggleWallClimb;

    private void Awake()
    {
        playerInputs = new PlayerInputs();
        InputLinking();
    }
    private void Start()
    {
        characterControl = GetComponent<StateCharacterControl>();
    }
    private void Update()
    {
        ReturnMovementVector();
    }

    private void JumpStarted(InputAction.CallbackContext context)
    {
        StartJumping?.Invoke();        
    }

    private void CancelChargeJump(InputAction.CallbackContext obj)
    {
        if (chargeStarted)
        {
            chargeStarted = false;
            CancelJumping?.Invoke();
            cancelJump = true;
            //Debug.Log("Cancelled");
        }        
    }

    private void InitiateWallClimb(InputAction.CallbackContext obj)
    {
        ToggleWallClimb?.Invoke();
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
            //Debug.Log("Normal Jump " + context.phase);
            if (cancelJump)
            {
                cancelJump = false;
                return;                
            }
            else
            {
                NormalJump?.Invoke();
            }
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
        //movementVector = new(inputVector.x, 0, inputVector.y);
        GetMovementVector?.Invoke(inputVector);
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
        playerInputs.CharacterControls.ChargeJumpCancel.performed += CancelChargeJump;
        playerInputs.CharacterControls.InitiateWallClimb.performed += InitiateWallClimb;
    }

    

    private void OnDestroy()
    {
        // this line prevents memory leak
        playerInputs.CharacterControls.Disable();
    }
}