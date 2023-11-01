using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackEffect4 : Effect
{
	[SerializeField] private GameObject objParabolicBomb;
	private Animator cAnimator;
	private Transform tUser;
	private float fDamage;
	private int nTargetLayer;
	public override void Initialize()
	{
		base.Initialize();
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
		transform.SetParent(null);
		transform.localScale = Vector3.one;
		//transform.position = tUser.position;

		StartCoroutine(MakeParabolicBomb());
		//if (GameManager.instance.phase == Phase.Phase_3)
		//	cAnimator.SetTrigger("Action");
	}
	private IEnumerator MakeParabolicBomb()
	{
		for (int i = 0; i < 44; i++)
		{
			GameObject objEffect;

			for (int j = 0; j < 7; j++)
			{
				objEffect = EffectManager.instance.GetEffect(objParabolicBomb);
				objEffect.GetComponent<Effect>().OnAction(tUser, fDamage, nTargetLayer);
			}
			yield return null;
		}
	}
}
