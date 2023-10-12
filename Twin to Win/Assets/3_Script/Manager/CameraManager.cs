using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    [SerializeField] private GameObject objCamera;
    private CinemachineVirtualCamera cMainCam;
    private CinemachineBrain brainCam;

    [SerializeField] private GameObject WTD_tutorialCam;
    private List<GameObject> cams = new List<GameObject>();

    [SerializeField] private Volume volume;

    private void Awake()
    {
        instance = this;
        GameObject objMainCamera = Instantiate(objCamera);
        cMainCam = objMainCamera.GetComponent<CinemachineVirtualCamera>();

        brainCam = Camera.main.GetComponent<CinemachineBrain>();

        cams.Add(WTD_tutorialCam);

        foreach (GameObject cam in cams) 
            cam.SetActive(false); 
    }
    private void Start()
    {
        ResetCamera();
    }
    public void ResetCamera() 
    {
        cMainCam.Follow = Player.instance.cCurrentCharacter.transform;
    }
    public void OnCamActive(CamType type, float camMoveTime = 1f)
    {
        brainCam.m_DefaultBlend.m_Time = camMoveTime;

        foreach (GameObject cam in cams) cam.SetActive(false);

        switch (type)
        {
            case CamType.WTD_tutorial: WTD_tutorialCam.SetActive(true); break;
        }
    }
    public void OffCamActive() 
    {
        foreach (GameObject cam in cams) cam.SetActive(false);
    }
    public void OnPlayerDie() 
    {
        StartCoroutine(ZoomInPlayer());
    }
    private IEnumerator ZoomInPlayer() 
    {
        float time = 5f;
        float runTime = 0;
        float startDist = cMainCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance;
        float targetDist = 3f;
        float moveDist = startDist - targetDist;
		while (runTime < time)
		{
            runTime += Time.deltaTime;
            cMainCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = startDist - moveDist * runTime / time;
            yield return null;
		}
    }
    public void Vignette(bool active) 
    {
        
    }
}
public enum CamType 
{
    WTD_tutorial
}