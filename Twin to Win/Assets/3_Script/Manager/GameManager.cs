using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEditorInternal;
using TreeEditor;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	public Phase phase;
	public GameStage gameStage;
	private Queue<Action> qAsynchronousAction = new Queue<Action>();

	private void Awake()
	{
        if (instance == null) 
        {
            gameStage = GameStage.Title;
            instance = this;
            DontDestroyOnLoad(transform.parent.gameObject);
        }
        else Destroy(transform.parent.gameObject); 
    }
    private void Start()
    {
        Player.instance.SetPlayerHp(100f);
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
        options.Add("�ƴϿ�");
        options.Add("��");
        yield return StartCoroutine(WaitForChoice("", "Ʃ�丮���� �ǳʶٽðڽ��ϱ�?", options));
        playersChoice = UIManager.instance.GetPlayersChoice();
        if (playersChoice == 0) 
        {
            yield return StartCoroutine(UIManager.instance.WaitForTutorial());
            yield return StartCoroutine(WaitForTalk("��Į��", "����... ���Ⱑ �����? �ٸ��� ������?"));
            yield return StartCoroutine(WaitForTalk("�ٸ���", "������. ��ϴ�?"));
            yield return StartCoroutine(WaitForTalk("��Į��", "�Ӹ��� ���� ������ ������ ��������. �׷��� �ٸ��� ��� �־�?"));
            yield return StartCoroutine(WaitForTalk("�ٸ���", "�� ���� �ִµ� �� ����?"));
            yield return StartCoroutine(WaitForTalk("��Į��", "���� �̰� ��� �� ������?!"));
            yield return StartCoroutine(WaitForTalk("�ٸ���", "��. �Ƹ��� �츮�� �ϳ��� �� �� ����"));
            yield return StartCoroutine(WaitForTalk("��Į��", "�������������?!"));
            yield return StartCoroutine(WaitForTalk("�ٸ���", "�ϴ� �� ������ ���"));
            yield return StartCoroutine(WaitForTalk("��Į��", "��� ������---", 0.4f));
            EnemyManager.instance.OnActiveEnemy(StageEnemySet.WTD_tutorial);
            EnemyManager.instance.StopAllEnemy();
            CameraManager.instance.OnCamActive(CamType.WTD_tutorial);
            yield return StartCoroutine(WaitForTalk("����", "����"));
            yield return StartCoroutine(WaitForTalk("�ٸ���", "���. ���;� ���� �غ���"));
            CameraManager.instance.OffCamActive();
            yield return StartCoroutine(WaitForTalk("��Į��", "���� ����� �� �濡 ó���� �ְھ�!!"));

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

            yield return StartCoroutine(WaitForTalk("��Į��", "����! �ô���! �� ��Į�� ���� �Ƿ���!"));
            yield return StartCoroutine(WaitForTalk("�ٸ���", "���. ���� �� ������"));

            EnemyManager.instance.OnActiveEnemy(StageEnemySet.WGS_tutorial_1);
            EnemyManager.instance.StopAllEnemy();
            yield return StartCoroutine(WaitForTalk("����", "����"));
            yield return StartCoroutine(WaitForTalk("��Į��", "�̹����� �� �濡---", 0.4f));

            EnemyManager.instance.OnActiveEnemy(StageEnemySet.WGS_tutorial_2);
            EnemyManager.instance.StopAllEnemy();
            yield return StartCoroutine(WaitForTalk("����", "���� ����"));
            yield return StartCoroutine(WaitForTalk("��Į��", "��, �� ������ ���� �� ��Į�� ����---", 0.4f));

            EnemyManager.instance.OnActiveEnemy(StageEnemySet.WGS_tutorial_3);
            EnemyManager.instance.StopAllEnemy();
            yield return StartCoroutine(WaitForTalk("����", "���� ���� ���� ���� ����"));
            yield return StartCoroutine(WaitForTalk("��Į��", ".....")); 
            yield return StartCoroutine(WaitForTalk("�ٸ���", "���?"));
            yield return StartCoroutine(WaitForTalk("��Į��", "....."));
            yield return StartCoroutine(WaitForTalk("�ٸ���", "???"));
            yield return StartCoroutine(WaitForTalk("��Į��", "�쿡������ �̰� �ʹ� ���ݾ�!!! �ٸ���. ��� �� ����!"));
            yield return StartCoroutine(WaitForTalk("�ٸ���", "��........."));

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

            yield return StartCoroutine(WaitForTalk("��Į��", "���Ѵ� �ٸ���! �� �������!"));
            yield return StartCoroutine(WaitForTalk("�ٸ���", "....."));
            yield return StartCoroutine(WaitForTalk("��Į��", "���� �� ������!"));
            yield return StartCoroutine(WaitForTalk("�ٸ���", "....."));
            EnemyManager.instance.OnActiveEnemy(StageEnemySet.Stage1_1);
            EnemyManager.instance.StopAllEnemy();
            yield return StartCoroutine(WaitForTalk("��Į��", "ũ��! �ƹ�ư �̳༮�� ��� ��������. �������� ���� �������� �ο��߰ھ�"));
            yield return StartCoroutine(WaitForTalk("�ٸ���", "....."));
            yield return StartCoroutine(WaitForTalk("��Į��", "�� ��¥! ���ݱ��� ������ �ƴϾ��ٰ�!"));
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
        Options.Clear();
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
        print("����");
        StartCoroutine(Lose());
    }
    private IEnumerator Lose()
    {
        CameraManager.instance.OnCamActive(CamType.PlayerDie, 0); // ��Ʈ ī�޶� ����
        Time.timeScale = 0.2f; // Ÿ�ӽ����� ���̱�
        yield return new WaitForSeconds(1.5f);
        Time.timeScale = 1f; // Ÿ�ӽ����� ����
        UIManager.instance.OnPlayerDie();// ȭ�� ���̵� �ƿ�
        yield return new WaitForSeconds(3.5f);

        int playersChoice;  // ������
        List<string> options = new List<string>();

        options.Add("�ƴϿ�"); options.Add("��");
        yield return StartCoroutine(WaitForChoice("", "��ȭ�� �ǳʶٽðڽ��ϱ�?", options));
        playersChoice = UIManager.instance.GetPlayersChoice();

        if (playersChoice == 0)
        {
            yield return StartCoroutine(WaitForTalk("", "����� ������ ��������� �� ���׿�", 2.5f));

            options.Add("�ƴϿ�"); options.Add("��");
            yield return StartCoroutine(WaitForChoice("", "���������� �����̾����ϱ�?", options));
            playersChoice = UIManager.instance.GetPlayersChoice();

            if (playersChoice == 0)
                yield return StartCoroutine(WaitForTalk("", "�׷��̱��� �װ��� �����̳׿�", 2.5f));
            else
                yield return StartCoroutine(WaitForTalk("", "�����ϼ̴ٴ� ���� �����̳׿�", 2.5f));

            yield return StartCoroutine(WaitForTalk("", "�̴�ζ�� ����� ���� ��ȭ�ο� ���� �������� �׷�����Ʈ�� ��ϵ� ���̸�", 2.5f));
        }

        options.Add("��������         [Ÿ��Ʋ��]"); options.Add("�׷� �� �Ⱦ��    [�ٽ��ϱ�]");
        yield return StartCoroutine(WaitForChoice("", "�� ������ ����� ����ϴ� ����� ���� ���Դϴ�", options));
        playersChoice = UIManager.instance.GetPlayersChoice();

        if (playersChoice == 0) { 
            yield return StartCoroutine(WaitForTalk("", "�׷�����. �׷� ������ �� ���ñ�", 2.5f));
            GoTitle();
        }
        else { 
            yield return StartCoroutine(WaitForTalk("", "�׷��ٸ� �̹��� �ε� ���ϴ� �ٸ� �̷�ñ�", 2.5f));
            GameStart();
        }
    }
    public void GoTitle()
    {
        SceneManager.LoadScene(0);
        gameStage = GameStage.Title;
        StartCoroutine(GoTitleCo());
    }
    private IEnumerator GoTitleCo()
    {
        yield return null;

        UIManager.instance.SetTitle();
        CameraManager.instance.SetTitle();
        EnemyManager.instance.SetTitle();
        EffectManager.instance.SetTitle();
    }
    public void GameStart() 
    {
        SceneManager.LoadScene(1);
        StartCoroutine(GameStartCo());
    }
    private IEnumerator GameStartCo() 
    {
        yield return null;

        StageManager.instance.UpdateNavMeshOne();
        UIManager.instance.SetGame();
        Player.instance.SetGame();
        CameraManager.instance.SetGame();
        EnemyManager.instance.SetGame();
        EffectManager.instance.SetGame();

        yield return null;
        gameStage = GameStage.Game;
        Stage1Start();
    }
}

public enum Phase
{
	Phase_1,
	Phase_2,
	Phase_3
}
public enum GameStage 
{
    Title, Game
}