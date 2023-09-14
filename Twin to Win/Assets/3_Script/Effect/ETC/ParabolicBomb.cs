using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolicBomb : Effect
{
	[SerializeField] private TargetType eTargetType;
	[SerializeField] private DamagableSpaceControl cDSC;
	private Transform tUser;
	private float fDamage;
	private int nTargetLayer;
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
		Transform tPlayer = Player.instance.cCurrentCharacter.transform;

		Vector3 v3TargetPos = Vector3.zero;
		Vector3 v3Target;
		float fZ;
		float fX;

		switch (eTargetType)
		{
			case TargetType.Player:
				v3Target = tPlayer.position;
				fZ = Random.Range(v3Target.z - 2f, v3Target.z + 2f);
				fX = Random.Range(v3Target.x - 2f, v3Target.x + 2f);
				v3TargetPos = new Vector3(fX, 0, fZ);
				break;
			case TargetType.Player_forward:
				v3Target = tPlayer.forward * 4f + tPlayer.position;
				fZ = Random.Range(v3Target.z - 2f, v3Target.z + 2f);
				fX = Random.Range(v3Target.x - 2f, v3Target.x + 2f);
				v3TargetPos = new Vector3(fX, 0, fZ);
				break;
			case TargetType.Player_look:
				string strPlayerState = Player.instance.GetCurrentCharacterStateName();

				if (strPlayerState.Equals("moveState"))
				{
					v3Target = tPlayer.forward * 4f + tPlayer.position;
					fZ = Random.Range(v3Target.z - 3f, v3Target.z + 3f);
					fX = Random.Range(v3Target.x - 3f, v3Target.x + 3f);
					v3TargetPos = new Vector3(fX, 0, fZ);
				}
				else
				{
					v3Target = tPlayer.position;
					fZ = Random.Range(v3Target.z - 1f, v3Target.z + 1f);
					fX = Random.Range(v3Target.x - 1f, v3Target.x + 1f);
					v3TargetPos = new Vector3(fX, 0, fZ);
				}
				break;
			case TargetType.Boss_vicinity_Long:
				Vector3 v3Direction = new Vector3(Random.Range(-1f, 1f) ,0 , Random.Range(-1f, 1f));
				v3Target = Quaternion.LookRotation(v3Direction, Vector3.up) * Vector3.forward * Random.Range(6f, 15f) + tUser.position;
				v3TargetPos = new Vector3(v3Target.x, 0, v3Target.z);
				break;
		}

		Vector3 v3CurrtPos = transform.position;
		float runTime = 0;
		float duration = 1f;

		while (runTime < duration)
		{
			runTime += Time.deltaTime;

			transform.position = Vector3.Lerp(v3CurrtPos, v3TargetPos, runTime / duration);

			yield return null;
		}

		cDSC.OnAction(0.33f, FillType.X_Y);
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
	private enum TargetType
	{
		Player,
		Player_forward,
		Player_look,
		Boss_vicinity_Long
	}
}
