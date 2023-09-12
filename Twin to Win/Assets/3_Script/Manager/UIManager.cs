using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;
	[SerializeField] private Text txtDebug;
	private void Awake()
	{
		instance = this;
	}
	public void DebugLog(string text)
	{
		txtDebug.text = text;
	}
}
