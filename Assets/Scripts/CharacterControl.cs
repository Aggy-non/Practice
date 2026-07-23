using Unity.VisualScripting;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dragSpeed;

    private bool isGrounded=true;

    private void Awake()
    {
        inputManager.GetMovementVector += MoveCharacter;
    }

    private void MoveCharacter(Vector3 InputVector)
    {
        rb.AddForce(InputVector * moveSpeed, ForceMode.VelocityChange);
    }

    private void Update()
    {
        if (isGrounded)
        {
            rb.linearDamping = dragSpeed;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }
}
