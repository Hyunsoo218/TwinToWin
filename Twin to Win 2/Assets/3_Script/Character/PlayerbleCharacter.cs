using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum SkillType
{
    Q, W, E, R
};
[Serializable]
public class Skill
{
    public GameObject effect;
    public SkillTimeInfo time;
}
[Serializable]
public struct SkillTimeInfo
{
    public SkillTimeInfo(float maxTime, float currentTime)
    {
        this.max = maxTime;
        this.current = currentTime;
        percentage = 1f - currentTime / maxTime;
    }
    public float max;
    public float current;
    [HideInInspector] public float percentage;
}
public class PlayerbleCharacter : Character
{
    [SerializeField] protected GameObject objMouseClickEffect;
    [SerializeField] protected Skill Skill_Q;
    [SerializeField] protected Skill Skill_W;
    [SerializeField] protected Skill Skill_E;
    [SerializeField] protected Skill Skill_R;
    [SerializeField] protected List<GameObject> normalAttackEffects;
    [SerializeField] protected Transform modelRoot;
    protected GameObject nextEffect;
    protected State idleState;
    protected State moveState;
    protected State dodgeState;
    protected State qSkillState;
    protected State wSkillState;
    protected State eSkillState;
    protected State rSkillState;
    protected State dieState;
    protected List<State> normalAttack = new List<State>();
    protected bool canMove = true;
    protected bool canAttack = true;
    protected bool canDodge = true;
    protected bool isDie = false;
    protected bool canTag = true;
    protected bool canDamage = true;
    protected bool canSkill = true;
    protected int attackCount = -1;


    #region Initalize
    protected void Awake()
    {
        cStateMachine = GetComponent<StateMachine>();
        cAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        Skill_Q.time.current = Skill_Q.time.max;
        Skill_W.time.current = Skill_W.time.max;
        Skill_E.time.current = Skill_E.time.max;

        StateInitalize();
        StateInitalizeOnEnter();
        StateInitalizeOnStay();
        StateInitalizeOnExit();

        cStateMachine.ChangeState(idleState);
    }
    protected virtual void StateInitalize()
    {
        idleState = new State("Idle");
        moveState = new State("Move");
        dodgeState = new State("Dodge");
        qSkillState = new State("Skill_Q");
        wSkillState = new State("Skill_W");
        eSkillState = new State("Skill_E");
        rSkillState = new State("Skill_R");
        dieState = new State("Die");
        for (int i = 0; i < normalAttackEffects.Count; i++) 
            normalAttack.Add(new State("Attack_" + i));
    }
    protected virtual void StateInitalizeOnEnter()
    {
        idleState.onEnter = () => {
            ChangeAnimation(idleState.strStateName);
            canMove = true;
            canAttack = true;
            canDodge = true;
            canTag = true;
            attackCount = -1;
            canDamage = true;
            canSkill = true;
        };
        moveState.onEnter = () => {
            ChangeAnimation(moveState.strStateName);
            canMove = true;
            canAttack = true;
            canDodge = true;
            agent.isStopped = false;
            canTag = true;
            canDamage = true;
            canSkill = true;
        };
        dodgeState.onEnter = () => {
            ChangeAnimation(dodgeState.strStateName);
            canMove = false;
            canAttack = false;
            canDodge = true;
            Action exitEvent = () => ReturnToIdle();
            StartCoroutine(LinearMovement(0.15f, 4f, 0, exitEvent));
            canTag = false;
            canDamage = false;
            canSkill = false;
        };
        qSkillState.onEnter = () => {
            ChangeAnimation(qSkillState.strStateName);
            canMove = false;
            canAttack = false;
            canDodge = true;
            GameManager.instance.AsynchronousExecution(InitializeSkillTime(Skill_Q));
            nextEffect = Skill_Q.effect;
            canTag = false;
            canDamage = true;
            AddRSkillTime(10f);
            canSkill = false;
        };
        wSkillState.onEnter = () => {
            ChangeAnimation(wSkillState.strStateName);
            canMove = false;
            canAttack = false;
            canDodge = true;
            GameManager.instance.AsynchronousExecution(InitializeSkillTime(Skill_W));
            nextEffect = Skill_W.effect;
            canTag = false;
            canDamage = true;
            AddRSkillTime(10f);
            canSkill = false;
        };
        eSkillState.onEnter = () => {
            ChangeAnimation(eSkillState.strStateName);
            canMove = false;
            canAttack = false;
            canDodge = true;
            GameManager.instance.AsynchronousExecution(InitializeSkillTime(Skill_E));
            nextEffect = Skill_E.effect;
            canTag = false;
            canDamage = true;
            AddRSkillTime(10f);
            canSkill = false;
        };
        rSkillState.onEnter = () => {
            ChangeAnimation(rSkillState.strStateName);
            canMove = false;
            canAttack = false;
            canDodge = false;
            GameManager.instance.AsynchronousExecution(InitializeSkillTime(Skill_R));
            canTag = false;
            canDamage = false;
            Skill_R.time.current = 0;
            canSkill = false;
        };
        dieState.onEnter = () => {
            ChangeAnimation(dieState.strStateName);
            canMove = false;
            canAttack = false;
            isDie = true;
            canDodge = false;
            canTag = false;
            canDamage = false;
            canSkill = false;
        };
        for (int i = 0; i < normalAttack.Count; i++)
        {
            int index = i;
            normalAttack[index].onEnter = () => {
                ChangeAnimation(normalAttack[index].strStateName);
                canMove = false;
                canAttack = false;
                canDodge = true;
                nextEffect = normalAttackEffects[index];
                canDamage = true;
                AddRSkillTime(1.3f);
                canSkill = true;
            };
        }
    }
    protected virtual void StateInitalizeOnStay()
    {
        float distanceForDestination;
        moveState.onStay = () => {
            distanceForDestination = Vector3.Distance(transform.position, agent.destination);
			if (distanceForDestination == 0)
                ChangeState(idleState); 
        };
    }
    protected virtual void StateInitalizeOnExit()
    {
        moveState.onExit = () => {
            if(gameObject.activeSelf)
                agent.isStopped = true;
        };
    }
	#endregion Initalize

