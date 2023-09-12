using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolicBomb : Effect
{
	private Animator cAnimator;
	private Transform tUser;
	private float fDamage;
	private int nTargetLayer;
	private void Awake()
	{
		cAnimator = GetComponent<Animator>();
	}
	public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
	{
		this.tUser = tUser;
		this.fDamage = fDamage;
		this.nTargetLayer = nTargetLayer;

		transform.SetParent(tUser);
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.SetParent(EffectManager.instance.transform);
		transform.localScale = Vector3.one;

		StartCoroutine(MoveToTarget());
	}
	private IEnumerator MoveToTarget()
	{
		Vector3 v3Target = Player.instance.cCurrentCharacter.transform.position;
		float fX = Random.Range(v3Target.x - 3f, v3Target.x + 3f);
		float fZ = Random.Range(v3Target.z - 3f, v3Target.z + 3f);
		//float fX = Random.Range(tUser.position.x - 10f, tUser.position.x + 10f);
		//float fZ = Random.Range(tUser.position.z - 10f, tUser.position.z + 10f);
		Vector3 v3TargetPos = new Vector3(fX, 0, fZ);
		Vector3 v3CurrtPos = transform.position;

		float runTime = 0;
		float duration = 1f;

		while (runTime < duration)
		{
			runTime += Time.deltaTime;

			transform.position = Vector3.Lerp(v3CurrtPos, v3TargetPos, runTime / duration);

			yield return null;
		}
	}
	public void Explosion() 
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, 1f, nTargetLayer);
		foreach (Collider cItem in colliders)
		{
			Character cTarget;
			if (cItem.TryGetComponent<Character>(out cTarget))
			{
				cTarget.Damage(fDamage);
				print($"포물선폭탄이 {cTarget.name}에게 {fDamage}의 데미지 입힘");
				// 지우지 마영 - 디버그용							  		   
			}
		}
	}
}
