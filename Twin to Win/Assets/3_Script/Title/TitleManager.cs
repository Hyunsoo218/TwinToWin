using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cinemachine;
using UnityEngine.SceneManagement;

public class TitleManager : CutScene
{
	[SerializeField] private Animator WGS_animator;
	[SerializeField] private Animator WTD_animator;
	private Animator animator;

	private void Awake() => animator = GetComponent<Animator>(); 
	public void GameStart() => animator.SetTrigger("Start"); 
	public void SceneLoad() => GameManager.instance.GameStart();
	public void ControlAnimatorWGS(int num) => WGS_animator.SetTrigger("Trigger_" + num);
	public void ControlAnimatorWTD(int num) => WTD_animator.SetTrigger("Trigger_" + num);
	public void SkipBtn() => animator.SetTrigger("Skip");
	public override IEnumerator Play() => throw new NotImplementedException(); 
}