	#region Input Event
	public void MoveStart(Vector3 targetPos) 
    {
        if (!canMove || isDie) return;
        agent.SetDestination(targetPos);
        GameObject effect = EffectManager.instance.GetEffect(objMouseClickEffect);
        effect.transform.position = agent.destination;
        effect.transform.eulerAngles = Vector3.zero;
        ChangeState(moveState);
    }
    public override void Move(Vector3 targetPos)
    {
        if (!canMove || isDie) return;
        agent.SetDestination(targetPos);
        if (cStateMachine.GetCurrentState() != moveState)
            ChangeState(moveState);
    }
    public override void Attack(Vector3 targetPos)
    {
        if (!canAttack || isDie) return;
        attackCount++;
        if (attackCount >= normalAttack.Count) return;
        Rotate(targetPos);
        ChangeState(normalAttack[attackCount]);
    }
    public bool Dodge(Vector3 targetPos)
    {
        if (!canDodge || isDie) return false;
        Rotate(targetPos);
        ChangeState(dodgeState);
        return true;
    }
    public void OnSkill(SkillType type, Vector3 targetPos)
    {
        if (!canSkill || isDie || !CanUseSkill(type)) return;
		Rotate(targetPos);
        switch (type)
		{
			case SkillType.Q: ChangeState(qSkillState); break;
			case SkillType.W: ChangeState(wSkillState); break;
            case SkillType.E: ChangeState(eSkillState); break;
            case SkillType.R: ChangeState(rSkillState); break;
        }
	}
    #endregion Input Event

    #region Animation Event 
    public void BeCanNextAction()
    {
        if (isDie) return;
        canAttack = true;
    }
    public void ReturnToIdle()
    {
        if (isDie) return;
        attackCount = -1;
        ChangeState(idleState);
    }
    public void EnableAttackEffect()
    {
        GameObject effect = EffectManager.instance.GetEffect(nextEffect);
        effect.GetComponent<Effect>().OnAction(transform, 100f, 1 << 7);
    }
    #endregion Animation Event

