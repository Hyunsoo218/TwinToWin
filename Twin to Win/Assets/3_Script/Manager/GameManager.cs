using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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
        UIManager.instance.OnStageUI(StageNumber.one);
		EnemyManager.instance.OnActiveEnemy(StageEnemySet.Stage1_1);
        foreach (var item in MonsterCharacter.allMonsterCharacters)  item.StopAction(); 
		yield return new WaitForSeconds(3.5f);
		// �������� ǥ���ϰ� ���� ��ȯ, ���� ���� ��Ű��, �÷��̾� �Է�x

        yield return StartCoroutine(UIManager.instance.WaitForTutorial());
        // �÷��̾� Ʃ�丮�� ���� ������ ���� ���			

        Player.instance.EnableCurrentPlayerInput(true);
		foreach (var item in MonsterCharacter.allMonsterCharacters) item.StartAction(); 
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
