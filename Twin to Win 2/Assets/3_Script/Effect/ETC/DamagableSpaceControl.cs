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
		gameObject.SetActive(false);
		DecalProjector[] decalProjectors = GetComponents<DecalProjector>();
		cArea = decalProjectors[0];
		cFill = decalProjectors[1];
	}
	public void OnAction(float fTime, FillType eType)
	{
		gameObject.SetActive(true);
		StartCoroutine(OnActionCo(fTime, eType));
	}
	private IEnumerator OnActionCo(float fTime, FillType eType) 
	{
		cArea.enabled = false;
		cArea.enabled = true;
		switch (eType)
		{
			case FillType.X:
				StartCoroutine(FillingX(fTime));
				StartCoroutine(FillingAlpha(fTime));
				break;
			case FillType.Y:
				StartCoroutine(FillingY(fTime));
				//StartCoroutine(FillingAlpha(fTime));
				break;
			case FillType.X_Y:
				StartCoroutine(FillingX_Y(fTime));
				break;
			case FillType.Alpha:
				StartCoroutine(FillingAlpha(fTime));
				break;
		}

		yield return null;
	}
	public void Cancel() 
	{
        gameObject.SetActive(false);
    }
	private IEnumerator FillingAlpha(float fTime)
	{
		cFill.size = new Vector3(0, 0, cArea.size.z);
		cArea.fadeFactor = 0;

		float fRunTime = 0;

		float fFadeOutTime = 0.2f;
		float fFadeInTime = fTime - fFadeOutTime;
		fFadeOutTime -= 0.05f;
		while (fRunTime < fFadeInTime)
		{
			yield return null;
			fRunTime += Time.deltaTime;
			cArea.fadeFactor = fRunTime / fFadeInTime;
			cFill.fadeFactor = fRunTime / fFadeInTime;
		}
		fRunTime = 0;
		while (fRunTime < fFadeOutTime)
		{
			yield return null;
			fRunTime += Time.deltaTime;
			cArea.fadeFactor = 1f - fRunTime / fFadeOutTime;
			cFill.fadeFactor = 1f - fRunTime / fFadeOutTime;
		}
		gameObject.SetActive(false);
	}
	private IEnumerator FillingY(float fTime)
	{
		cFill.size = new Vector3(cArea.size.x, 0, cArea.size.z);
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

		gameObject.SetActive(false);
	}
	private IEnumerator FillingX(float fTime)
	{
		cFill.size = new Vector3(0, cArea.size.y, cArea.size.z);
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

		gameObject.SetActive(false);
	}
	private IEnumerator FillingX_Y(float fTime)
	{
		cFill.size = new Vector3(0, 0, cArea.size.z);
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

		gameObject.SetActive(false);
	}
}
public enum FillType
{
	X, Y, X_Y, Alpha
}
