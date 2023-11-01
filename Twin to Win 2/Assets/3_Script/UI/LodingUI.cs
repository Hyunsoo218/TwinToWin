using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LodingUI : MonoBehaviour
{
	[SerializeField] private Slider lodingBar;
	[SerializeField] private TextMeshProUGUI text;

	private void OnEnable()
	{
		lodingBar.maxValue = EffectManager.instance.poolingObjCount;
		lodingBar.value = EffectManager.instance.finishedPoolingObjCount;
		text.text = lodingBar.value + " / " + lodingBar.maxValue;
	}
	private void Update()
	{
		lodingBar.value = EffectManager.instance.finishedPoolingObjCount;
		text.text = ((lodingBar.value / lodingBar.maxValue * 100f).ToString("N0") + "%");
	}
}
