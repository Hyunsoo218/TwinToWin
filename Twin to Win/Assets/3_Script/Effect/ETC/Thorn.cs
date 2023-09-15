using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thorn : Effect
{
	public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
	{
		float fAngX = Random.Range(-10f, 10f);
		float fAngY = Random.Range(0, 360f);
		float fAngZ = Random.Range(-10f, 10f);
		float fSize = Random.Range(1f, 1.5f);
		transform.eulerAngles = new Vector3(fAngX, fAngY, fAngZ);
		transform.localScale = fSize * Vector3.one;
	}
}
