using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cinemachine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
	[SerializeField] private List<CinemachineVirtualCamera> arrCamPos;
	[SerializeField] private List<movementBlock> arrBlock;
	[SerializeField] private GameObject btnStart;
	[SerializeField] private Image imgLogo;
	[SerializeField] private Image imgBackground;
	[SerializeField] private Image imgTouchToStart;
	private Coroutine coFade;
	private Coroutine coMoveCamPos;
	private bool bGameStart = false;
	private int nCamPosCount = 10;

	private void Awake()
	{
		imgLogo.gameObject.SetActive(true);
		imgBackground.gameObject.SetActive(true);
		imgTouchToStart.gameObject.SetActive(true);
		btnStart.SetActive(false);
		imgLogo.color = new Color(1f, 1f, 1f, 0);
		imgBackground.color = Color.white;
		imgTouchToStart.color = new Color(1f, 1f, 1f, 0);
	}
	private void Start()
	{
		coFade = StartCoroutine(ImgFadeInOut());
	}
	private void Update()
    {
		if (!bGameStart)
		{
			foreach (movementBlock item in arrBlock)
			{
				item.MoveBlock();
			}
		}
    }
	public void GameStart() 
	{
		if (!bGameStart)
		{
            bGameStart = true;
			StopCoroutine(coFade);
			StopCoroutine(coMoveCamPos);
			StartCoroutine(TouchToStartFadeOutAll());
			arrCamPos[0].Priority = nCamPosCount;
			GameManager.instance.GameStart();
		}
	}
	private IEnumerator MoveCamPos() 
	{
		int nCamPosIdx = 0;
		while (true)
		{
			arrCamPos[nCamPosIdx].Priority = nCamPosCount;
			nCamPosCount++;
			nCamPosIdx++;

			nCamPosIdx = (arrCamPos.Count == nCamPosIdx) ? 0 : nCamPosIdx;
			yield return new WaitForSeconds(5f);
		}
	}
	private IEnumerator ImgFadeInOut()
	{
		yield return StartCoroutine(LogoFadeIn());
		StartCoroutine(BackgroundFadeOut());
		yield return StartCoroutine(LogoFadeOut());
		btnStart.SetActive(true);
		coMoveCamPos = StartCoroutine(MoveCamPos());
		while (true)
		{
			yield return StartCoroutine(TouchToStartFadeIn());
			yield return StartCoroutine(TouchToStartFadeOut());
		}
	}
	private IEnumerator LogoFadeIn()
	{
		while (imgLogo.color.a < 1f)
		{
			imgLogo.color = new Color(1f, 1f, 1f, imgLogo.color.a + Time.deltaTime * 0.5f);
			yield return null;
		}
	}
	private IEnumerator LogoFadeOut()
	{
		yield return new WaitForSeconds(1f);
		while (imgLogo.color.a > 0)
		{
			imgLogo.color = new Color(1f, 1f, 1f, imgLogo.color.a - Time.deltaTime * 0.25f);
			yield return null;
		}
	}
	private IEnumerator BackgroundFadeOut()
	{
		while (imgBackground.color.a > 0)
		{
			imgBackground.color = new Color(0, 0, 0, imgBackground.color.a - Time.deltaTime * 0.33f);
			yield return null;
		}
	}
	private IEnumerator TouchToStartFadeIn()
	{
		while (imgTouchToStart.color.a < 1f)
		{
			if (bGameStart) break;
			imgTouchToStart.color = new Color(1f, 1f, 1f, imgTouchToStart.color.a + Time.deltaTime * 0.33f);
			yield return null;
		}
	}
	private IEnumerator TouchToStartFadeOut()
	{
		while (imgTouchToStart.color.a > 0.5f)
		{
			if (bGameStart) break;
			imgTouchToStart.color = new Color(1f, 1f, 1f, imgTouchToStart.color.a - Time.deltaTime * 0.33f);
			yield return null;
		}
	}
	private IEnumerator TouchToStartFadeOutAll()
	{
		while (imgTouchToStart.color.a > 0.01f)
		{
			imgTouchToStart.color = new Color(1f, 1f, 1f, imgTouchToStart.color.a - Time.deltaTime * 1f);
			yield return null;
		}
	}
	[Serializable]
	private class movementBlock
	{
		[SerializeField] private Transform tBlock;
		public void MoveBlock() 
		{
			tBlock.localPosition -= Vector3.forward * Time.deltaTime;
			if (tBlock.localPosition.z <= -51.6f)
			{
				tBlock.localPosition += Vector3.forward * 154.8f;
			}
		}
	}
}
