using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackEffect3 : Effect
{
	[SerializeField] private GameObject objParabolicBomb;
	private Transform tUser;
	private float fDamage;
	private int nTargetLayer;
	private Coroutine coTrackUser;

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

		coTrackUser = StartCoroutine(TrackUser());
		StartCoroutine(Overlap());
		if (GameManager.instance.phase == Phase.Phase_3)
		{
			StartCoroutine(MakeParabolicBomb());
		}
	}
	private IEnumerator TrackUser() 
	{
		while (true)
		{
			transform.position = tUser.position;
			yield return null;
		}
	}
	private IEnumerator Overlap() 
	{
		for (int i = 0; i < 10; i++)
		{
			Collider[] arrOverlapObj = Physics.OverlapSphere(transform.position, 4.5f, nTargetLayer);

			foreach (Collider cItem in arrOverlapObj)
			{
				Character cTarget;
				if (cItem.TryGetComponent<Character>(out cTarget))
				{
					cTarget.Damage(fDamage);
					print($"{tUser.name}이(가) {cTarget.name}에게 {fDamage}의 데미지 입힘");
					// 지우지 마영 - 디버그용							  		   
				}
			}
			yield return new WaitForSeconds(0.33f);
		}
	}
	private IEnumerator MakeParabolicBomb() 
	{
		for (int i = 0; i < 30; i++)
		{
			GameObject objEffect = EffectManager.instance.GetEffect(objParabolicBomb);
			objEffect.GetComponent<Effect>().OnAction(tUser, fDamage, nTargetLayer);
			yield return new WaitForSeconds(0.1f);
		}
	}
	protected override void InPoolEvent()
	{
		base.InPoolEvent();
		StopCoroutine(coTrackUser);
	}
}
