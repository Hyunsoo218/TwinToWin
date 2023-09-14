using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DamagableSpaceControl : MonoBehaviour
{
	private DecalProjector cArea;
	private DecalProjector cFill;
	private void Awake()
	{
		DecalProjector[] decalProjectors = GetComponents<DecalProjector>();
		cArea = decalProjectors[0];
		cFill = decalProjectors[1];
		cArea.gameObject.SetActive(false);
		cFill.gameObject.SetActive(false);
	}
	public void OnAction(float fTime, FillType eType)
	{
		cArea.gameObject.SetActive(true);
		cFill.gameObject.SetActive(true);
		switch (eType)
		{
			case FillType.X:
				cFill.size = new Vector3(0, cArea.size.y, cArea.size.z);
				StartCoroutine(FillingX(fTime));
				break;
			case FillType.Y:
				cFill.size = new Vector3(cArea.size.x, 0, cArea.size.z);
				StartCoroutine(FillingY(fTime));
				break;
			case FillType.X_Y:
				cFill.size = new Vector3(0, 0, cArea.size.z);
				StartCoroutine(FillingX_Y(fTime));
				break;
			case FillType.Alpha:
				cFill.size = new Vector3(0, 0, cArea.size.z);
				cArea.fadeFactor = 0;
				StartCoroutine(FillingAlpha(fTime));
				break;
		}
	}
	private IEnumerator FillingAlpha(float fTime)
	{
		float fRunTime = 0;

		float fFadeInTime = 0.8f;
		float fFadeOutTime = 0.2f;

		while (fRunTime < fFadeInTime)
		{
			yield return null;
			fRunTime += Time.deltaTime;
			cArea.fadeFactor = fRunTime / fFadeInTime;
		}
		fRunTime = 0;
		while (fRunTime < fFadeOutTime)
		{
			yield return null;
			fRunTime += Time.deltaTime;
			cArea.fadeFactor = 1f - fRunTime / fFadeOutTime;
		}
		cArea.gameObject.SetActive(false);
	}
	private IEnumerator FillingY(float fTime)
	{
		float fRunTime = 0;
		Vector3 v3Current;

		while (fRunTime < fTime)
		{
			yield return null;
			fRunTime += Time.deltaTime;
			v3Current = cArea.size * fRunTime / fTime;
			v3Current.z = 10f;
			v3Current.x = cArea.size.x;
			cFill.size = v3Current;
		}

		cArea.gameObject.SetActive(false);
		cFill.gameObject.SetActive(false);
	}
	private IEnumerator FillingX(float fTime)
	{
		float fRunTime = 0;
		Vector3 v3Current;

		while (fRunTime < fTime)
		{
			yield return null;
			fRunTime += Time.deltaTime;
			v3Current = cArea.size * fRunTime / fTime;
			v3Current.z = 10f;
			v3Current.y = cArea.size.y;
			cFill.size = v3Current;
		}

		cArea.gameObject.SetActive(false);
		cFill.gameObject.SetActive(false);
	}
	private IEnumerator FillingX_Y(float fTime)
	{
		float fRunTime = 0;
		Vector3 v3Current;

		while (fRunTime < fTime)
		{
			yield return null;
			fRunTime += Time.deltaTime;
			v3Current = cArea.size * fRunTime / fTime;
			v3Current.z = 10f;
			cFill.size = v3Current;
		}

		cArea.gameObject.SetActive(false);
		cFill.gameObject.SetActive(false);
	}
}
public enum FillType
{
	X, Y, X_Y, Alpha
}
