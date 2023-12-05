using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolicBomb : Effect
{
	[SerializeField] private TargetType eTargetType;
	[SerializeField] private DamagableSpaceControl cDSC;
	[SerializeField] private GameObject bomb;
	private Transform tUser;
	private float fDamage;
	private int nTargetLayer;
	public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
	{
		this.tUser = tUser;
		this.fDamage = fDamage;
		this.nTargetLayer = nTargetLayer;

		bomb.SetActive(true);

		transform.SetParent(tUser);
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		transform.SetParent(null);
		transform.localScale = Vector3.one;

		StartCoroutine(MoveToTarget());
	}
	private IEnumerator MoveToTarget()
	{
		Transform tPlayer = Player.Instance.CurrentCharacter.transform;

		Vector3 v3TargetPos = Vector3.zero;
		Vector3 v3Target;
		float fZ;
		float fX;

		switch (eTargetType)
		{
			case TargetType.Player:
				v3Target = tPlayer.position;
				fZ = Random.Range(v3Target.z, v3Target.z);
				fX = Random.Range(v3Target.x, v3Target.x);
				v3TargetPos = new Vector3(fX, 0, fZ);
				break;
			case TargetType.Player_forward:
				v3Target = tPlayer.forward * 4f + tPlayer.position;
				fZ = Random.Range(v3Target.z - 2f, v3Target.z + 2f);
				fX = Random.Range(v3Target.x - 2f, v3Target.x + 2f);
				v3TargetPos = new Vector3(fX, 0, fZ);
				break;
			case TargetType.Player_look:
				string strPlayerState = Player.Instance.GetCurrentCharacterStateName();

				if (strPlayerState.Equals("moveState"))
				{
					v3Target = tPlayer.forward * 4f + tPlayer.position;
					fZ = Random.Range(v3Target.z - 2f, v3Target.z + 2f);
					fX = Random.Range(v3Target.x - 2f, v3Target.x + 2f);
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
		bomb.SetActive(false);
		switch (eTargetType)
		{
			case TargetType.Player:
			case TargetType.Player_forward:
			case TargetType.Player_look:
				soundComponent.PlayOneShot(clip);
				break;
		}
		Collider[] colliders = Physics.OverlapSphere(transform.position, 1f, nTargetLayer);
		foreach (Collider cItem in colliders)
		{
			Character cTarget;
			if (cItem.TryGetComponent<Character>(out cTarget))
			{
				DamageCalculator.OnDamage(cTarget, fDamage, criticalHit);
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
