using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
	[SerializeField] protected float fRunTime = 3f;
	[SerializeField] protected AudioSource soundComponent;
	[SerializeField] protected AudioClip clip;
	[SerializeField] protected bool criticalHit = false;

	protected List<Animator> animators = new List<Animator>();
	protected List<ParticleSystem> particles = new List<ParticleSystem>();
	protected float _MasterSpeed;
	protected float MasterSpeed
	{
		get
		{
			return Time.timeScale * _MasterSpeed;
		}
		set
		{
			_MasterSpeed = value;
			foreach (var item in animators) 
				item.speed = MasterSpeed;
			foreach (var item in particles) {
				ParticleSystem.MainModule targetMain = item.main;
				targetMain.simulationSpeed = MasterSpeed;
			}
		}
	}

	public virtual void Initialize()
	{
		Animator animator;
		Animator[] inChildrenAnimators = GetComponentsInChildren<Animator>();
		ParticleSystem particle;
		ParticleSystem[] inChildrenParticles = GetComponentsInChildren<ParticleSystem>();
		if (TryGetComponent<Animator>(out animator)) animators.Add(animator);
		if (TryGetComponent<ParticleSystem>(out particle)) particles.Add(particle);
		foreach (var item in inChildrenAnimators) animators.Add(item);
		foreach (var item in inChildrenParticles) particles.Add(item);
		MasterSpeed = 1f;
	}
	protected virtual void OnEnable() => StartCoroutine(InPool());
	public virtual void OnAction(Transform tUser, float fDamage, int nTargetLayer) => print("함수를 오버라이드 하세요");
	public virtual void Slow(float slowAmount) => MasterSpeed = slowAmount;
	public virtual void SlowEnd() => MasterSpeed = 1f;
	protected IEnumerator InPool() 
	{
		float currentTime = 0;
		while (currentTime < fRunTime)
		{
			currentTime += Time.deltaTime * MasterSpeed;
			yield return null;
		}
		InPoolEvent();
	}
	protected virtual void InPoolEvent() 
	{
		gameObject.SetActive(false);
	}
}
