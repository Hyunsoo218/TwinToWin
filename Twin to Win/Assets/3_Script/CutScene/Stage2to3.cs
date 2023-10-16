using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2to3 : CutScene
{
	[SerializeField] private Animator WGS_animator;
	[SerializeField] private Animator WTD_animator;
	[SerializeField] private Animator Boss_animator;
	[SerializeField] private GameObject bossEyesFire_1;

	public void ControlAnimatorWGS(int num) => WGS_animator.SetTrigger("Trigger_" + num);
	public void ControlAnimatorWTD(int num) => WTD_animator.SetTrigger("Trigger_" + num);
	public void ControlAnimatorBoss(int num) => Boss_animator.SetTrigger("Trigger_" + num); 
	public void OnBossEyesFire_1() => bossEyesFire_1.SetActive(true);
	public void EndCutScene() => playing = false;
	public void SetTimeScale(float timeScale) => Time.timeScale = timeScale; 
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
