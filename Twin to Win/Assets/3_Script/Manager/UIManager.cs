using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	[SerializeField] private PlayerStateUI cPSUI;
	[SerializeField] private BossStateUI cBSUI;
	[SerializeField] private StageUI cSUI;
	[SerializeField] private TutorialUI cTUI;
	public static UIManager instance;
	private void Awake()
	{
		instance = this;
		cPSUI.gameObject.SetActive(true);
		cBSUI.gameObject.SetActive(true);
		cSUI.gameObject.SetActive(true);
		cTUI.gameObject.SetActive(true);
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
	public void OnStageUI(StageNumber number) 
	{
		cSUI.OnStage(number);
	}
    public IEnumerator WaitForTutorial()
    {
		yield return cTUI.WaitForTutorial();
        yield return new WaitForSeconds(0.5f);
    }
}
