using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	[SerializeField] private PlayerStateUI cPSUI;
	[SerializeField] private StageUI cSUI;
	public static UIManager instance;
	private void Awake()
	{
		instance = this;
	}
    private void Update()
    {
		cPSUI.SetSkillFill();
		if (Input.GetKeyDown(KeyCode.Alpha1)) cSUI.OnStage(StageNumber.one); 
		if (Input.GetKeyDown(KeyCode.Alpha2)) cSUI.OnStage(StageNumber.twe); 
		if (Input.GetKeyDown(KeyCode.Alpha3)) cSUI.OnStage(StageNumber.three); 
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
	public void OnStageUI(StageNumber number) 
	{
		cSUI.OnStage(number);
	}
}
