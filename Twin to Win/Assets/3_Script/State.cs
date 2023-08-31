using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class State
{
	public string strStateName;
	public Action onEnter;
	public Action onStay;
	public Action onExit;
	public State(string strStateName)
	{
		this.strStateName = strStateName;
	}
}
