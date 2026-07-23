using UnityEngine;

public class CCscript : MonoBehaviour
{
    [SerializeField] private CharacterController cc;

    [SerializeField] private InputManager inputManager;
    [SerializeField] private float moveSpeed;

    private void Awake()
    {
        inputManager.GetMovementVector += Movement;
    }

    private void Movement(Vector3 inputDirection)
    {
        cc.SimpleMove(inputDirection*moveSpeed);
        
    }
}
