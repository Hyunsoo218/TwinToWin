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
		StartCoroutine(TutorialTag());
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
	public void OnStageUI(StageNumber number) 
	{
		cSUI.OnStage(number);
	}
	public IEnumerator TutorialTag() 
	{
		bool run = true;
		while (run) 
		{
			yield return null;
            if (Input.GetKeyDown(KeyCode.Tab)) run = false; 
		}
	}
    public IEnumerator TutorialSkill()
    {
        bool run = true;
        while (run)
        {
            yield return null;
            if (Input.GetKeyDown(KeyCode.E)) run = false; 
        }
    }
}
