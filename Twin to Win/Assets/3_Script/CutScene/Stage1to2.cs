using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1to2 : CutScene
{
	[SerializeField] private Animator WGS_animator;
	[SerializeField] private Animator WTD_animator;
	[SerializeField] private Animator Boss_animator;
	[SerializeField] private Animator Turnipa_animator;
	[SerializeField] private GameObject weapon_WGS;
	[SerializeField] private GameObject weapon_WTD_righr;
	[SerializeField] private GameObject weapon_WTD_left;

	public void ControlAnimatorWGS(int num) => WGS_animator.SetTrigger("Trigger_" + num);
	public void ControlAnimatorWTD(int num) => WTD_animator.SetTrigger("Trigger_" + num);
	public void ControlAnimatorBoss(int num) => Boss_animator.SetTrigger("Trigger_" + num);
	public void ControlAnimatorTurnipa(int num) => Turnipa_animator.SetTrigger("Trigger_" + num);
	public void OnWeaponWGS() => weapon_WGS.SetActive(true);
	public void OffWeaponWGS() => weapon_WGS.SetActive(false);
	public void OnWeaponWTD() 
	{
		weapon_WTD_righr.SetActive(true); 
		weapon_WTD_left.SetActive(true); 
	}
	public void OffWeaponWTD()
	{
		weapon_WTD_righr.SetActive(false);
		weapon_WTD_left.SetActive(false);
	}
	public void EndCutScene() => playing = false;
	public override IEnumerator Play()
	{
		GetComponent<Animator>().SetTrigger("Play");
		while (playing)
		{
			yield return null;
		}
		GetComponent<Animator>().SetTrigger("End");
	}
}
