using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
	private State cCurrentState;

	private void Update()
	{
		cCurrentState?.onStay?.Invoke();
	}
	public void ChangeState(State cNextState)
	{
		if (cCurrentState != null) {
			cCurrentState.onExit?.Invoke();
		}
		cCurrentState = cNextState;
		cCurrentState.onEnter?.Invoke();
	}
	public State GetCurrentState()
	{
		return cCurrentState;
	}
}
