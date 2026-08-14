using Unity.Cinemachine;
using UnityEngine;
using System;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera zoomCamera;
    [SerializeField] private CinemachineCamera orbitCamera;
    [SerializeField] private CanvasGroup crosshairGroup;
    //[SerializeField] private bool isAim=false;

    private InputManager inputManager;

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        inputManager.ChargeStart += AimCameraActive;
        inputManager.ChargeEnd += MainCameraActive;
        SetPriority(zoomCamera, 0);
        SetPriority(orbitCamera, 10);
        crosshairGroup.alpha = 0f;
    }


    private void AimCameraActive()
    {
        SetPriority(zoomCamera, 20);
        SetPriority(orbitCamera, 10);
        crosshairGroup.alpha = 1f;
    }

    private void MainCameraActive()
    {
        SetPriority(zoomCamera, 0);
        SetPriority(orbitCamera, 10);
        crosshairGroup.alpha = 0f;
    }

    private void SetPriority(CinemachineCamera cam, int value)
    {
        cam.Priority = new PrioritySettings { Enabled = true, Value = value };
    }


}
