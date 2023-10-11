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
	[SerializeField] private TalkUI cTalkUI;
	[SerializeField] private HpbarControl cHpC;
	[SerializeField] private DamageFontControl cDFC;
	[SerializeField] private GameObject playerDie;
	public static UIManager instance;
	private void Awake()
	{
		instance = this;
		cPSUI.gameObject.SetActive(true);
		cBSUI.gameObject.SetActive(false);
		cSUI.gameObject.SetActive(true);
		cTUI.gameObject.SetActive(true);
        cTalkUI.gameObject.SetActive(true);
        cHpC.gameObject.SetActive(true);
        cDFC.gameObject.SetActive(true);
		playerDie.SetActive(false);
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
        yield return null;
        yield return cTUI.WaitForTutorial();
        yield return new WaitForSeconds(0.5f);
    }
	public void SetPlayerHealthPoint()
    {
        cPSUI.SetPlayerHealthPoint();
    }
    public void OnTutorial(TutorialType type)
    {
		cTUI.OnTutorial(type);
    }
    public void OffTutorial()
    {
		cTUI.OffTutorial();
    }
	public IEnumerator WaitForTalk(string name, string script, float autoClickTime = -1f) 
	{
		yield return StartCoroutine(cTalkUI.ShowText(name, script, autoClickTime));
	}
	public void InsertHpbar(Character target, Vector3 offset) 
	{
        cHpC.InsertHpbar(target, offset);
    }
    public void SetHp(Character target)
    {
        cHpC.SetHp(target);
    }
    public void RemoveHpbar(Character target)
    {
        cHpC.RemoveHpbar(target);
    }
    public void OnDamageFont(Vector3 targetPos, DamageType type, float dmage)
    {
        cDFC.EnableDamageFont(targetPos, type, dmage);
    }
	public void OnPlayerDie() 
	{
		playerDie.SetActive(true);
	}
}
