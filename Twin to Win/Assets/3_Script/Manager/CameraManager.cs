using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    [SerializeField] private GameObject objCamera;
    private CinemachineVirtualCamera cMainCam;

    private void Awake()
    {
        instance = this;
        GameObject objMainCamera = Instantiate(objCamera);
        cMainCam = objMainCamera.GetComponent<CinemachineVirtualCamera>();
    }
    private void Start()
    {
        ResetCamera();
    }
    public void ResetCamera() 
    {
        cMainCam.Follow = Player.instance.cCurrentCharacter.transform;
    }
}
