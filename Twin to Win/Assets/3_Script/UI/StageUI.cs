using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageUI : MonoBehaviour
{
	private Animator cAnimator;
	private void Awake()
	{
		cAnimator = GetComponent<Animator>();
	}
	public void OnStage(StageNumber number) 
	{
		string animationTrigger = "Stage";
		switch (number)
		{
			case StageNumber.one: animationTrigger += "_1"; break;
			case StageNumber.twe: animationTrigger += "_2"; break;
			case StageNumber.three: animationTrigger += "_3"; break;
		}
		cAnimator.SetTrigger(animationTrigger);
	}
}
public enum StageNumber 
{
	one, twe, three
}