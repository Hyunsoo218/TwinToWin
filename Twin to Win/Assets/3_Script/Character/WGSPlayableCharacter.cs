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
            if (fESkillTimer <= 0f && EnableESkill() == true)
            {
                cStateMachine.ChangeState(cESkill);
            }
        };
        eSkillAction.performed += ctx =>
        {
            if (cStateMachine.GetCurrentState() == cESkill && isDoingHoldESkill == false)
            {
                isDoingHoldESkill = true;
                StartCoroutine(StartESkillHoldTimer());
            }
        };
        eSkillAction.canceled += ctx =>
        {
            if (cStateMachine.GetCurrentState() == cESkill)
            {
                StartCoroutine(StartESkillCoolDown());
                StopESkill();
            }
        };
    }
    #endregion

    #region QSkill
    private float fQSkillTimer = 0f;
    #endregion

    #region WSkill
    private float fWSkillTimer = 0f;
    
    #endregion

    #region ESKill
    private float fESkillTimer = 0f;
    private float fESkillHoldTime = 3f;
    private float fESkillHoldTimer = 0f;
    private bool isDoingHoldESkill = false;

    InputAction eSkillAction;
    #endregion

    #region Normal Attack Part

    public override void Attack()
    {
        isNotNormalAttackState = cStateMachine.GetCurrentState() != cNormalAttack[0] && 
            cStateMachine.GetCurrentState() != cNormalAttack[1] && 
            cStateMachine.GetCurrentState() != cNormalAttack[2] && 
            cStateMachine.GetCurrentState() != cNormalAttack[3] && 
            cStateMachine.GetCurrentState() != cNormalAttack[4];
        IncreaseAttackCount();
        ResetAttackCount();
        CheckExceededCancelTime();
    }

    #endregion

    private bool EnableQSkill()
    {
        return cStateMachine.GetCurrentState() != cQSkill 
            && cStateMachine.GetCurrentState() != cESkill 
            && cStateMachine.GetCurrentState() != cDodgeState 
            && cStateMachine.GetCurrentState() != cToStand;
    }
    private bool EnableWSkill()
    {
        return cStateMachine.GetCurrentState() != cWSkill 
            && cStateMachine.GetCurrentState() != cESkill 
            && cStateMachine.GetCurrentState() != cDodgeState 
            && cStateMachine.GetCurrentState() != cToStand;
    }
    private bool EnableESkill()
    {
        return cStateMachine.GetCurrentState() != cESkill 
            && cStateMachine.GetCurrentState() != cDodgeState
            && cStateMachine.GetCurrentState() != cToStand;
    }

    #region QSkill Part
    public void OnQSkill(InputAction.CallbackContext context)
    {
        if (context.started && fQSkillTimer <= 0f && EnableQSkill() == true)
        {
            cStateMachine.ChangeState(cQSkill);
            StartCoroutine(StartQSkillCoolDown());
        }
    }

    private IEnumerator StartQSkillCoolDown()
    {
        while (fQSkillTimer < srtQSkill.fSkillCoolDown)
        {
            fQSkillTimer += Time.deltaTime;
            yield return null;
        }
        fQSkillTimer = 0f;
    }
    #endregion

    #region WSkill Part
    public void OnWSkill(InputAction.CallbackContext context)
    {
        if (context.started && fWSkillTimer <= 0f && EnableWSkill() == true)
        {
            cStateMachine.ChangeState(cWSkill);
            StartCoroutine(StartWSkillCoolDown());
        }
    }

    private IEnumerator StartWSkillCoolDown()
    {
        while (fWSkillTimer < srtWSkill.fSkillCoolDown)
        {
            fWSkillTimer += Time.deltaTime;
            yield return null;
        }
        fWSkillTimer = 0f;
    }


    #endregion

    #region ESkill Part

    private IEnumerator StartESkillHoldTimer()
    {
        while (fESkillHoldTime > fESkillHoldTimer && isDoingHoldESkill == true)
        {
            Vector3 mousePosOnVirtualGround = GetPositionOnVirtualGround();
            transform.localRotation = GetMouseAngle();
            transform.position = Vector3.MoveTowards(transform.position, mousePosOnVirtualGround, Time.deltaTime * 3f);
            fESkillHoldTimer += Time.deltaTime;
            yield return null;
        }
        StopESkill();
    }

    private void StopESkill()
    {
        isDoingHoldESkill = false;
        cStateMachine.ChangeState(cToStand);
        fESkillHoldTimer = 0f;
    }

    private IEnumerator StartESkillCoolDown()
    {
        while (fESkillTimer < srtESkill.fSkillCoolDown)
        {
            fESkillTimer += Time.deltaTime;
            yield return null;
        }
        fESkillTimer = 0f;
    }
    #endregion

}
