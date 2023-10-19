using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Unity.VisualScripting;
//using UnityEditorInternal;
//using TreeEditor;
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
        //StartCoroutine(CutSceneManager.instance.PlayCutScene(CutSceneType.Stage1to2));
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

        options.Add("아니요");
        options.Add("네");
        yield return StartCoroutine(WaitForChoice("", "튜토리얼을 건너뛰시겠습니까?", options));
        playersChoice = UIManager.instance.GetPlayersChoice();
        if (playersChoice == 0) 
        {
            yield return StartCoroutine(UIManager.instance.WaitForTutorial());
            yield return StartCoroutine(WaitForTalk("스칼렛", "으으... 여기가 어디지? 앨리스 무사해?"));
            yield return StartCoroutine(WaitForTalk("앨리스", "괜찮아. 언니는?"));
            yield return StartCoroutine(WaitForTalk("스칼렛", "머리가 조금 아프긴 하지만 참을만해. 그런데 앨리스 어디 있어?"));
            yield return StartCoroutine(WaitForTalk("앨리스", "나 여기 있는데 안 보여?"));
            yield return StartCoroutine(WaitForTalk("스칼렛", "어어어 이게 어떻게 된 일이지?!"));
            yield return StartCoroutine(WaitForTalk("앨리스", "흠. 아마도 우리가 하나가 된 거 같아"));
            yield return StartCoroutine(WaitForTalk("스칼렛", "뭐어어어어어어어어어어?!"));
            yield return StartCoroutine(WaitForTalk("앨리스", "일단 좀 진정해 언니"));
            yield return StartCoroutine(WaitForTalk("스칼렛", "어떻게 진정을---", 0.4f));
            EnemyManager.instance.OnActiveEnemy(TutorialEnemySet.WTD_tutorial);
            EnemyManager.instance.StopAllEnemy();
            CameraManager.instance.OnCamActive(CamType.WTD_tutorial, 2f);
            yield return StartCoroutine(WaitForTalk("몬스터", "끼긱"));
            yield return StartCoroutine(WaitForTalk("앨리스", "언니. 몬스터야 전투 준비해"));
            CameraManager.instance.OffCamActive();
            yield return StartCoroutine(WaitForTalk("스칼렛", "저런 잡몹은 한 방에 처리해 주겠어!!"));

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

            yield return StartCoroutine(WaitForTalk("스칼렛", "하하! 봤느냐! 이 스칼렛 님의 실력을!"));
            yield return StartCoroutine(WaitForTalk("앨리스", "언니. 아직 안 끝났어"));

            EnemyManager.instance.OnActiveEnemy(TutorialEnemySet.WGS_tutorial_1);
            EnemyManager.instance.StopAllEnemy();
            yield return StartCoroutine(WaitForTalk("몬스터", "끼긱"));
            yield return StartCoroutine(WaitForTalk("스칼렛", "이번에도 한 방에---", 0.4f));

            EnemyManager.instance.OnActiveEnemy(TutorialEnemySet.WGS_tutorial_2);
            EnemyManager.instance.StopAllEnemy();
            yield return StartCoroutine(WaitForTalk("몬스터", "끼긱 끼긱"));
            yield return StartCoroutine(WaitForTalk("스칼렛", "며, 몇 마리가 오든 이 스칼렛 님이---", 0.4f));

            EnemyManager.instance.OnActiveEnemy(TutorialEnemySet.WGS_tutorial_3);
            EnemyManager.instance.StopAllEnemy();
            yield return StartCoroutine(WaitForTalk("몬스터", "끼긱 끼긱 끼긱 끼긱 끼긱"));
            yield return StartCoroutine(WaitForTalk("스칼렛", ".....")); 
            yield return StartCoroutine(WaitForTalk("앨리스", "언니?"));
            yield return StartCoroutine(WaitForTalk("스칼렛", "....."));
            yield return StartCoroutine(WaitForTalk("앨리스", "???"));
            yield return StartCoroutine(WaitForTalk("스칼렛", "우에에에엥 이건 너무 많잖아!!! 앨리스. 어떻게 좀 해줘!"));
            yield return StartCoroutine(WaitForTalk("앨리스", "하........."));

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

            yield return StartCoroutine(WaitForTalk("스칼렛", "잘한다 앨리스! 다 쓸어버려!"));
            yield return StartCoroutine(WaitForTalk("앨리스", "....."));
            yield return StartCoroutine(WaitForTalk("스칼렛", "뭐야 그 눈빛은!"));
            yield return StartCoroutine(WaitForTalk("앨리스", "....."));
            EnemyManager.instance.OnActiveEnemy(Stage1EnemySet.Stage1_1);
            EnemyManager.instance.StopAllEnemy();
            yield return StartCoroutine(WaitForTalk("스칼렛", "크흠! 아무튼 이녀석들 계속 몰려오네. 이제부턴 나도 진심으로 싸워야겠어"));
            yield return StartCoroutine(WaitForTalk("앨리스", "....."));
            yield return StartCoroutine(WaitForTalk("스칼렛", "아 진짜! 지금까진 진심이 아니었다고!"));
        }

        phase = Phase.Phase_1;
        EnemyManager.instance.SetStage(phase);

        UIManager.instance.OnStageUI(StageNumber.one);

        EnemyManager.instance.OnActiveEnemy(Stage1EnemySet.Stage1_1);
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
        StartCoroutine(Lose());
    }
    private IEnumerator Lose()
    {
        CameraManager.instance.OnCamActive(CamType.PlayerDie, 0); // 히트 카메라 무빙
        Time.timeScale = 0.2f; // 타임스케일 줄이기
        yield return new WaitForSeconds(1.5f);
        Time.timeScale = 1f; // 타임스케일 복귀
        UIManager.instance.OnPlayerDie();// 화면 페이드 아웃
        yield return new WaitForSeconds(3.5f);

        EffectManager.instance.DisableAllEffect();
        EnemyManager.instance.StopAllEnemy();

        int playersChoice;  // 선택지
        List<string> options = new List<string>();

        yield return StartCoroutine(WaitForTalk("", "모험이 도중에 중단되었습니다", 3f));

        options.Add("아니요"); options.Add("네");
        yield return StartCoroutine(WaitForChoice("", "다시 플레이 하시겠습니까?", options));
        playersChoice = UIManager.instance.GetPlayersChoice();

        if (playersChoice == 0) { 
            yield return StartCoroutine(WaitForTalk("", "플레이해 주셔서 감사합니다. 안녕히 가십시오", 2.5f));
            GoTitle();
        }
        else {
            yield return StartCoroutine(WaitForTalk("", "모험을 다시 시작합니다", 2f));
            yield return StartCoroutine(WaitForTalk("", "3", 1f));
            yield return StartCoroutine(WaitForTalk("", "2", 1f));
            yield return StartCoroutine(WaitForTalk("", "1", 1f));
            GameStart();
        }
    }
    public void GoTitle()
    {
        SceneManager.LoadScene(0);
        gameStage = GameStage.Title;
        StartCoroutine(GoTitleCo());
    }
    public void GameStart() 
    {
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
        Player.instance.SetGame();
        CameraManager.instance.SetGame();
        EnemyManager.instance.SetGame();
        EnemyManager.instance.SetStage(phase);
        CutSceneManager.instance.SetGame();

        Player.instance.EnablePlayerInput(false);

        UIManager.instance.ActiveLodingUI(true);
        yield return StartCoroutine(EffectManager.instance.SetGame());
        UIManager.instance.ActiveLodingUI(false);
        gameStage = GameStage.Game;
        Stage1Start();
    }
    public void StageClear() 
    {
        EffectManager.instance.DisableAllEffect();
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
        Player.instance.EnablePlayerInput(false);
        yield return new WaitForSeconds(2f);

        Player.instance.cCurrentCharacter.gameObject.SetActive(false);
        yield return StartCoroutine(CutSceneManager.instance.PlayCutScene(CutSceneType.Stage1to2));

        UIManager.instance.OnStageUI(StageNumber.twe);
        Player.instance.cCurrentCharacter.gameObject.SetActive(true);
        EnemyManager.instance.OnActiveEnemy(Stage2EnemySet.Boss_normal);
        EnemyManager.instance.StopAllEnemy();
        yield return new WaitForSeconds(3.5f);

        Player.instance.EnablePlayerInput(true);
        EnemyManager.instance.StartActionAllEnemy();
    }
    private IEnumerator Stage2to3 ()
    {
        Player.instance.EnablePlayerInput(false);
        yield return new WaitForSeconds(2f);

        Player.instance.cCurrentCharacter.gameObject.SetActive(false);
        yield return StartCoroutine(CutSceneManager.instance.PlayCutScene(CutSceneType.Stage2to3));

        UIManager.instance.OnStageUI(StageNumber.three);
        Player.instance.cCurrentCharacter.gameObject.SetActive(true);
        EnemyManager.instance.OnActiveEnemy(Stage3EnemySet.Boss_angry);
        EnemyManager.instance.StopAllEnemy();
        yield return new WaitForSeconds(3.5f);

        Player.instance.EnablePlayerInput(true);
        EnemyManager.instance.StartActionAllEnemy();
    }
    private IEnumerator GameClear() 
    {
        Player.instance.EnablePlayerInput(false);
        Time.timeScale = 0.2f; // 타임스케일 줄이기
        yield return new WaitForSeconds(1f);
        Time.timeScale = 1f; // 타임스케일 복귀
        UIManager.instance.OnGameClear(); // 화면 페이드 아웃
        yield return new WaitForSeconds(5.5f);

        yield return StartCoroutine(WaitForTalk("", "축하드립니다. 모험을 성공적으로 마쳤습니다", 3f));

        int playersChoice;  // 선택지
        List<string> options = new List<string>();

        options.Add("아니요"); options.Add("네");
        yield return StartCoroutine(WaitForChoice("", "다시 플레이 하시겠습니까?", options));
        playersChoice = UIManager.instance.GetPlayersChoice();

        if (playersChoice == 0)
        {
            yield return StartCoroutine(WaitForTalk("", "플레이해 주셔서 감사합니다. 안녕히 가십시오", 2.5f));
            GoTitle();
        }
        else
        {
            yield return StartCoroutine(WaitForTalk("", "모험을 다시 시작합니다", 2f));
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