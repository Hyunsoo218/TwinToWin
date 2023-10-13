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
        if (instance == null) instance = this; 
		else Destroy(gameObject);
        SetTitle();
    }
    public void SetTitle() 
    {
        cPSUI.gameObject.SetActive(false);
        cBSUI.gameObject.SetActive(false);
        cSUI.gameObject.SetActive(false);
        cTUI.gameObject.SetActive(false);
        cTalkUI.gameObject.SetActive(false);
        cHpC.gameObject.SetActive(false);
        cDFC.gameObject.SetActive(false);
        playerDie.SetActive(false);
    }
	public void SetGame() 
	{
        cPSUI.gameObject.SetActive(true);
        cBSUI.gameObject.SetActive(false);
        cSUI.gameObject.SetActive(true);
        cTUI.gameObject.SetActive(true);
        cTalkUI.gameObject.SetActive(true);
        cHpC.gameObject.SetActive(true);
        cDFC.gameObject.SetActive(true);
        playerDie.SetActive(false);

        print("ui ����");
    }
    private void Update()
	{
		if (GameManager.instance.gameStage == GameStage.Game)
        {
            cPSUI.SetSkillFill();
        }
    }
    public void ConvertPlayer() 
	{
		cPSUI.Convert();
	}
	public void OnSkillBtn(KeyCode key, bool stay = false, bool stayEnd = false) 
	{
		cPSUI.OnButton(key, stay, stayEnd);
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
	public IEnumerator WaitForChoice(string name, string script, List<string> Options) 
	{
		yield return StartCoroutine(cTalkUI.Choice(name, script, Options));
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
	public int GetPlayersChoice()
	{
		return cTalkUI.GetPlayersChoice();
	}
}
