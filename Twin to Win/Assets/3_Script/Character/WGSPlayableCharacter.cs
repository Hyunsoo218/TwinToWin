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
        InitalizeESkillButton();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Attack();
    }


    #region init
    private new void Initialize()
    {
        eSkillAction = GetComponent<PlayerInput>().actions["WGSESkill"];
    }
    protected override void StateInitalizeOnEnter()
    {
        base.StateInitalizeOnEnter();
        cNormalAttack[0].onEnter += () => { ChangeAnimation(cNormalAttack[0].strStateName); };
        cNormalAttack[1].onEnter += () => { ChangeAnimation(cNormalAttack[1].strStateName); };
        cNormalAttack[2].onEnter += () => { ChangeAnimation(cNormalAttack[2].strStateName); };
        cNormalAttack[3].onEnter += () => { ChangeAnimation(cNormalAttack[3].strStateName); };
        cNormalAttack[4].onEnter += () => { ChangeAnimation(cNormalAttack[4].strStateName); };
    }



    private void InitalizeESkillButton()
    {
        eSkillAction.started += ctx =>
        {
            UIManager.instance.OnSkillBtn(KeyCode.E);
            if (EnableESkill() == true && (fESkillTimer >= srtESkill.fSkillCoolDown || fESkillTimer == 0f) && Player.instance.cCurrentCharacter == Player.instance.GetGreatSword())
            {
                FeverGauge.Instance.IncreaseSkillFeverGauge();
                srtCurrentSkill = srtESkill;
                cStateMachine.ChangeState(cESkillState);
            }
        };
        eSkillAction.performed += ctx =>
        {
            if (cStateMachine.GetCurrentState() == cESkillState && isDoingHoldESkill == false && Player.instance.cCurrentCharacter == Player.instance.GetGreatSword())
            {
                isDoingHoldESkill = true;
                GameManager.instance.AsynchronousExecution(StartESkillHoldTimer());
            }
        };
        eSkillAction.canceled += ctx =>
        {
            if (cStateMachine.GetCurrentState() == cESkillState && Player.instance.cCurrentCharacter == Player.instance.GetGreatSword())
            {
                StopESkill();
            }
        };
    }
    #endregion

    #region E Skill
    private float fESkillHoldTime = 3f;
    private bool isDoingHoldESkill = false;

    InputAction eSkillAction;
    #endregion

    #region Normal Attack Part

    public override void Attack()
    {
        isNormalAttackState = cStateMachine.GetCurrentState() == cNormalAttack[0] ||
            cStateMachine.GetCurrentState() == cNormalAttack[1] ||
            cStateMachine.GetCurrentState() == cNormalAttack[2] ||
            cStateMachine.GetCurrentState() == cNormalAttack[3] ||
            cStateMachine.GetCurrentState() == cNormalAttack[4];
        IncreaseAttackCount();
        ResetAttackCount();
        CheckExceededCancelTime();
    }

    #endregion

    private bool EnableQSkill()
    {
        return cStateMachine.GetCurrentState() != cQSkillState
            && cStateMachine.GetCurrentState() != cWSkillState
            && cStateMachine.GetCurrentState() != cESkillState
            && cStateMachine.GetCurrentState() != cDodgeState
            && cStateMachine.GetCurrentState() != cToStandState;
    }
    private bool EnableWSkill()
    {
        return cStateMachine.GetCurrentState() != cQSkillState
            && cStateMachine.GetCurrentState() != cWSkillState
            && cStateMachine.GetCurrentState() != cESkillState
            && cStateMachine.GetCurrentState() != cDodgeState
            && cStateMachine.GetCurrentState() != cToStandState;
    }
    private bool EnableESkill()
    {
        return cStateMachine.GetCurrentState() != cQSkillState
            && cStateMachine.GetCurrentState() != cWSkillState
            && cStateMachine.GetCurrentState() != cESkillState
            && cStateMachine.GetCurrentState() != cDodgeState
            && cStateMachine.GetCurrentState() != cToStandState;
    }

    #region QSkill Part
    public void OnQSkill(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            UIManager.instance.OnSkillBtn(KeyCode.Q);
        }
        
        if (context.started && EnableQSkill() == true && (fQSkillTimer >= srtQSkill.fSkillCoolDown || fQSkillTimer == 0f))
        {
            FeverGauge.Instance.IncreaseSkillFeverGauge();
            srtCurrentSkill = srtQSkill;
            transform.localRotation = GetMouseAngle();
            cStateMachine.ChangeState(cQSkillState);
        }
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
            fQSkillTimer += Time.deltaTime * Constants.fSpeedConstant;
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
        
        if (context.started && EnableWSkill() == true && (fWSkillTimer >= srtWSkill.fSkillCoolDown || fWSkillTimer == 0f))
        {
            FeverGauge.Instance.IncreaseSkillFeverGauge();
            srtCurrentSkill = srtWSkill;
            cStateMachine.ChangeState(cWSkillState);
        }
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
            fWSkillTimer += Time.deltaTime * Constants.fSpeedConstant;
            yield return null;
        }
        fWSkillTimer = srtWSkill.fSkillCoolDown;
    }


    #endregion

    #region ESkill Part

    public void StartWGSESkillCoolDownCoroutine()
    {
        GameManager.instance.AsynchronousExecution(StartESkillCoolDown());
    }

    private IEnumerator StartESkillHoldTimer()
    {
        while (fESkillHoldTime > fESkillHoldTimer && isDoingHoldESkill == true)
        {
            Vector3 mousePosOnVirtualGround = GetPositionOnVirtualGround();
            transform.position = Vector3.MoveTowards(transform.position, mousePosOnVirtualGround, Time.deltaTime * Constants.fSpeedConstant * 3f);
            fESkillHoldTimer += Time.deltaTime * Constants.fSpeedConstant;
            yield return null;
        }
        StopESkill();
    }

    private void StopESkill()
    {
        isDoingHoldESkill = false;
        cStateMachine.ChangeState(cToStandState);
        transform.localRotation = GetMouseAngle();
        fESkillHoldTimer = 0f;
    }

    private IEnumerator StartESkillCoolDown()
    {
        fESkillTimer = 0f;
        while (fESkillTimer < srtESkill.fSkillCoolDown)
        {
            fESkillTimer += Time.deltaTime * Constants.fSpeedConstant;
            yield return null;
        }
        fESkillTimer = srtESkill.fSkillCoolDown;
    }
    #endregion

    #region Fever Part
    public override void OnFever(InputAction.CallbackContext ctx)
    {
        base.OnFever(ctx);
        //if (FeverGauge.Instance.IsDoubleFeverGaugeFull() == false && FeverGauge.Instance.IsFeverGaugeFull() == true && IsFeverTime() == false)
        //{
        //    CutCoolDown(fCoolDownCutAndRestoreTime);
        //    SetIsFeverTime(true);
        //    StartCoroutine(FeverGauge.Instance.StartRedFeverTime());
        //}

        if (FeverGauge.Instance.IsFeverGaugeFull() == true && IsFeverTime() == false)
        {
            CutCoolDown(fCoolDownCutAndRestoreTime);
            SetIsFeverTime(true);
            StartCoroutine(FeverGauge.Instance.StartRedFeverTime());
        }
    }
    #endregion

}