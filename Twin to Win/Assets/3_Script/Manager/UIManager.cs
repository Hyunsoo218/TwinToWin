using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	[SerializeField] private PlayerStateUI cPSUI;
	public static UIManager instance;

	private void Awake()
	{
		instance = this;
	}
    private void Update()
    {
		cPSUI.SetSkillFill();
    }
    public void ConvertPlayer() 
	{
		cPSUI.Convert();
	}
	public void OnSkillBtn(KeyCode key) 
	{
		cPSUI.OnButton(key);
	}
	public void OnDodgeBtn() 
	{
		cPSUI.Dodge();
	}
}
