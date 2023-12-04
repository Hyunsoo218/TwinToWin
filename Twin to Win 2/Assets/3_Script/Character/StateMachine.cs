using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
	public State CurrentState { get; private set; }
	public State PrevState { get; private set; }

	private void Update()
	{
		CurrentState?.onStay?.Invoke();
	}
	public void ChangeState(State cNextState)
	{
		if (CurrentState != null) {
			CurrentState?.onExit?.Invoke();
			PrevState = CurrentState;
		}
		CurrentState = cNextState;
		CurrentState?.onEnter?.Invoke();
	}
}
