using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Character : MonoBehaviour
{ 
	protected StateMachine cStateMachine;
	protected Animator cAnimator;
	protected NavMeshAgent agent;
	public abstract void Move(Vector3 targetPos);
	public abstract void Attack(Vector3 targetPos);
	public abstract void Damage(float fAmount);
	public abstract void Die();
	protected abstract void ChangeState(State cNextState);
	protected abstract void ChangeAnimation(string strTrigger);
}