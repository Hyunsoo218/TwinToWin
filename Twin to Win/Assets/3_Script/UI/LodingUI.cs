using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LodingUI : MonoBehaviour
{
	[SerializeField] private Slider lodingBar;
	[SerializeField] private TextMeshProUGUI text;
	[SerializeField] private TextMeshProUGUI poolingText;

	private void OnEnable()
	{
		lodingBar.maxValue = EffectManager.instance.poolingObjCount;
		lodingBar.value = EffectManager.instance.finishedPoolingObjCount;
		text.text = lodingBar.value + " / " + lodingBar.maxValue;
		StartCoroutine(PoolingTextUpdate());
	}
	private void Update()
	{
		lodingBar.value = EffectManager.instance.finishedPoolingObjCount;
		text.text = lodingBar.value + " / " + lodingBar.maxValue;
	}
	private IEnumerator PoolingTextUpdate() 
	{
		while (true)
		{
			poolingText.text = "Pooling.";
			yield return new WaitForSeconds(0.15f);
			poolingText.text = "Pooling..";
			yield return new WaitForSeconds(0.15f);
			poolingText.text = "Pooling...";
			yield return new WaitForSeconds(0.15f);
		}
	}
}
