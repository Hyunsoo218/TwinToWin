using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;

public class WTDPlayableCharacter : PlayerbleCharacter
{
    private State[] cNormalAttack = new State[3] { new State("normalAttack1"), new State("normalAttack2"), new State("normalAttack3") };
    protected override void Awake()
    {
        base.Awake();
        InitalizeLeftMouseState();
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
        cNormalAttack[0].onEnter += () => { ChangeAnimation(cNormalAttack[0].strStateName); };
        cNormalAttack[1].onEnter += () => { ChangeAnimation(cNormalAttack[1].strStateName); };
        cNormalAttack[2].onEnter += () => { ChangeAnimation(cNormalAttack[2].strStateName); };
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
    #endregion

    #region QSkill
    private float fQSkillTimer = 0f;
    #endregion

    #region WSkill
    private float fWSkillTimer = 0f;
    
    #endregion

    #region ESKill
    private float fESkillTimer = 0f;
    private float fParabolaTimer = 0f;
    private float fFreeFallTimer = 0f;
    private float parabolaHeight = 0f;
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

    public override void Attack()
    {
        isNotNormalAttackState = cStateMachine.GetCurrentState() != cNormalAttack[0] && cStateMachine.GetCurrentState() != cNormalAttack[1] && cStateMachine.GetCurrentState() != cNormalAttack[2];
        IncreaseAttackCount();
        ResetAttackCount();
        CheckExceededCancelTime();
    }

    private void IncreaseAttackCount()
    {
        if (cStateMachine.GetCurrentState() == cNormalAttack[nNormalAttackCount] && fNormalAttackCancelTimer < fNormalAttackCancelTime)
        {
            EnableAttackEffect();
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
            ReturnToIdleWithHold();
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

    private bool EnableQSkill()
    {
        return cStateMachine.GetCurrentState() != cQSkill;
    }
    private bool EnableWSkill()
    {
        return cStateMachine.GetCurrentState() != cWSkill;
    }
    private bool EnableESkill()
    {
        return cStateMachine.GetCurrentState() != cESkill && cStateMachine.GetCurrentState() != cDodgeState;
    }

    #region QSkill Part
    public void OnQSkill(InputAction.CallbackContext context)
    {
        if (context.started && fQSkillTimer <= 0f && EnableQSkill() == true)
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
        if (context.started && fWSkillTimer <= 0f && EnableWSkill() == true)
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
    public void OnESkill(InputAction.CallbackContext context)
    {

        if (context.started && fESkillTimer <= 0f && EnableESkill() == true)
        {
            eMouseState = mouseState.None;
            cStateMachine.ChangeState(cESkill);
            StartCoroutine(StartJumpAndRotate());
            StartCoroutine(StartESkillCoolDown());
        }
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

    private void OnJumpAndRotate(AnimationEvent skillEvent)
    {
        srtCurrentSkill = dicCurrentSkill[(skillEvent.stringParameter)];
    }

    private IEnumerator StartJumpAndRotate()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + transform.forward * 3f;
        float parabolaHighestHeight = 3f;
        float parabolaSpeed = 2f;

        fParabolaTimer = 0f;
        fFreeFallTimer = 0f;
        yield return new WaitUntil(() => DoJumpAndRotate(startPos, endPos, parabolaHighestHeight, parabolaSpeed) == true);
        cStateMachine.ChangeState(cToStand);
    }

    private bool DoJumpAndRotate(Vector3 startPos, Vector3 endPos, float parabolaHighestHeight, float parabolaSpeed)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + new Vector3(0f, 0.5f), 0.5f, ~(1 << LayerMask.NameToLayer("Player")));
        Vector3 parabolaPos = Parabola(startPos, endPos, parabolaHighestHeight, fParabolaTimer / 1f);
        fParabolaTimer += Time.deltaTime * parabolaSpeed;
        fParabolaTimer = fParabolaTimer % 1f;

        if (colliders.Length > 0)
        {
            fFreeFallTimer += Time.deltaTime;
            endPos = new Vector3(transform.position.x, 0f, transform.position.z);
            transform.position = FreeFall(parabolaPos.y, fFreeFallTimer);
        }
        else
        {
            transform.position = parabolaPos;
        }
        return Physics.Raycast(transform.position, Vector3.down, 1f, 1 << LayerMask.NameToLayer("Ground")) && fParabolaTimer >= 0.5f;
    }


    #endregion

    private Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        parabolaHeight = -4 * height * t * t + 4 * height * t;

        Vector3 mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, parabolaHeight + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    private Vector3 FreeFall(float height, float t)
    {
        float freeFallY = height - 1 / 2 * 9.82f * (t * t);

        return new Vector3(transform.position.x, freeFallY, transform.position.z);
    }
}
