using Unity.Cinemachine;
using UnityEngine;
using System;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera zoomCamera;
    [SerializeField] private CinemachineCamera orbitCamera;
    
    //[SerializeField] private bool isAim=false;

    private InputManager inputManager;

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        inputManager.ChargeStart += AimCameraActive;
        inputManager.ChargeEnd += MainCameraActive;
        SetPriority(zoomCamera, 0);
        SetPriority(orbitCamera, 10);
        
    }


    private void AimCameraActive()
    {
        SetPriority(zoomCamera, 20);
        SetPriority(orbitCamera, 10);
        
    }

    private void MainCameraActive()
    {
        SetPriority(zoomCamera, 0);
        SetPriority(orbitCamera, 10);
        
    }

    private void SetPriority(CinemachineCamera cam, int value)
    {
        cam.Priority = new PrioritySettings { Enabled = true, Value = value };
    }


}

//rotate plater to face aim camera 
