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

    [SerializeField] private CinemachineVirtualCamera WTD_tutorialCam;
    [SerializeField] private CinemachineVirtualCamera PlayerDieCam;
    private List<CinemachineVirtualCamera> cams = new List<CinemachineVirtualCamera>();
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        cams.Add(WTD_tutorialCam);
        cams.Add(PlayerDieCam);
        SetTitle();
    }
    public void SetTitle() 
    {
        OffCamActive();
    }
    public void SetGame() 
    {
        GameObject objMainCamera = Instantiate(objCamera);
        cMainCam = objMainCamera.GetComponent<CinemachineVirtualCamera>();
        brainCam = Camera.main.GetComponent<CinemachineBrain>();

        OffCamActive();
        ResetCamera();

        print("카메라 끄고, 세팅");
    }
    public void ResetCamera() 
    {
        cMainCam.Follow = Player.instance.cCurrentCharacter.transform;
    }
    public void OnCamActive(CamType type, float camMoveTime = 1f)
    {
        brainCam.m_DefaultBlend.m_Time = camMoveTime;

        foreach (CinemachineVirtualCamera cam in cams) cam.gameObject.SetActive(false);

        switch (type)
        {
            case CamType.WTD_tutorial: 
                WTD_tutorialCam.gameObject.SetActive(true); 
                break;
            case CamType.PlayerDie: 
                PlayerDieCam.gameObject.SetActive(true); 
                PlayerDieCam.transform.position = Player.instance.cCurrentCharacter.transform.position;
                PlayerDieCam.m_LookAt = Player.instance.cCurrentCharacter.transform;
                PlayerDieCam.m_Follow = Player.instance.cCurrentCharacter.transform;
                break;
        }
    }
    public void OffCamActive() 
    {
        foreach (CinemachineVirtualCamera cam in cams) cam.gameObject.SetActive(false);
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
}
public enum CamType 
{
    WTD_tutorial, PlayerDie, FirstPersonPerspective
}