using Unity.Cinemachine;
using UnityEngine;
using System;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera aimCamera;
    [SerializeField] private CinemachineCamera mainCamera;
    [SerializeField] private CinemachineCamera wallCamera;
    
    //[SerializeField] private bool isAim=false;

    private InputManager inputManager;
    private CharacterControl characterControl;
    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        inputManager.ChargeStart += AimCameraActive;
        inputManager.ChargeEnd += MainCameraActive;
        inputManager.CancelJumping += MainCameraActive;
        MainCameraActive();
        
    }


    private void AimCameraActive()
    {
        SetPriority(aimCamera, 20);
        SetPriority(mainCamera, 10);
        SetPriority(wallCamera, 0);
    }

    public void MainCameraActive()
    {
        SetPriority(aimCamera, 0);
        SetPriority(mainCamera, 20);
        SetPriority(wallCamera, 10);
    }

    public void WallCameraActive()
    {
        SetPriority(aimCamera, 0);
        SetPriority(mainCamera, 10);
        SetPriority(wallCamera, 20);
    }

    private void SetPriority(CinemachineCamera cam, int value)
    {
        cam.Priority = new PrioritySettings { Enabled = true, Value = value };
    }


}

//rotate plater to face aim camera 