    #region In Game Event
    public override void Damage(float amount) 
    {
        if (!canDamage) return;
        Player.instance.CurrentHealthPoint -= amount; 
    }
    public override void Die()
    {
        if (isDie) return;
        ChangeState(dieState);
    }
    protected override void ChangeState(State cNextState)
    {
        if (isDie) return;
        cStateMachine.ChangeState(cNextState);
    }
    protected override void ChangeAnimation(string strTrigger)
    {
        if (cStateMachine.GetPrevState() != null)
            cAnimator.ResetTrigger(cStateMachine.GetPrevState().strStateName);
        cAnimator.SetTrigger(strTrigger);
    }
    protected IEnumerator LinearMovement(float time, float dist, float delayTime = 0, Action exitEvent = null)
    {
        float runTime = 0;
        time -= delayTime;
        Vector3 targetPos = transform.position + transform.forward * dist;
        Vector3 startPos = transform.position;
        yield return new WaitForSeconds(delayTime);

        Ray ray;
        RaycastHit hit;
        Vector3 pos = Vector3.zero;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        while (runTime <= time)
        {
            runTime += Time.deltaTime;

            if (!Physics.SphereCast(transform.position + new Vector3(0, 0.6f, 0), 0.25f, transform.forward, out hit, 1f, 1 << 6))
                transform.position = Vector3.Lerp(startPos, targetPos, runTime / time);

            yield return null;
		}
        exitEvent?.Invoke();
    }
    protected IEnumerator LinearJumpMovement(float time, float height, float delayTime = 0, Action exitEvent = null)
    {
        agent.updatePosition = false;
        float runTime = 0;
        time -= delayTime;
        
        yield return new WaitForSeconds(delayTime);

        while (runTime <= time)
        {
            runTime += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, height * (float)Math.Sin(180 * runTime / time * Mathf.Deg2Rad), transform.position.z);
            yield return null;
        }

        exitEvent?.Invoke();
        agent.nextPosition = transform.position;
        agent.updatePosition = true;
    }
    protected bool CanUseSkill(SkillType type) 
    {
		switch (type)
		{
			case SkillType.Q: return (Skill_Q.time.current >= Skill_Q.time.max) ? true : false; 
			case SkillType.W: return (Skill_W.time.current >= Skill_W.time.max) ? true : false;
            case SkillType.E: return (Skill_E.time.current >= Skill_E.time.max) ? true : false;
            case SkillType.R: return (Skill_R.time.current >= Skill_R.time.max) ? true : false;
        }
        return false;
	}
    protected void Rotate(Vector3 targetPos)
    {
        targetPos.y = transform.position.y;
        Vector3 dir = targetPos - transform.position;
        transform.rotation = Quaternion.LookRotation(dir.normalized);
    }
    protected IEnumerator InitializeSkillTime(Skill usingSkill) 
    {
        usingSkill.time.current = 0;

        if (usingSkill.Equals(Skill_R) == false)
        {
            while (usingSkill.time.current <= usingSkill.time.max)
            {
                usingSkill.time.current += Time.deltaTime;
                yield return null;
            }
            ReadySkill(usingSkill);
        }
    }
    protected void ReadySkill(Skill usingSkill)
    {
        usingSkill.time.current = usingSkill.time.max;
    }
    protected void OnDisable() => ChangeState(idleState);
    protected void AddRSkillTime(float addTime) 
    {
        addTime = Random.Range(addTime * 0.8f, addTime * 1.2f);
        Skill_R.time.current += addTime;
    }
    #endregion In Game Event

    #region Public Event
    public string GetCurrentStateName() => cStateMachine.GetCurrentState().strStateName;
    public void UseSkillWithoutPressKey(SkillType skillType, Vector3 target)
    {
        transform.LookAt(target);
        switch (skillType)
        {
            case SkillType.Q: ChangeState(qSkillState); break;
            case SkillType.W: ChangeState(wSkillState); break;
            case SkillType.E: ChangeState(eSkillState); break;
            case SkillType.R: ChangeState(rSkillState); break;
        }
    }
    public SkillTimeInfo GetSkillTimer(SkillType skill)
    {
        switch (skill)
        {
            case SkillType.Q: return new SkillTimeInfo(Skill_Q.time.max, Skill_Q.time.current);
            case SkillType.W: return new SkillTimeInfo(Skill_W.time.max, Skill_W.time.current);
            case SkillType.E: return new SkillTimeInfo(Skill_E.time.max, Skill_E.time.current);
            case SkillType.R: return new SkillTimeInfo(Skill_R.time.max, Skill_R.time.current);
        }
        return new SkillTimeInfo(0, 0);
    }
    public bool GetCanTag() => canTag;
    public virtual void ResetSkillTime() => print("override ResetSkillTime()"); 
    #endregion
}