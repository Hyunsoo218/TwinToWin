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
    private Coroutine shakeCamera;

    [SerializeField] private CinemachineVirtualCamera WTD_tutorialCam;
    [SerializeField] private CinemachineVirtualCamera PlayerDieCam;
    private List<CinemachineVirtualCamera> cams = new List<CinemachineVirtualCamera>();
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        cams.Add(WTD_tutorialCam);
        cams.Add(PlayerDieCam);
    }
	private void Start() => SetTitle(); 
	public void SetTitle() => OffCamActive(); 
    public void SetGame() 
    {
        GameObject objMainCamera = Instantiate(objCamera);
        cMainCam = objMainCamera.GetComponent<CinemachineVirtualCamera>();
        brainCam = Camera.main.GetComponent<CinemachineBrain>();

        brainCam.m_DefaultBlend.m_Time = 0;
        OffCamActive();
        ResetCamera();
    }
    public void SetDefaultBlend(float blend) 
    {
        brainCam.m_DefaultBlend.m_Time = blend;
    }
    public void ResetCamera() => cMainCam.Follow = Player.Instance.CurrentCharacter.transform; 
    public void OnCamActive(CamType type, float camMoveTime = 0)
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
                PlayerDieCam.transform.position = Player.Instance.CurrentCharacter.transform.position;
                PlayerDieCam.m_LookAt = Player.Instance.CurrentCharacter.transform;
                PlayerDieCam.m_Follow = Player.Instance.CurrentCharacter.transform;
                break;
        }
    }
    public void OffCamActive() 
    {
        foreach (CinemachineVirtualCamera cam in cams) cam.gameObject.SetActive(false);
        if(GameManager.instance.gameStage == GameStage.Game)
            StartCoroutine(ResetBrainCamBlendTime());
    }
    private IEnumerator ResetBrainCamBlendTime() 
    {
        yield return new WaitForSeconds(brainCam.m_DefaultBlend.m_Time);
        brainCam.m_DefaultBlend.m_Time = 0;
    }
    public void ShakeCamera(float power, float time) 
    {
        if (shakeCamera != null)
            StopCoroutine(shakeCamera);
        shakeCamera = StartCoroutine(DoShakeCamera(power, time));
    }
    private IEnumerator DoShakeCamera(float power, float time) 
    {
        cMainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = power;
        yield return new WaitForSeconds(time);
        cMainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
    }
}
public enum CamType 
{
    WTD_tutorial, PlayerDie, FirstPersonPerspective
}