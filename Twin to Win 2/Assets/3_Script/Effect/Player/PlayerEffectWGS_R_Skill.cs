using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerEffectWGS_R_Skill : PlayerEffect
{
	private Transform tUser;
	private int nTargetLayer;
	private Animator animator;
	[SerializeField] private CinemachineVirtualCamera cam;
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
		cam.m_LookAt = tUser;
		animator.SetTrigger("Run");
	}
	public void ControlTimeScale(float scale)
	{
		Time.timeScale = scale;
	}
	public void EnableEffect() 
	{
		StartCoroutine(DoShakeCamera());
		Vector3 vOverlapPos = Quaternion.LookRotation(transform.forward, Vector3.up) * vAttackAreaCenter + transform.position;
		Collider[] arrOverlapObj = null;
		if (isSphere)
			arrOverlapObj = Physics.OverlapSphere(vOverlapPos, sphereAttackAreaRange, nTargetLayer);
		else
			arrOverlapObj = Physics.OverlapBox(vOverlapPos, vAttackAreaSize, tUser.rotation, nTargetLayer);
		foreach (Collider cItem in arrOverlapObj)
		{
			if (cItem.TryGetComponent<Character>(out var target))
				DamageCalculator.OnDamage(target, damage, criticalHit);
		}
	}
	private IEnumerator DoShakeCamera()
	{
		cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = shakeCameraPower;
		yield return new WaitForSeconds(shakeCameraTime);
		cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
	}
	public void ResetCharacterState() 
	{
		Player.instance.CurrentCharacter.ReturnToIdle();
	}
	private void OnDisable()
	{
		cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
	}
}
