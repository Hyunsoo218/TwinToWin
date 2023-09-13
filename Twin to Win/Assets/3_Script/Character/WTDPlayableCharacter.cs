using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;

public class WTDPlayableCharacter : PlayerbleCharacter
{
    protected override void Awake()
    {
        base.Awake();
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

    public override void Attack()
    {
        isNotNormalAttackState = cStateMachine.GetCurrentState() != cNormalAttack[0] 
            && cStateMachine.GetCurrentState() != cNormalAttack[1] 
            && cStateMachine.GetCurrentState() != cNormalAttack[2];
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
    public void OnESkill(InputAction.CallbackContext context)
    {

        if (context.started && fESkillTimer <= 0f && EnableESkill() == true)
        {
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
