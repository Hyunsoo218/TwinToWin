using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	public Phase phase;
	private Queue<Action> qAsynchronousAction = new Queue<Action>();

	private void Awake()
	{
		instance = this;
    }
    private void Start()
    {
        Stage1Start();
    }
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{

        GameLose();
		}

    }
	public void AsynchronousExecution(IEnumerator enumerator) 
	{
		StartCoroutine(enumerator);
	}
	public void AsynchronousExecution(Queue<Action> qAsynchronousAction, int nOneFrameActionCount)
	{
		StartCoroutine(AsynchronousExecutors(qAsynchronousAction, nOneFrameActionCount));
	}
	private IEnumerator AsynchronousExecutors(Queue<Action> qAsynchronousAction, int nOneFrameActionCount)
	{
		while (qAsynchronousAction.Count != 0)
		{
			yield return null;
			int nCount = (qAsynchronousAction.Count > nOneFrameActionCount) ? nOneFrameActionCount : qAsynchronousAction.Count;
			for (int i = 0; i < nCount; i++)
			{
				qAsynchronousAction.Dequeue().Invoke();
			}
		}
	}
	public void Stage1Start() 
	{
		StartCoroutine(Stage1());
	}
	private IEnumerator Stage1() 
	{
		yield return null;
        List<string> options = new List<string>();
        int playersChoice;
        Player.instance.EnablePlayerInput(false);
        options.Add("¾Æ´Ï¿ä");
        options.Add("³×");
        yield return StartCoroutine(WaitForChoice("", "Æ©Åä¸®¾óÀ» °Ç³Ê¶Ù½Ã°Ú½À´Ï±î?", options));
        options.Clear();
        playersChoice = UIManager.instance.GetPlayersChoice();
        if (playersChoice == 0) 
        {
            yield return StartCoroutine(UIManager.instance.WaitForTutorial());
            yield return StartCoroutine(WaitForTalk("½ºÄ®·¿", "À¸À¸... ¿©±â°¡ ¾îµðÁö? ¾Ù¸®½º ¹«»çÇØ?"));
            yield return StartCoroutine(WaitForTalk("¾Ù¸®½º", "±¦Âú¾Æ. ¾ð´Ï´Â?"));
            yield return StartCoroutine(WaitForTalk("½ºÄ®·¿", "¸Ó¸®°¡ Á¶±Ý ¾ÆÇÁ±ä ÇÏÁö¸¸ ÂüÀ»¸¸ÇØ. ±×·±µ¥ ¾Ù¸®½º ¾îµð ÀÖ¾î?"));
            yield return StartCoroutine(WaitForTalk("¾Ù¸®½º", "³ª ¿©±â ÀÖ´Âµ¥ ¾È º¸¿©?"));
            yield return StartCoroutine(WaitForTalk("½ºÄ®·¿", "¾î¾î¾î ÀÌ°Ô ¾î¶»°Ô µÈ ÀÏÀÌÁö?!"));
            yield return StartCoroutine(WaitForTalk("¾Ù¸®½º", "Èì. ¾Æ¸¶µµ ¿ì¸®°¡ ÇÏ³ª°¡ µÈ °Å °°¾Æ"));
            yield return StartCoroutine(WaitForTalk("½ºÄ®·¿", "¹¹¾î¾î¾î¾î¾î¾î¾î¾î¾î¾î?!"));
            yield return StartCoroutine(WaitForTalk("¾Ù¸®½º", "ÀÏ´Ü Á» ÁøÁ¤ÇØ ¾ð´Ï"));
            yield return StartCoroutine(WaitForTalk("½ºÄ®·¿", "¾î¶»°Ô ÁøÁ¤À»---", 0.4f));
            EnemyManager.instance.OnActiveEnemy(StageEnemySet.WTD_tutorial);
            EnemyManager.instance.StopAllEnemy();
            CameraManager.instance.OnCamActive(CamType.WTD_tutorial);
            yield return StartCoroutine(WaitForTalk("¸ó½ºÅÍ", "³¢±ã"));
            yield return StartCoroutine(WaitForTalk("¾Ù¸®½º", "¾ð´Ï. ¸ó½ºÅÍ¾ß ÀüÅõ ÁØºñÇØ"));
            CameraManager.instance.OffCamActive();
            yield return StartCoroutine(WaitForTalk("½ºÄ®·¿", "Àú·± Àâ¸÷Àº ÇÑ ¹æ¿¡ Ã³¸®ÇØ ÁÖ°Ú¾î!!"));

		    UIManager.instance.OnTutorial(TutorialType.WTD_W);
            yield return StartCoroutine(WaitFotInputKey(KeyCode.W));
            Player.instance.cCurrentCharacter.UseSkillWithoutPressKey(SkillType.WSkill, new Vector3(8f, 0, 8f));
		    UIManager.instance.OffTutorial();
		    yield return new WaitForSeconds(0.3f);

            UIManager.instance.OnTutorial(TutorialType.WTD_Q);
            yield return StartCoroutine(WaitFotInputKey(KeyCode.Q));
            Player.instance.cCurrentCharacter.UseSkillWithoutPressKey(SkillType.QSkill, new Vector3(8f, 0, 8f));
            UIManager.instance.OffTutorial();
            yield return new WaitForSeconds(0.7f);

            UIManager.instance.OnTutorial(TutorialType.WTD_QW);
            yield return StartCoroutine(WaitFotInputKey(KeyCode.Space));
            UIManager.instance.OffTutorial();
            yield return new WaitForSeconds(0.3f);

            yield return StartCoroutine(WaitForTalk("½ºÄ®·¿", "ÇÏÇÏ! ºÃ´À³Ä! ÀÌ ½ºÄ®·¿ ´ÔÀÇ ½Ç·ÂÀ»!"));
            yield return StartCoroutine(WaitForTalk("¾Ù¸®½º", "¾ð´Ï. ¾ÆÁ÷ ¾È ³¡³µ¾î"));

            EnemyManager.instance.OnActiveEnemy(StageEnemySet.WGS_tutorial_1);
            EnemyManager.instance.StopAllEnemy();
            yield return StartCoroutine(WaitForTalk("¸ó½ºÅÍ", "³¢±ã"));
            yield return StartCoroutine(WaitForTalk("½ºÄ®·¿", "ÀÌ¹ø¿¡µµ ÇÑ ¹æ¿¡---", 0.4f));

            EnemyManager.instance.OnActiveEnemy(StageEnemySet.WGS_tutorial_2);
            EnemyManager.instance.StopAllEnemy();
            yield return StartCoroutine(WaitForTalk("¸ó½ºÅÍ", "³¢±ã ³¢±ã"));
            yield return StartCoroutine(WaitForTalk("½ºÄ®·¿", "¸ç, ¸î ¸¶¸®°¡ ¿Àµç ÀÌ ½ºÄ®·¿ ´ÔÀÌ---", 0.4f));

            EnemyManager.instance.OnActiveEnemy(StageEnemySet.WGS_tutorial_3);
            EnemyManager.instance.StopAllEnemy();
            yield return StartCoroutine(WaitForTalk("¸ó½ºÅÍ", "³¢±ã ³¢±ã ³¢±ã ³¢±ã ³¢±ã"));
            yield return StartCoroutine(WaitForTalk("½ºÄ®·¿", ".....")); 
            yield return StartCoroutine(WaitForTalk("¾Ù¸®½º", "¾ð´Ï?"));
            yield return StartCoroutine(WaitForTalk("½ºÄ®·¿", "....."));
            yield return StartCoroutine(WaitForTalk("¾Ù¸®½º", "???"));
            yield return StartCoroutine(WaitForTalk("½ºÄ®·¿", "¿ì¿¡¿¡¿¡¿¨ ÀÌ°Ç ³Ê¹« ¸¹ÀÝ¾Æ!!! ¾Ù¸®½º. ¾î¶»°Ô Á» ÇØÁà!"));
            yield return StartCoroutine(WaitForTalk("¾Ù¸®½º", "ÇÏ........."));

            UIManager.instance.OnTutorial(TutorialType.WGS_Tag);
            yield return StartCoroutine(WaitFotInputKey(KeyCode.Tab));
            Player.instance.cCurrentCharacter.UseSkillWithoutPressKey(SkillType.Tag);
            UIManager.instance.OffTutorial();
            EnemyManager.instance.StartActionAllEnemy();

            yield return new WaitForSeconds(0.5f);

            EnemyManager.instance.StopAllEnemy();
            UIManager.instance.OnTutorial(TutorialType.WGS_E);
            yield return StartCoroutine(WaitFotInputKey(KeyCode.E));
            EnemyManager.instance.StartActionAllEnemy();
            Player.instance.cCurrentCharacter.UseSkillWithoutPressKey(SkillType.ESkill, new Vector3(8f, 0, 8f));
            UIManager.instance.OffTutorial();
            yield return new WaitForSeconds(5f);

            yield return StartCoroutine(WaitForTalk("½ºÄ®·¿", "ÀßÇÑ´Ù ¾Ù¸®½º! ´Ù ¾µ¾î¹ö·Á!"));
            yield return StartCoroutine(WaitForTalk("¾Ù¸®½º", "....."));
            yield return StartCoroutine(WaitForTalk("½ºÄ®·¿", "¹¹¾ß ±× ´«ºûÀº!"));
            yield return StartCoroutine(WaitForTalk("¾Ù¸®½º", "....."));
            EnemyManager.instance.OnActiveEnemy(StageEnemySet.Stage1_1);
            EnemyManager.instance.StopAllEnemy();
            yield return StartCoroutine(WaitForTalk("½ºÄ®·¿", "Å©Èì! ¾Æ¹«Æ° ÀÌ³à¼®µé °è¼Ó ¸ô·Á¿À³×. ÀÌÁ¦ºÎÅÏ ³ªµµ Áø½ÉÀ¸·Î ½Î¿ö¾ß°Ú¾î"));
            yield return StartCoroutine(WaitForTalk("¾Ù¸®½º", "....."));
            yield return StartCoroutine(WaitForTalk("½ºÄ®·¿", "¾Æ ÁøÂ¥! Áö±Ý±îÁø Áø½ÉÀÌ ¾Æ´Ï¾ú´Ù°í!"));
        }

        UIManager.instance.OnStageUI(StageNumber.one);
        EnemyManager.instance.OnActiveEnemy(StageEnemySet.Stage1_1);
        EnemyManager.instance.StopAllEnemy();
        yield return new WaitForSeconds(3.5f);
        Player.instance.EnablePlayerInput(true);
        EnemyManager.instance.StartActionAllEnemy();
    }
	private IEnumerator WaitForTalk(string name, string script, float autoClickTime = -1f) 
	{
        yield return StartCoroutine(UIManager.instance.WaitForTalk(name, script, autoClickTime));
    }
    private IEnumerator WaitForChoice(string name, string script, List<string> Options)
    {
        yield return StartCoroutine(UIManager.instance.WaitForChoice(name, script, Options));
    }
    private IEnumerator WaitFotInputKey(KeyCode key) 
	{
		bool wait = true;
		while (wait) 
		{
			if (Input.GetKeyDown(key))
				wait = false; 
			yield return null;
		}
	}
    public void GameLose() 
    {
        CameraManager.instance.OnPlayerDie();
        UIManager.instance.OnPlayerDie();
    }
}

public enum Phase
{
	Phase_1,
	Phase_2,
	Phase_3
}
