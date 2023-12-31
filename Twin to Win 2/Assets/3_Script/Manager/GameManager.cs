using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Unity.VisualScripting;
//using UnityEditorInternal;
//using TreeEditor;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

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
        Player.Instance.EnablePlayerInput(false);
        GoTitle();
    }
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && gameStage == GameStage.Game)
		{
            UIManager.instance.ActiveStopUI();
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

      //  options.Add("�ƴϿ�");
      //  options.Add("��");
      //  yield return StartCoroutine(WaitForChoice("", "Ʃ�丮���� �ǳʶٽðڽ��ϱ�?", options));
      //  playersChoice = UIManager.instance.GetPlayersChoice();
      //  if (playersChoice == 0) 
      //  {
      //      yield return StartCoroutine(UIManager.instance.WaitForTutorial());
      //      yield return StartCoroutine(WaitForTalk("��Į��", "���� �ϴü��� �����ߴ�!"));
      //      yield return StartCoroutine(WaitForTalk("�ٸ���", "���. ������ �������� ������"));
      //      yield return StartCoroutine(WaitForTalk("��Į��", "����. ������ ������ ����������!"));
      //      yield return StartCoroutine(WaitForTalk("�ٸ���", "�Ͼ� �� ��ϸ� ��¼�� ����"));
      //      yield return StartCoroutine(WaitForTalk("��Į��", "�ٸ���. �׷��� �Ѽ��� ���� �ָ��� �����?"));
      //      EnemyManager.instance.OnActiveEnemy(TutorialEnemySet.WTD_tutorial);
      //      EnemyManager.instance.StopAllEnemy();
      //      CameraManager.instance.OnCamActive(CamType.WTD_tutorial, 2f);
      //      yield return StartCoroutine(WaitForTalk("����", "����"));
      //      yield return StartCoroutine(WaitForTalk("�ٸ���", "���. ���;� ���� �غ���"));
      //      CameraManager.instance.OffCamActive();
      //      yield return StartCoroutine(WaitForTalk("��Į��", "���� ����� �� �濡 ó���� �ְھ�!!"));

		    //UIManager.instance.OnTutorial(TutorialType.WTD_W);
      //      yield return StartCoroutine(WaitFotInputKey(KeyCode.W));
      //      Player.Instance.CurrentCharacter.UseSkillWithoutPressKey(SkillType.W, new Vector3(8f, 0, 8f));
		    //UIManager.instance.OffTutorial();
		    //yield return new WaitForSeconds(0.3f);

      //      UIManager.instance.OnTutorial(TutorialType.WTD_Q);
      //      yield return StartCoroutine(WaitFotInputKey(KeyCode.Q));
      //      Player.Instance.CurrentCharacter.UseSkillWithoutPressKey(SkillType.Q, new Vector3(8f, 0, 8f));
      //      UIManager.instance.OffTutorial();
      //      yield return new WaitForSeconds(0.7f);

      //      UIManager.instance.OnTutorial(TutorialType.WTD_QW);
      //      yield return StartCoroutine(WaitFotInputKey(KeyCode.Space));
      //      UIManager.instance.OffTutorial();
      //      yield return new WaitForSeconds(0.3f);

      //      yield return StartCoroutine(WaitForTalk("��Į��", "����! �ô���! �� ��Į�� ���� �Ƿ���!"));
      //      yield return StartCoroutine(WaitForTalk("�ٸ���", "���. ���� �� ������"));

      //      EnemyManager.instance.OnActiveEnemy(TutorialEnemySet.WGS_tutorial_1);
      //      EnemyManager.instance.StopAllEnemy();
      //      yield return StartCoroutine(WaitForTalk("����", "����"));
      //      yield return StartCoroutine(WaitForTalk("��Į��", "�̹����� �� �濡---", 0.4f));

      //      EnemyManager.instance.OnActiveEnemy(TutorialEnemySet.WGS_tutorial_2);
      //      EnemyManager.instance.StopAllEnemy();
      //      yield return StartCoroutine(WaitForTalk("����", "���� ����"));
      //      yield return StartCoroutine(WaitForTalk("��Į��", "��, �� ������ ���� �� ��Į�� ����---", 0.4f));

      //      EnemyManager.instance.OnActiveEnemy(TutorialEnemySet.WGS_tutorial_3);
      //      EnemyManager.instance.StopAllEnemy();
      //      yield return StartCoroutine(WaitForTalk("����", "���� ���� ���� ���� ����"));
      //      yield return StartCoroutine(WaitForTalk("��Į��", ".....")); 
      //      yield return StartCoroutine(WaitForTalk("�ٸ���", "���?"));
      //      yield return StartCoroutine(WaitForTalk("��Į��", "....."));
      //      yield return StartCoroutine(WaitForTalk("�ٸ���", "???"));
      //      yield return StartCoroutine(WaitForTalk("��Į��", "�쿡������ �̰� �ʹ� ���ݾ�!!! �ٸ���. ��� �� ����!"));
      //      yield return StartCoroutine(WaitForTalk("�ٸ���", "��........."));

      //      UIManager.instance.OnTutorial(TutorialType.WGS_Tag);
      //      yield return StartCoroutine(WaitFotInputKey(KeyCode.Tab));
      //      Player.Instance.DoTag();
      //      UIManager.instance.OffTutorial();

      //      yield return new WaitForSeconds(0.2f);

      //      UIManager.instance.OnTutorial(TutorialType.WGS_E);
      //      yield return StartCoroutine(WaitFotInputKey(KeyCode.E));
      //      EnemyManager.instance.StartActionAllEnemy();
      //      Player.Instance.CurrentCharacter.UseSkillWithoutPressKey(SkillType.E, new Vector3(8f, 0, 8f));
      //      UIManager.instance.OffTutorial();
      //      yield return new WaitForSeconds(5f);

      //      yield return StartCoroutine(WaitForTalk("��Į��", "���Ѵ� �ٸ���! �� �������!"));
      //      yield return StartCoroutine(WaitForTalk("�ٸ���", "....."));
      //      yield return StartCoroutine(WaitForTalk("��Į��", "���� �� ������!"));
      //      yield return StartCoroutine(WaitForTalk("�ٸ���", "....."));
      //      EnemyManager.instance.OnActiveEnemy(Stage1EnemySet.Stage1_1);
      //      EnemyManager.instance.StopAllEnemy();
      //      yield return StartCoroutine(WaitForTalk("��Į��", "ũ��! �ƹ�ư �̳༮�� ��� ��������. �������� ���� �������� �ο��߰ھ�"));
      //      yield return StartCoroutine(WaitForTalk("�ٸ���", "....."));
      //      yield return StartCoroutine(WaitForTalk("��Į��", "�� ��¥! ���ݱ��� ������ �ƴϾ��ٰ�!"));
      //  }

        phase = Phase.Phase_1;
        EnemyManager.instance.SetStage(phase);

        UIManager.instance.OnStageUI(StageNumber.one);

        EnemyManager.instance.OnActiveEnemy(Stage1EnemySet.Stage1_1);
        EnemyManager.instance.StopAllEnemy();
        yield return new WaitForSeconds(3.5f);
        Player.Instance.EnablePlayerInput(true);
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

        EffectManager.instance.DisableAllEffect();
        EnemyManager.instance.StopAllEnemy();

        int playersChoice;  // ������
        List<string> options = new List<string>();

        yield return StartCoroutine(WaitForTalk("", "������ ���߿� �ߴܵǾ����ϴ�", 3f));

        options.Add("�ƴϿ�"); options.Add("��");
        yield return StartCoroutine(WaitForChoice("", "�ٽ� �÷��� �Ͻðڽ��ϱ�?", options));
        playersChoice = UIManager.instance.GetPlayersChoice();

        if (playersChoice == 0) { 
            yield return StartCoroutine(WaitForTalk("", "�÷����� �ּż� �����մϴ�. �ȳ��� ���ʽÿ�", 2.5f));
            GoTitle();
        }
        else {
            yield return StartCoroutine(WaitForTalk("", "������ �ٽ� �����մϴ�", 2f));
            yield return StartCoroutine(WaitForTalk("", "3", 1f));
            yield return StartCoroutine(WaitForTalk("", "2", 1f));
            yield return StartCoroutine(WaitForTalk("", "1", 1f));
            GameStart();
        }
    }
    public void GoTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        gameStage = GameStage.Title;
        StartCoroutine(GoTitleCo());
    }
    public void GameStart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        SceneManager.LoadScene(1);
        StartCoroutine(GameStartCo());
    }
    private IEnumerator GoTitleCo()
    {
        yield return null;

        UIManager.instance.SetTitle();
        CameraManager.instance.SetTitle();
        EnemyManager.instance.SetTitle();
        EffectManager.instance.SetTitle();
        CutSceneManager.instance.SetTitle();
    }
    private IEnumerator GameStartCo()
    {
        yield return null;

        phase = Phase.Tutorial;
        StageManager.instance.UpdateNavMeshOne();
        UIManager.instance.SetGame();
        Player.Instance.SetGame();
        CameraManager.instance.SetGame();
        EnemyManager.instance.SetGame();
        EnemyManager.instance.SetStage(phase);
        CutSceneManager.instance.SetGame();

        Player.Instance.EnablePlayerInput(false);

        UIManager.instance.ActiveLodingUI(true);
        yield return StartCoroutine(EffectManager.instance.SetGame());
        UIManager.instance.ActiveLodingUI(false);
        gameStage = GameStage.Game;
        Stage1Start();
    }
    public void StageClear()
    {
        //EffectManager.instance.DisableAllEffect();
		switch (phase)
		{
			case Phase.Phase_1:
                phase = Phase.Phase_2;
                EnemyManager.instance.SetStage(phase);
                StartCoroutine(Stage1to2());
                break;
			case Phase.Phase_2:
                phase = Phase.Phase_3;
                EnemyManager.instance.SetStage(phase);
                StartCoroutine(Stage2to3());
                break;
			case Phase.Phase_3:
                phase = Phase.Phase_1;
                EnemyManager.instance.SetStage(phase);
                StartCoroutine(GameClear());
                break;
            case Phase.Tutorial:
                phase = Phase.Phase_1;
                EnemyManager.instance.SetStage(phase);
                break;
		}
	}
    private IEnumerator Stage1to2()
    {
        if (true)
        {

        }
        /*
        Player.Instance.EnablePlayerInput(false);
        yield return new WaitForSeconds(2f);

        Player.Instance.CurrentCharacter.gameObject.SetActive(false);
        yield return StartCoroutine(CutSceneManager.instance.PlayCutScene(CutSceneType.Stage1to2));
        */
        yield return null;
        UIManager.instance.OnStageUI(StageNumber.twe);
        //Player.Instance.CurrentCharacter.gameObject.SetActive(true);
        EnemyManager.instance.OnActiveEnemy(Stage2EnemySet.Boss_normal);
        //EnemyManager.instance.StopAllEnemy();
        //yield return new WaitForSeconds(3.5f);

        //Player.Instance.EnablePlayerInput(true);
        //EnemyManager.instance.StartActionAllEnemy();
    }
    private IEnumerator Stage2to3()
    {
        /*
        Player.Instance.EnablePlayerInput(false);
        yield return new WaitForSeconds(2f);

        Player.Instance.CurrentCharacter.gameObject.SetActive(false);
        yield return StartCoroutine(CutSceneManager.instance.PlayCutScene(CutSceneType.Stage2to3));
        */
        UIManager.instance.OnStageUI(StageNumber.three);
        Player.Instance.CurrentCharacter.gameObject.SetActive(true);
        EnemyManager.instance.OnActiveEnemy(Stage3EnemySet.Boss_angry);
        EnemyManager.instance.StopAllEnemy();
        yield return new WaitForSeconds(3.5f);

        Player.Instance.EnablePlayerInput(true);
        EnemyManager.instance.StartActionAllEnemy();
    }
    private IEnumerator GameClear()
    {
        Player.Instance.EnablePlayerInput(false);
        Time.timeScale = 0.2f; // Ÿ�ӽ����� ���̱�
        yield return new WaitForSeconds(1f);
        Time.timeScale = 1f; // Ÿ�ӽ����� ����
        UIManager.instance.OnGameClear(); // ȭ�� ���̵� �ƿ�
        yield return new WaitForSeconds(5.5f);

        yield return StartCoroutine(WaitForTalk("", "���ϵ帳�ϴ�. ������ ���������� ���ƽ��ϴ�", 3f));

        int playersChoice;  // ������
        List<string> options = new List<string>();

        options.Add("�ƴϿ�"); options.Add("��");
        yield return StartCoroutine(WaitForChoice("", "�ٽ� �÷��� �Ͻðڽ��ϱ�?", options));
        playersChoice = UIManager.instance.GetPlayersChoice();

        if (playersChoice == 0)
        {
            yield return StartCoroutine(WaitForTalk("", "�÷����� �ּż� �����մϴ�. �ȳ��� ���ʽÿ�", 2.5f));
            GoTitle();
        }
        else
        {
            yield return StartCoroutine(WaitForTalk("", "������ �ٽ� �����մϴ�", 2f));
            yield return StartCoroutine(WaitForTalk("", "3", 1f));
            yield return StartCoroutine(WaitForTalk("", "2", 1f));
            yield return StartCoroutine(WaitForTalk("", "1", 1f));
            GameStart();
        }
    }
}

public enum Phase
{
	Phase_1,
	Phase_2,
	Phase_3,
    Tutorial
}
public enum GameStage 
{
    Title, Game
}