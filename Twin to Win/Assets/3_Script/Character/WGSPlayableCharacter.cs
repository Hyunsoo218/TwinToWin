using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;

public class WGSPlayableCharacter : PlayerbleCharacter
{
    private State[] cNormalAttack = new State[5] { new State("normalAttack1"), new State("normalAttack2"), new State("normalAttack3"), new State("normalAttack4"), new State("normalAttack5") };
    protected override void Awake()
    {
        base.Awake();
        InitalizeLeftMouseState();
        InitalizeESkillButton();
        Initialize();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Attack();
    }

    #region init
    private new void Initialize()
    {
        eSkillAction = GetComponent<PlayerInput>().actions["ESkill"];
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

    private void InitalizeLeftMouseState()
    {
        normalAttackAction.started += ctx =>
        {
            if (fNormalAttackCancelTimer > 0f)
            {
                fNormalAttackCancelTimer = 0f;
                canResetDuringCancelTime = true;
            }

            if (eMouseState != mouseState.Hold &&
                cStateMachine.GetCurrentState() != cNormalAttack[nNormalAttackCount] &&
                cStateMachine.GetCurrentState() != cDodgeState &&
                cStateMachine.GetCurrentState() != cQSkill &&
                canNextAttack)
            {
                cStateMachine.ChangeState(cNormalAttack[nNormalAttackCount]);
            }
            else if (eMouseState == mouseState.Hold)
            {
                StartCoroutine(AttackDuringHoldMove());
            }
        };
    }

    private void InitalizeESkillButton()
    {
        eSkillAction.started += ctx =>
        {
            if (fESkillTimer <= 0f && cStateMachine.GetCurrentState() != cESkill && cStateMachine.GetCurrentState() != cDodgeState)
            {
                cStateMachine.ChangeState(cESkill);
                StartCoroutine(StartESkillCoolDown());
            }
        };
        eSkillAction.performed += ctx =>
        {
            if (cStateMachine.GetCurrentState() == cESkill)
            {
                isDoingESkill = true;
            }
            
        };
        eSkillAction.canceled += ctx =>
        {
            isDoingESkill = false;
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
    private float fESkillHoldTime = 5f;
    private bool isDoingESkill = false;

    InputAction eSkillAction;
    #endregion

    #region Normal Attack Part
    private IEnumerator AttackDuringHoldMove()
    {
        eMouseState = mouseState.None;
        cStateMachine.ChangeState(cNormalAttack[nNormalAttackCount]);
        yield return new WaitUntil(() => fNormalAttackCancelTimer > 0.2f || cStateMachine.GetCurrentState() == cIdleState);
        KeepHoldMove();
    }

    private void EnableAttackEffect()
    {
        GameObject obj = EffectManager.instance.GetEffect(objAttackEffect);
        obj.GetComponent<Effect>().OnAction(transform, fPower, 1 << 7);
    }

    private void EnableRotationAttackEffect()
    {
        GameObject obj = EffectManager.instance.GetEffect(objRotationAttackEffect);
        obj.GetComponent<Effect>().OnAction(transform, fPower, 1 << 7);
    }

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

    private void IncreaseAttackCount()
    {
        if (cStateMachine.GetCurrentState() == cNormalAttack[nNormalAttackCount] && fNormalAttackCancelTimer < fNormalAttackCancelTime)
        {
            nNormalAttackCount = nNormalAttackCount < cNormalAttack.Length - 1 ? ++nNormalAttackCount : 0;
        }
    }

    private void ResetAttackCount()
    {
        if (isNotNormalAttackState)
        {
            canNextAttack = true;
            nNormalAttackCount = 0;
        }
    }

    private void CheckExceededCancelTime()
    {
        if (fNormalAttackCancelTimer >= fNormalAttackCancelTime - 0.05f)
        {
            ReturnToIdle();
        }
    }

    private void DisableNextAttack()
    {
        canNextAttack = false;
    }

    private void EnableNextAttack()
    {
        normalAttackCancelTimer = StartCoroutine(StartNormalAttackCancelTimer());
        canNextAttack = true;
    }



    private IEnumerator StartNormalAttackCancelTimer()
    {
        while (fNormalAttackCancelTimer < fNormalAttackCancelTime)
        {
            if (eMouseState == mouseState.Hold)
            {
                fNormalAttackCancelTimer = 0f;
                yield break;
            }
            if (canResetDuringCancelTime == true)
            {
                fNormalAttackCancelTimer = 0f;
                canResetDuringCancelTime = false;
                yield break;
            }
            fNormalAttackCancelTimer += Time.deltaTime;
            yield return null;
        }

        fNormalAttackCancelTimer = 0f;
        canResetDuringCancelTime = false;
    }
    #endregion

    #region QSkill Part
    public void OnQSkill(InputAction.CallbackContext context)
    {
        if (context.started && fQSkillTimer <= 0f && cStateMachine.GetCurrentState() != cQSkill)
        {
            eMouseState = mouseState.None;
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
        if (context.started && fWSkillTimer <= 0f && cStateMachine.GetCurrentState() != cWSkill)
        {
            eMouseState = mouseState.None;
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
