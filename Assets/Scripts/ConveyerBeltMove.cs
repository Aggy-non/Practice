using UnityEngine;

public class ConveyerBeltMove : MonoBehaviour
{
    private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            print("player detected");
        }
    }
}
