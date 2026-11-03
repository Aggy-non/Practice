using UnityEngine;

public class Grapple : MonoBehaviour
{
    public GameObject activationMesh;

    public void activateGrappleVisual()
    {
        activationMesh.SetActive(true);
    }
}
