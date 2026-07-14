using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private bool chargeStarted=false;
    private Vector3 movementVector;
    private void Awake()
    {

    }
    private void Update()
    {
      
    }
    public void JumpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //charge state
            Debug.Log("Jump is charging");
            chargeStarted = true;
        }

        else if (context.canceled && !chargeStarted)
        {
            //normal jump
            Debug.Log("Normal Jump ");
        }

        else if (context.canceled && chargeStarted)
        {
            //Charge jump
            //To change charge jump threshold go to input system map to change it
            Debug.Log("Charge jump is performed ");
            chargeStarted = false;
        }
        
    }

    
    public void MovementInput(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        movementVector = new Vector3(inputVector.x, 0, inputVector.y);
    }

    public Vector3 ReturnMovementVector()
    {
        return movementVector;
    }
}
