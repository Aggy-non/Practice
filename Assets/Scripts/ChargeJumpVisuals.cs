using UnityEngine;

public class ChargeJumpVisuals : MonoBehaviour
{
    private InputManager inputManager;
    private bool startVisualFlag=false;
    private Transform origin;
    void Start()
    {
        inputManager = GetComponent<InputManager>();
        inputManager.ChargeStart += VisualsStart;
    }

    private void Update()
    {
        if(startVisualFlag)
        {

        }
    }

    private void VisualsStart(float obj)
    {
        startVisualFlag=true;
    }

    private void ExtendLine()
    {

    }
}
