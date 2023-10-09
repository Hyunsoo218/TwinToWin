using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;
using UnityEngine.AI;

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
        cNormalAttack[0].onEnter += () => { ChangeAnimation(cNormalAttack[0].strStateName); isNormalAttackState = true; };
        cNormalAttack[1].onEnter += () => { ChangeAnimation(cNormalAttack[1].strStateName); isNormalAttackState = true; };
        cNormalAttack[2].onEnter += () => { ChangeAnimation(cNormalAttack[2].strStateName); isNormalAttackState = true; }; 
    }

    protected override void StateInitalizeOnExit()
    {
        base.StateInitalizeOnExit();
        cQSkillState.onExit += () => { canResetCoolDown = false; };
        cWSkillState.onExit += () => { canResetCoolDown = false; };
    }

    #endregion

    #region ESKill Var
    private float parabolaHeight = 0f;
    #endregion

    #region Skill Var
    private bool canResetCoolDown = false;
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
            nNormalAttackCount = nNormalAttackCount < 2 ? ++nNormalAttackCount : 0;
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

    private void EnableSkillEffect()
    {
        GameObject obj = EffectManager.instance.GetEffect(srtCurrentSkill.objSkillEffect);
        obj.GetComponent<Effect>().OnAction(transform, fPower, 1 << 7);
        ResetCoolDownWhenMonsterDie(srtCurrentSkill, obj?.GetComponent<PlayerEffect>().GetMonsterInOverlap());
    }

    private void ResetCoolDownWhenMonsterDie(Skill skillType, Collider monster)
    {
        if (monster == null || monster.GetComponent<MonsterCharacter>().GetIsEnterMonsterDead() == false)
        {
            return;
        }
        else if (monster.GetComponent<MonsterCharacter>().GetIsEnterMonsterDead() == true)
        {
            if (skillType.Equals(srtQSkill) || skillType.Equals(srtWSkill))
            {
                canResetCoolDown = true;
            }
        }
        monster.GetComponent<MonsterCharacter>().SetIsEnterMonsterDead(false);

    }

    public override void Damage(float fAmount)
    {
        if (Invincible() == true)
        {
            return;
        }

        ReduceHP(fAmount);
        if (fHealthPoint <= 0)
        {
            Die();
        }
    }

    private bool Invincible()
    {
        if (cStateMachine.GetCurrentState() == cDeadState || cStateMachine.GetCurrentState() == cESkillState)
        {
            return true;
        }
        return false;
    }

    protected override void ReduceHP(float fAmount) 
    {
        fHealthPoint -= fAmount;
    }


    #region QSkill Part
    public void OnQSkill(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            UIManager.instance.OnSkillBtn(KeyCode.Q);
        }

        if (EnableSkill() == true && context.started && (fQSkillTimer >= srtQSkill.fSkillCoolDown || fQSkillTimer == 0f))
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

    public void StartWTDQSkillCoolDownCoroutine()
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
        fWSkillTimer = srtWSkill.fSkillCoolDown;
    }


    #endregion

    #region WSkill Part
    public void OnWSkill(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            UIManager.instance.OnSkillBtn(KeyCode.W);
        }
        
        if (EnableSkill() == true && context.started && (fWSkillTimer >= srtWSkill.fSkillCoolDown || fWSkillTimer == 0f))
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

    public void StartWTDWSkillCoolDownCoroutine()
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
        
        if (EnableSkill() == true && context.started && (fESkillTimer >= srtESkill.fSkillCoolDown || fESkillTimer == 0f))
        {
            RSkillGauge.Instance.IncreaseRSkillGaugeUsingSkill();
            srtCurrentSkill = srtESkill;
            cStateMachine.ChangeState(cESkillState);
            GameManager.instance.AsynchronousExecution(StartJumpAndRotate());
        }
    }
    private void UseESkillWithoutKey(Vector3 target)
    {
        srtCurrentSkill = srtESkill;
        cStateMachine.ChangeState(cESkillState);
        transform.LookAt(target);
        GameManager.instance.AsynchronousExecution(StartJumpAndRotate());
    }

    public void StartWTDESkillCoolDownCoroutine()
    {
        GameManager.instance.AsynchronousExecution(StartESkillCoolDown());
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


    private IEnumerator StartJumpAndRotate()
    {
        float parabolaHighestHeight = 3f;
        float parabolaSpeed = 2f;
        float parabolaPower = 10f;
        bool isHitWall = false;
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + transform.forward * parabolaPower;

        
        fParabolaTimer = 0f;
        fFreeFallTimer = 0f;
        yield return new WaitUntil(() => DoJumpAndRotate(startPos, endPos, parabolaHighestHeight, parabolaSpeed, ref isHitWall) == true);
        StartWTDESkillCoolDownCoroutine();
        cStateMachine.ChangeState(cToStandState);
    }

    private bool DoJumpAndRotate(Vector3 startPos, Vector3 _endPos, float parabolaHighestHeight, float parabolaSpeed, ref bool isHitWall)
    {
        RaycastHit hit;
        Vector3 endPos = _endPos;

        fParabolaTimer += Time.deltaTime * parabolaSpeed;
        fParabolaTimer = fParabolaTimer % 1f;

        Collider[] colliders = Physics.OverlapSphere(transform.position + new Vector3(0f, 0.5f), 0.5f, ~(1 << LayerMask.NameToLayer("Player")));
        if (colliders.Length > 0)   // 벽치기 임시
        {
            isHitWall = true;
            fFreeFallTimer += Time.deltaTime;
            endPos = new Vector3(transform.position.x, 0f, transform.position.z);
            transform.position = FreeFall(Parabola(startPos, endPos, parabolaHighestHeight, fParabolaTimer / 1f).y, fFreeFallTimer);
        }

        if (Physics.Raycast(Parabola(startPos, endPos, parabolaHighestHeight, fParabolaTimer / 1f), Vector3.down, out hit, 100f, 1 << LayerMask.NameToLayer("Ground")) && isHitWall == false) // 일반 포물선 운동
        {
            transform.position = Parabola(startPos, endPos, parabolaHighestHeight, fParabolaTimer / 1f);
        }
        else if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f, 1 << LayerMask.NameToLayer("Ground")))    // 빵꾸난 곳
        {
            isHitWall = true;
            fFreeFallTimer += Time.deltaTime;
            transform.position = FreeFall(Parabola(transform.position, hit.point, parabolaHighestHeight, fParabolaTimer / 1f).y, fFreeFallTimer);
        }
        return Physics.Raycast(transform.position, Vector3.down, 1f, 1 << LayerMask.NameToLayer("Ground")) && fParabolaTimer >= 0.5f;
    }


    #endregion

    #region RSkill Part
    private float fCoolDownCutAndRestoreTime = 2f;
    private int fRSkillConsumeTime = 5;
    public void OnRSkill(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            UIManager.instance.OnSkillBtn(KeyCode.R);
        }
        //if (FeverGauge.Instance.IsDoubleFeverGaugeFull() == false && FeverGauge.Instance.IsFeverGaugeFull() == true && IsFeverTime() == false)
        //{
        //    CutCoolDown(fCoolDownCutAndRestoreTime);
        //    SetIsFeverTime(true);
        //    StartCoroutine(FeverGauge.Instance.StartRedFeverTime());
        //}

        if (ctx.started && EnableSkill() == true && RSkillGauge.Instance.IsRSkillGaugeFull() == true && IsRSkillTime() == false)
        {
            CutCoolDown(fCoolDownCutAndRestoreTime);
            SetIsRSkillTime(true);
            srtCurrentSkill = srtRSkill;
            StartCoroutine(StartRedRSkillTime(fRSkillConsumeTime));
        }
    }

    public IEnumerator StartRedRSkillTime(float rSkillTime)
    {
        bool isUsed = false;
        float fRedGaugeTimer = 0f;

        while (isUsed == false)
        {
            if (fRedGaugeTimer >= rSkillTime)
            {
                isUsed = true;
            }
            fRedGaugeTimer += Time.deltaTime;
            yield return null;
        }
        RSkillGauge.Instance.fRedGauge = 0f;
        RestoreCoolDown(fCoolDownCutAndRestoreTime);
        SetIsRSkillTime(false);
    }

    public override float GetCoolDownCutAndRestoreTime()
    {
        return fCoolDownCutAndRestoreTime;
    }
    #endregion


    protected override void DoSkillWithoutPressKey(SkillType skillType, Vector3 t)
    {
        switch (skillType)
        {
            case SkillType.QSkill:
                UseQSkillWithoutKey(t);
                break;
            case SkillType.WSkill:
                UseWSkillWithoutKey(t);
                break;
            case SkillType.ESkill:
                UseESkillWithoutKey(t);
                break;
            default:
                Debug.Log("SkillType is Null!");
            break;
        }
    }

    private Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        parabolaHeight = -4 * height * t * t + 4 * height * t;

        Vector3 mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, parabolaHeight + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    private Vector3 FreeFall(float height, float t)
    {
        float freeFallY = height + 1 / 2 * 9.82f * (t * t);

        return new Vector3(transform.position.x, freeFallY, transform.position.z);
    }
}