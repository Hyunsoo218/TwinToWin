using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectProjectile : EffectPhysics
{
	[SerializeField] private float fSpeed = 10f;
	protected void Update()
	{
		transform.Translate(Vector3.forward * Time.deltaTime * MasterSpeed * fSpeed);
	}
}
