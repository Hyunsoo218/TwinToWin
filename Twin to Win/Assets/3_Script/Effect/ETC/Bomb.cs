using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Effect
{
	[SerializeField] private Transform tBomb;
	[SerializeField] private Transform tExplosion;
	private Transform tUser;
	private float fDamage;
	private int nTargetLayer;
	protected override void OnEnable()
	{
		base.OnEnable();
		tExplosion.gameObject.SetActive(false);
		StartCoroutine(BombMoveToTarget());
	}
	protected override void InPool()
	{
		tExplosion.gameObject.SetActive(false);
		base.InPool();
	}
	public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
	{
		this.tUser = tUser;
		this.fDamage = fDamage;
		this.nTargetLayer = nTargetLayer;
		tExplosion.gameObject.SetActive(false);
	}
	private IEnumerator BombMoveToTarget()
	{
		tExplosion.gameObject.SetActive(false);
		yield return null;
		tExplosion.gameObject.SetActive(false);
		tBomb.position = tUser.position + Vector3.up * 5f;

		Vector3 v3TargetPos = transform.position;
		Vector3 v3CurrtPos = tBomb.position;

		float runTime = 0;
		float duration = 1f;

		while (runTime < duration)
		{
			runTime += Time.deltaTime * 1.5f;

			tBomb.position = Vector3.Slerp(v3CurrtPos, v3TargetPos, runTime / duration);

			yield return null;
		}
	}
	public void Explosion()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, tExplosion.localScale.x * 0.5f, nTargetLayer);
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
