using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{ 
	[SerializeField] protected float fHealthPoint;
	[SerializeField] protected float fPower;
	protected StateMachine cStateMachine;
	protected Animator cAnimator;
	public abstract void Move();
	public abstract void Attack();
	public abstract void Damage();
	public abstract void Die();
	public abstract void ChangeState();
}