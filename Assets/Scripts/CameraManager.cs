using Unity.Cinemachine;
using UnityEngine;
using System;

public class CameraManager : MonoBehaviour
{

    [SerializeField] private CinemachineCamera aimCamera;
    [SerializeField] private CinemachineCamera mainCamera;
    [SerializeField] private CinemachineCamera wallCamera;

    //[SerializeField] private bool isAim=false;
    public static CameraManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // a second copy showed up, so remove it
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // optional: survive scene changes
    }
    private void Start()
    {
        MainCameraActive();
        
    }


    public void AimCameraActive()
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

