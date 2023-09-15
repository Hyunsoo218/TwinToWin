using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackEffect5 : Effect
{
	[SerializeField] private List<Transform> arrThornPos_1;
	[SerializeField] private List<Transform> arrThornPos_2;
	[SerializeField] private List<Transform> arrThornPos_3;
	[SerializeField] private List<DamagableSpaceControl> arrDSC;
	[SerializeField] private GameObject objParabolicBomb_Player;
	[SerializeField] private GameObject objParabolicBomb_Player_Look;
	private Animator cAnimator;
	private Transform tUser;
	private float fDamage;
	private int nTargetLayer;
	private int nDSCNum;
	private int nOverlapNum;
	private void Awake()
	{
		cAnimator = GetComponent<Animator>();
	}
	public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
	{
		this.tUser = tUser;
		this.fDamage = fDamage;
		this.nTargetLayer = nTargetLayer;

		transform.SetParent(tUser.GetChild(1));
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.SetParent(EffectManager.instance.transform);
		transform.localScale = Vector3.one;

		nDSCNum = -1;
		nOverlapNum = -1;

		string strAnimationTrigger = "Action";
		if (GameManager.instance.phase == Phase.Phase_3)
		{
			strAnimationTrigger += "Enhance";
			StartCoroutine(MakeParabolicBomb());
			StartCoroutine(LookTarget());
		}
		cAnimator.SetTrigger(strAnimationTrigger);
	}
	public void OnDamagableSpace()
	{
		nDSCNum++;
		float fTime = 0;
		switch (nDSCNum)
		{
			case 0: fTime = 1.5f; break;
			case 1: fTime = 1f; break;
			case 2: fTime = 1.25f; break;
		}
		arrDSC[nDSCNum].gameObject.SetActive(true);
		arrDSC[nDSCNum].OnAction(fTime, FillType.Alpha);
	}
	public void OnDamageOverlap() 
	{
		nOverlapNum++;
		List<Transform> arrOverlapPos = null;
		Character cTarget = null;
		int nTargetHitNum = 0;
		switch (nOverlapNum)
		{
			case 0: arrOverlapPos = arrThornPos_1; break;
			case 1: arrOverlapPos = arrThornPos_2; break;
			case 2: arrOverlapPos = arrThornPos_3; break;
		}
		foreach (Transform pos in arrOverlapPos)
		{
			Collider[] target = Physics.OverlapSphere(pos.position, 0.5f, nTargetLayer);
			foreach (Collider cItem in target)
			{
				if (cItem.TryGetComponent<Character>(out cTarget))
				{
					nTargetHitNum++;
				}
			}
		}
		if (nTargetHitNum > 0 )
		{
			cTarget.Damage(fDamage);
			print($"가시가 {cTarget.name}에게 {fDamage}의 데미지 입힘");
		}
	}
	private IEnumerator MakeParabolicBomb()
	{
		yield return new WaitForSeconds(0.5f);
		GameObject objEffect;
		for (int i = 0; i < 5; i++)
		{
			objEffect = EffectManager.instance.GetEffect(objParabolicBomb_Player_Look);
			objEffect.GetComponent<Effect>().OnAction(tUser, fDamage, nTargetLayer);
			objEffect = EffectManager.instance.GetEffect(objParabolicBomb_Player);
			objEffect.GetComponent<Effect>().OnAction(tUser, fDamage, nTargetLayer);
			yield return new WaitForSeconds(0.2f);
		}
	}
	private IEnumerator LookTarget() 
	{
		float fRunTime = 1f;
		float fTime = 0;
		Vector3 v3TempRot;
		while (fTime < fRunTime)
		{
			fTime += Time.deltaTime;
			transform.LookAt(Player.instance.cCurrentCharacter.transform);
			v3TempRot = transform.eulerAngles;
			v3TempRot.x = v3TempRot.z = 0;
			transform.eulerAngles = v3TempRot;
			yield return null;
		}
	}
	public void NavMeshBake() 
	{
		StageManager.instance.UpdateNavMeshOne();
	}
}
