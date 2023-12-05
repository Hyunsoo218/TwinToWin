using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectWTD_R_Skill : PlayerEffect
{
    private Transform tUser;
    private int nTargetLayer;
    private Animator animator;
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private GameObject flnishAttack;
    public override void Initialize()
    {
        base.Initialize();
        animator = GetComponent<Animator>();
    }
    public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
    {
        this.tUser = tUser;
        this.nTargetLayer = nTargetLayer;
        transform.position = tUser.position;
        transform.rotation = tUser.rotation;
        CameraManager.instance.SetDefaultBlend(0);
        animator.SetTrigger("Run");
    }
    public void ControlTimeScale(float scale)
    {
        Time.timeScale = scale;
    }
    private IEnumerator DoShakeCamera()
    {
        cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = shakeCameraPower;
        yield return new WaitForSeconds(shakeCameraTime);
        cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
    }
    public void ResetCharacterState()
    {
        CameraManager.instance.SetDefaultBlend(0.2f);
        Player.Instance.CurrentCharacter.ReturnToIdle();
    }
    private void OnDisable()
    {
        cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
    }
    public void OnActiveRoot() => Player.Instance.SetActiveRoot(true);
    public void OffActiveRoot() => Player.Instance.SetActiveRoot(false);
    public void EnableEffect() 
    {
        Vector3 vOverlapPos = Quaternion.LookRotation(transform.forward, Vector3.up) * vAttackAreaCenter + transform.position;
        Collider[] arrOverlapObj = null;
        if (isSphere)
            arrOverlapObj = Physics.OverlapSphere(vOverlapPos, sphereAttackAreaRange, nTargetLayer);
        else
            arrOverlapObj = Physics.OverlapBox(vOverlapPos, vAttackAreaSize, tUser.rotation, nTargetLayer);
        foreach (Collider cItem in arrOverlapObj)
        {
            if (cItem.TryGetComponent<Character>(out var target))
                DamageCalculator.OnDamage(target, damage * 0.02f, criticalHit);
        }
    }
    public void EnableFinishAttack() 
    {
        StartCoroutine(DoShakeCamera());
        GameObject effect = Instantiate(flnishAttack, transform.position, transform.rotation);
        effect.transform.position += new Vector3(0, 0.32f, 0);
        
        Vector3 vOverlapPos = Quaternion.LookRotation(transform.forward, Vector3.up) * vAttackAreaCenter + transform.position;
        Collider[] arrOverlapObj = null;
        if (isSphere)
            arrOverlapObj = Physics.OverlapSphere(vOverlapPos, sphereAttackAreaRange, nTargetLayer);
        else
            arrOverlapObj = Physics.OverlapBox(vOverlapPos, vAttackAreaSize, tUser.rotation, nTargetLayer);
        foreach (Collider cItem in arrOverlapObj)
        {
            if (cItem.TryGetComponent<Character>(out var target))
                DamageCalculator.OnDamage(target, damage * 0.8f, criticalHit);
        }
    }
}
