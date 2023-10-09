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
    [SerializeField] private bool fastDebug = false;
	private Queue<Action> qAsynchronousAction = new Queue<Action>();
	private void Awake()
	{
		instance = this;
    }
    private void Start()
    {
        Stage1Start();
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
        Player.instance.EnableCurrentPlayerInput(false);

        if (!fastDebug) 
        { 
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
            yield return new WaitForSeconds(0.5f);

            UIManager.instance.OnTutorial(TutorialType.WGS_E);
            yield return StartCoroutine(WaitFotInputKey(KeyCode.E));
            Player.instance.cCurrentCharacter.UseSkillWithoutPressKey(SkillType.ESkill, new Vector3(8f, 0, 8f));
            UIManager.instance.OffTutorial();
            yield return new WaitForSeconds(5.5f);

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
        yield return new WaitForSeconds(3.5f);

        yield return StartCoroutine(UIManager.instance.WaitForTutorial());

        Player.instance.EnableCurrentPlayerInput(true);
        EnemyManager.instance.StartActionAllEnemy();
    }
	private IEnumerator WaitForTalk(string name, string script, float autoClickTime = -1f) 
	{
        yield return StartCoroutine(UIManager.instance.WaitForTalk(name, script, autoClickTime));
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
}

public enum Phase
{
	Phase_1,
	Phase_2,
	Phase_3
}
