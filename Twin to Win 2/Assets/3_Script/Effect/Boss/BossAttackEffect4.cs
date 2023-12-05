using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackEffect4 : Effect
{
	[SerializeField] private DamagableSpaceControl dsc;
	[SerializeField] private float sphereAttackAreaRange = 0f;
	[SerializeField] private GameObject explotions;
	private Transform tUser;
	private float fDamage;
	private int nTargetLayer;
    private void Start()
    {
		dsc.OnAction(2f, FillType.Alpha);
	}
    protected override void OnEnable()
    {
        base.OnEnable(); 
		explotions.SetActive(false);
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
		dsc.OnAction(2f,FillType.Alpha);
		StartCoroutine(DoAction());
	}
	private IEnumerator DoAction() 
	{
		yield return new WaitForSeconds(2f);
		explotions.SetActive(true);
		Collider[] arrOverlapObj = Physics.OverlapSphere(transform.position, sphereAttackAreaRange, nTargetLayer);
		foreach (Collider cItem in arrOverlapObj)
		{
			if (cItem.TryGetComponent<Character>(out var target)) 
			{
				float dist = Vector3.Distance(target.transform.position, transform.position);
                if (dist > 4f)
					DamageCalculator.OnDamage(target, fDamage, criticalHit);
			}
		}
	}
    private void OnDrawGizmos()
    {
		Gizmos.DrawWireSphere(transform.position, sphereAttackAreaRange);
	}
}
