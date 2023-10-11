using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;

public class WGSPlayableCharacter : PlayerbleCharacter
{
    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Attack();
    }
    #region init
    protected override void StateInitalizeOnEnter()
    {
        base.StateInitalizeOnEnter();
        cNormalAttack[0].onEnter += () => { ChangeAnimation(cNormalAttack[0].strStateName); isNormalAttackState = true; };
        cNormalAttack[1].onEnter += () => { ChangeAnimation(cNormalAttack[1].strStateName); isNormalAttackState = true; };
        cNormalAttack[2].onEnter += () => { ChangeAnimation(cNormalAttack[2].strStateName); isNormalAttackState = true; };
        cNormalAttack[3].onEnter += () => { ChangeAnimation(cNormalAttack[3].strStateName); isNormalAttackState = true; };
        cNormalAttack[4].onEnter += () => { ChangeAnimation(cNormalAttack[4].strStateName); isNormalAttackState = true; };
    }
    #endregion

    #region E Skill Var
    private float fESkillHoldTime = 3f;
    #endregion

    #region Skill Var
    #endregion

    #region Normal Attack Part

    public override void Attack()
    {
        IncreaseAttackCount();
        ResetAttackCount();
        CheckExceededCancelTime();
    }

    protected override void IncreaseAttackCount()
    {
        if (cStateMachine.GetCurrentState() == cNormalAttack[nNormalAttackCount] && canNextAttack == true)
        {
            nNormalAttackCount = nNormalAttackCount < 4 ? ++nNormalAttackCount : 0;
        }
    }

    #endregion



    private bool EnableSkill()
    {
        return cStateMachine.GetCurrentState() != cQSkillState
            && cStateMachine.GetCurrentState() != cWSkillState
            && cStateMachine.GetCurrentState() != cESkillState
            && cStateMachine.GetCurrentState() != cRSkillState
            && cStateMachine.GetCurrentState() != cDodgeState
            && cStateMachine.GetCurrentState() != cToStandState;
    }

    public void EnableSkillEffect(float damage)
    {
        GameObject obj = EffectManager.instance.GetEffect(srtCurrentSkill.objSkillEffect);
        float finalDamage = ChangeDamageToRandom(damage);
        obj.GetComponent<Effect>().OnAction(transform, finalDamage, 1 << 7);
	}

    public override void Damage(float fAmount)
    {
        if (Invincible() == true)
        {
            return;
        }

        ReduceHP(fAmount);
        UIManager.instance.SetPlayerHealthPoint();
        if (Player.instance.GetPlayerHp() <= 0)
        {
            Die();
        }
    }

    private bool Invincible()
    {
        if (cStateMachine.GetCurrentState() == cDeadState || cStateMachine.GetCurrentState() == cRSkillState)
        {
            return true;
        }
        return false;
    }

    protected override void ReduceHP(float fAmount)
    {
        float currentHp = Player.instance.GetPlayerHp();

        if (cStateMachine.GetCurrentState() == cQSkillState
            || cStateMachine.GetCurrentState() == cWSkillState
            || cStateMachine.GetCurrentState() == cESkillState)
        {
            Player.instance.SetPlayerHp(currentHp - (fAmount * 0.5f));
        }
        else
        {
            Player.instance.SetPlayerHp(currentHp - fAmount);
        }
    }

    #region QSkill Part
    public void OnQSkill(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            UIManager.instance.OnSkillBtn(KeyCode.Q);
        }
        
        if (context.started && EnableSkill() == true && (fQSkillTimer >= srtQSkill.fSkillCoolDown || fQSkillTimer == 0f))
        {
            RSkillGauge.Instance.IncreaseRSkillGaugeUsingSkill();
            srtCurrentSkill = srtQSkill;
            cStateMachine.ChangeState(cQSkillState);
        }
    }

    private void UseQSkillWithoutKey(Vector3 target)
    {
        srtCurrentSkill = srtQSkill;
        cStateMachine.ChangeState(cQSkillState);
        transform.LookAt(target);
    }

    public void StartWGSQSkillCoolDownCoroutine()
    {
        GameManager.instance.AsynchronousExecution(StartQSkillCoolDown());
    }

    private IEnumerator StartQSkillCoolDown()
    {
        fQSkillTimer = 0f;
        while (fQSkillTimer < srtQSkill.fSkillCoolDown)
        {
            fQSkillTimer += Time.deltaTime;
            yield return null;
        }
        fQSkillTimer = srtQSkill.fSkillCoolDown;
    }
    #endregion

    #region WSkill Part
    public void OnWSkill(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            UIManager.instance.OnSkillBtn(KeyCode.W);
        }
        
        if (context.started && EnableSkill() == true && (fWSkillTimer >= srtWSkill.fSkillCoolDown || fWSkillTimer == 0f))
        {
            RSkillGauge.Instance.IncreaseRSkillGaugeUsingSkill();
            srtCurrentSkill = srtWSkill;
            cStateMachine.ChangeState(cWSkillState);
        }
    }

    private void UseWSkillWithoutKey(Vector3 target)
    {
        srtCurrentSkill = srtWSkill;
        cStateMachine.ChangeState(cWSkillState);
        transform.LookAt(target);
    }

    public void StartWGSWSkillCoolDownCoroutine()
    {
        GameManager.instance.AsynchronousExecution(StartWSkillCoolDown());
    }

    private IEnumerator StartWSkillCoolDown()
    {
        fWSkillTimer = 0f;
        while (fWSkillTimer < srtWSkill.fSkillCoolDown)
        {
            fWSkillTimer += Time.deltaTime;
            yield return null;
        }
        fWSkillTimer = srtWSkill.fSkillCoolDown;
    }


    #endregion

    #region ESkill Part
    public void OnESkill(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            UIManager.instance.OnSkillBtn(KeyCode.E);
        }

        if (EnableSkill() == true && context.started && (fESkillTimer >= srtESkill.fSkillCoolDown || fESkillTimer == 0f) && Player.instance.cCurrentCharacter == Player.instance.GetGreatSword())
        {
            RSkillGauge.Instance.IncreaseRSkillGaugeUsingSkill();
            srtCurrentSkill = srtESkill;
            cStateMachine.ChangeState(cESkillState);
        }
    }

    private void UseESkillWithoutKey(Vector3 target)
    {
        srtCurrentSkill = srtESkill;
        cStateMachine.ChangeState(cESkillState);
        transform.LookAt(target);
    }

    public void StartWGSESkillCoolDownCoroutine()
    {
        GameManager.instance.AsynchronousExecution(StartESkillHoldTimer());
        GameManager.instance.AsynchronousExecution(StartESkillCoolDown());
    }

    public IEnumerator StartESkillHoldTimer()
    {
        while (fESkillHoldTime > fESkillHoldTimer 
            && cStateMachine.GetCurrentState() != cQSkillState
            && cStateMachine.GetCurrentState() != cWSkillState
            && cStateMachine.GetCurrentState() != cRSkillState
            && cStateMachine.GetCurrentState() != cDodgeState
            && cStateMachine.GetCurrentState() != cToStandState
            && cStateMachine.GetCurrentState() != cIdleState
            && cStateMachine.GetCurrentState() != cMoveState
            && isNormalAttackState == false)
        {
            Vector3 mousePosOnVirtualGround = GetPositionOnVirtualGround();
            transform.position = Vector3.MoveTowards(transform.position, mousePosOnVirtualGround, Time.deltaTime * 3f);
            fESkillHoldTimer += Time.deltaTime;
            yield return null;
        }
        StopESkill();
    }

    private void StopESkill()
    {
        //ReturnToIdleWithHold();
        cStateMachine.ChangeState(cToStandState);
        transform.localRotation = GetMouseAngle();
        fESkillHoldTimer = 0f;
    }

    private IEnumerator StartESkillCoolDown()
    {
        fESkillTimer = 0f;
        while (fESkillTimer < srtESkill.fSkillCoolDown)
        {
            fESkillTimer += Time.deltaTime;
            yield return null;
        }
        fESkillTimer = srtESkill.fSkillCoolDown;
    }
    #endregion

    #region RSkill Part

    public void OnRSkill(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            UIManager.instance.OnSkillBtn(KeyCode.R);
        }

        if (ctx.started && EnableSkill() == true && RSkillGauge.Instance.IsRSkillGaugeFull() == true)
        {
            RSkillGauge.Instance.fBlueGauge = 0f;
            srtCurrentSkill = srtRSkill;
            cStateMachine.ChangeState(cRSkillState);
        }
    }

    private void UseRSkillWithoutKey(Vector3 target)
    {
        srtCurrentSkill = srtRSkill;
        cStateMachine.ChangeState(cRSkillState);
        transform.LookAt(target);
    }

    public override float GetCoolDownCutAndRestoreTime()
    {
        return 0f;
    }
    #endregion

    protected override void DoSkillWithoutPressKey(SkillType skillType, Vector3 target)
    {
        switch (skillType)
        {
            case SkillType.QSkill:
                UseQSkillWithoutKey(target);
                break;
            case SkillType.WSkill:
                UseWSkillWithoutKey(target);
                break;
            case SkillType.ESkill:
                UseESkillWithoutKey(target);
                break;
            case SkillType.RSkill:
                UseRSkillWithoutKey(target);
                break;
            default:
                Debug.Log("SkillType is Null!");
                break;
        }
    }

}