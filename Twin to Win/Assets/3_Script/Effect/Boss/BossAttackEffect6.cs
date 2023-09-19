using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackEffect6 : Effect
{
	[SerializeField] private DamagableSpaceControl cDSC;
	[SerializeField] private bool bPreviewOverlapArea;
	[SerializeField] private Vector3 vAttackAreaCenter;
	[SerializeField] private Vector3 vAttackAreaSize;
	[SerializeField] private List<Effect> arrRush;
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

		StartCoroutine(OnAction());

		cDSC.OnAction(1f, FillType.X);

		//string strAnimationTrigger = "Action";
		//if (GameManager.instance.phase == Phase.Phase_3)
		//{
		//	strAnimationTrigger += "Enhance";
		//}
		//cAnimator.SetTrigger(strAnimationTrigger);
	}
	private IEnumerator OnAction()
	{
		float time = Time.time;
		transform.SetParent(EffectManager.instance.transform);
		transform.localScale = Vector3.one;

		transform.SetParent(tUser.GetChild(1));
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;

		yield return new WaitForSeconds(1f);

		transform.SetParent(EffectManager.instance.transform);
		Vector3 vOverlapPos = Quaternion.LookRotation(transform.forward, Vector3.up) * vAttackAreaCenter + transform.position;
		Collider[] arrOverlapObj = Physics.OverlapBox(vOverlapPos, vAttackAreaSize, transform.rotation, nTargetLayer);
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
		if (bPreviewOverlapArea)
		{
			GameObject objPreviewBox = Resources.Load<GameObject>("AttackAreaPreviewCube");
			GameObject obj = Instantiate(objPreviewBox);
			obj.transform.position = vOverlapPos;
			obj.transform.rotation = transform.rotation;
			obj.transform.localScale = vAttackAreaSize * 2f;
			Destroy(obj, 1f);
		}

		yield return new WaitForSeconds(1.5f);

		if (GameManager.instance.phase == Phase.Phase_3)
		{
			for (int i = 0; i < arrRush.Count; i++)
			{
				for (int j = 0; j < arrRush.Count / 5; j++ )
				{
					if (i + j < arrRush.Count)
					{
						arrRush[i + j].gameObject.SetActive(true);
						arrRush[i + j].OnAction(tUser, fDamage, nTargetLayer);
					}
				}
				yield return new WaitForSeconds(.5f);
				i += arrRush.Count / 5 - 1;
			}
		}
		else
		{
			float fRushInterval = 1f;
			for (int i = 0; i < arrRush.Count; i++)
			{
				arrRush[i].gameObject.SetActive(true);
				arrRush[i].OnAction(tUser, fDamage, nTargetLayer);
				yield return new WaitForSeconds(fRushInterval);
				fRushInterval = (fRushInterval > 0.3f) ? fRushInterval -= 0.1f : 0.3f;
			}
		}
	}
}
