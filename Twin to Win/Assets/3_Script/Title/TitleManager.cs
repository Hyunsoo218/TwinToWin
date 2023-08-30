using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
	[SerializeField] private Transform tBlock_0;
	[SerializeField] private Transform tBlock_1;
	[SerializeField] private Transform tBlock_2;
	[SerializeField] private Image imgLogo;
	[SerializeField] private Image imgBackground;
	[SerializeField] private Image imgTouchToStart;
	private bool bGameStart = false;

    void Start()
    {
		FadeInOut();
	}
    void Update()
    {
		if (!bGameStart)
		{

		}
    }
	private void FadeInOut()
	{
		imgLogo.gameObject.SetActive(true);
		imgBackground.gameObject.SetActive(true);
		imgTouchToStart.gameObject.SetActive(true);
		imgLogo.color = new Color(1f, 1f, 1f, 0);
		imgBackground.color = Color.white;
		imgTouchToStart.color = new Color(1f, 1f, 1f, 0);
		StartCoroutine(ImgFadeInOut());
	}
	private IEnumerator ImgFadeInOut()
	{
		yield return StartCoroutine(LogoFadeIn());
		StartCoroutine(BackgroundFadeOut());
		yield return StartCoroutine(LogoFadeOut());
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
			imgTouchToStart.color = new Color(1f, 1f, 1f, imgTouchToStart.color.a + Time.deltaTime * 0.33f);
			yield return null;
		}
	}
	private IEnumerator TouchToStartFadeOut()
	{
		while (imgTouchToStart.color.a > 0.5f)
		{
			imgTouchToStart.color = new Color(1f, 1f, 1f, imgTouchToStart.color.a - Time.deltaTime * 0.33f);
			yield return null;
		}
	}
}
