using System;
using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
//using static UnityEditor.Rendering.InspectorCurveEditor;

public class Player : MonoBehaviour
{
    public static Player instance;
    public PlayerbleCharacter cCurrentCharacter;
    [SerializeField] private GameObject objWTD;
    [SerializeField] private GameObject objWGS;
    private WGSPlayableCharacter cGreatSword;
    private WTDPlayableCharacter cTwinSword;

    public float fCurrentStamina = 10f;
    public readonly float fMaxStamina = 10f;
    public float fUsingDodgeStamina = 3f;

    public float fMoveSpeed = 3f;

    [SerializeField] private float fPlayerMaxHealthPoint = 1000f;
    [SerializeField] private float fPlayerCurrentHealthPoint = 0f;

    #region Dodge Var

    public float fDodgePower = 30f;
    [HideInInspector] public float fDodgePlayTime = 0.1f;
    [HideInInspector] public float fDodgePlayTimer = 0f;
    [HideInInspector] public bool isDodging = false;
    #endregion

    #region Tag Var
    [HideInInspector] public bool canTag = true;
    [HideInInspector] public float fTagTimer = 99f;
    public float fTagCoolDown = 2f;
    #endregion

    #region Move Var
    private Transform tCurrentTrans;
    private bool isHoldingState = false;

    [HideInInspector] public State cMoveState = new State("moveState");
    [HideInInspector] public bool isMoving = false;
    [HideInInspector] public Coroutine moveCoroutine = null;
    #endregion

    #region RSkill Var
    public float fRedGauge = 0f;
    public float fBlueGauge = 0f;
    public float fIncreaseAttackGaugeAmount = 0.02f;
    public float fIncreaseSkillGaugeAmount = 0.1f; // default 0.1
    #endregion

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        cMoveState.onEnter += () => { cCurrentCharacter.ChangeAnimation(cMoveState.strStateName); isMoving = true; };
    }
    public void SetGame() 
    {
        fPlayerCurrentHealthPoint = fPlayerMaxHealthPoint;
        UIManager.instance.SetPlayerHealthPoint();
        StartCoroutine(RecoverStamina());
        GameObject wtd = Instantiate(objWTD, new Vector3(4f, 0.5f, 4f), Quaternion.Euler(0, 45f, 0));
        GameObject wgs = Instantiate(objWGS, new Vector3(4f, 0.5f, 4f), Quaternion.Euler(0, 45f, 0));

        cTwinSword = (WTDPlayableCharacter)wtd.GetComponent<PlayerbleCharacter>();
        cGreatSword = (WGSPlayableCharacter)wgs.GetComponent<PlayerbleCharacter>();

        cGreatSword.gameObject.SetActive(false);

        cCurrentCharacter = cTwinSword.gameObject.activeSelf ? cTwinSword : cGreatSword;

        ResetAllCharacterRSkillGauge();
    }
    public void ConvertCharacter()
    {
        ResetWTDRSkillGauge();
        ResetPrevAllSkillButton();
        InActiveCurrentCharacter();

        cCurrentCharacter = (cCurrentCharacter == cTwinSword) ? cGreatSword : cTwinSword;
        ActiveNextCharacter();

        StartCoroutine(cCurrentCharacter.StartTagCoolDown());
    }

    public float GetPlayerMaxHp()
    {
        return fPlayerMaxHealthPoint;
    }

    public float GetPlayerHp()
    {
        return fPlayerCurrentHealthPoint > 0f ? fPlayerCurrentHealthPoint : 0f;
    }

    public void SetPlayerHp(float hp)
    {
        this.fPlayerCurrentHealthPoint = hp;
    }

    private void ResetWTDRSkillGauge()
    {
        cCurrentCharacter.RestoreCoolDown(cCurrentCharacter.GetCoolDownCutAndRestoreTime());
        ResetCurrentCharacterRSkillGauge();
    }

    private void ResetPrevAllSkillButton()
    {
        UIManager.instance.OnSkillBtn(KeyCode.Q);
        UIManager.instance.OnSkillBtn(KeyCode.W);
        UIManager.instance.OnSkillBtn(KeyCode.E);
        UIManager.instance.OnSkillBtn(KeyCode.R);
    }

    private void InActiveCurrentCharacter()
	{
        tCurrentTrans = cCurrentCharacter.transform;
        if (cCurrentCharacter.eMouseState == mouseState.Hold)
        {
            isHoldingState = true;
        }
        cCurrentCharacter.gameObject.SetActive(false);
    }
	private void ActiveNextCharacter()
    {
        cCurrentCharacter.transform.position = tCurrentTrans.position;
        if (isHoldingState == true)
        {
            cCurrentCharacter.eMouseState = mouseState.Hold;
            isHoldingState = false;
        }
        cCurrentCharacter.gameObject.SetActive(true);
    }
	public string GetCurrentCharacterStateName()
	{
		return cCurrentCharacter.GetCurrentStateName();
	}

    public WTDPlayableCharacter GetTwinSword()
    {
        return cTwinSword;
    }

    public WGSPlayableCharacter GetGreatSword()
    {
        return cGreatSword;
    }
    public PlayerSkillTimeInfo GetTagTimer() 
    {
        return new PlayerSkillTimeInfo(fTagCoolDown, fTagTimer);
    }
    public bool CanDodge() 
    {
        if (fCurrentStamina >= fUsingDodgeStamina)
        {
            fCurrentStamina -= fUsingDodgeStamina;
            return true;
        }
        return false;
    }
    private IEnumerator RecoverStamina() 
    {
        while (true)
        {
            if (fCurrentStamina < fMaxStamina)
            {
                fCurrentStamina += Time.deltaTime;
            }
            yield return null;
        }
    }

    public void StartMoveCoroutine(Vector3 mousePosOnGround, Quaternion mouseAngle)
    {
        moveCoroutine = StartCoroutine(MoveCoroutine(mousePosOnGround, mouseAngle));
    }

    public void StopPlayerCoroutine(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
    }

    public IEnumerator MoveCoroutine(Vector3 mousePosOnGround, Quaternion mouseAngle)
    {
        while (isMoving == true)
        {
            if (cCurrentCharacter.GetCurrentStateName() != "moveState")
            {
                cCurrentCharacter.ChangeState(cMoveState);
                
            }
            cCurrentCharacter.transform.localRotation = mouseAngle;
            if (Vector3.Distance(cCurrentCharacter.transform.position, mousePosOnGround) <= 0.1f)
            {
                isMoving = false;
                cCurrentCharacter.eMouseState = mouseState.None;
                cCurrentCharacter.ChangeState("cIdleState");
                yield break;
            }
            cCurrentCharacter.transform.position = Vector3.MoveTowards(cCurrentCharacter.transform.position, mousePosOnGround, Time.deltaTime * fMoveSpeed);

            yield return null;
        }
    }

    public void IncreaseRSkillGaugeUsingAttack()
    {
        if (cCurrentCharacter == cTwinSword && cCurrentCharacter.IsRSkillTime() == false && fRedGauge < 1f)
        {
            fRedGauge += fIncreaseAttackGaugeAmount;
        }
        else if (cCurrentCharacter == cGreatSword && cCurrentCharacter.IsRSkillTime() == false && fBlueGauge < 1f)
        {
            fBlueGauge += fIncreaseAttackGaugeAmount;
        }
    }

    public void IncreaseRSkillGaugeUsingSkill()
    {
        if (cCurrentCharacter == cTwinSword && cCurrentCharacter.IsRSkillTime() == false && fRedGauge < 1f)
        {
            fRedGauge += fIncreaseSkillGaugeAmount;
        }
        else if (cCurrentCharacter == cGreatSword && cCurrentCharacter.IsRSkillTime() == false && fBlueGauge < 1f)
        {
            fBlueGauge += fIncreaseSkillGaugeAmount;
        }
    }

    public bool IsRSkillGaugeFull()
    {
        return cCurrentCharacter == cTwinSword ? Math.Round(fRedGauge, 2) >= 1f : Math.Round(fBlueGauge, 2) >= 1f;
    }

    public void ResetCurrentCharacterRSkillGauge()
    {
        if (cCurrentCharacter.IsRSkillTime() == true)
        {
            if (cCurrentCharacter == cTwinSword)
            {
                fRedGauge = 0f;
            }
            else if (cCurrentCharacter == cGreatSword)
            {
                fBlueGauge = 0f;
            }
            cCurrentCharacter.SetIsRSkillTime(false);
        }
    }

    public void ResetAllCharacterRSkillGauge()
    {
        fRedGauge = 0f;
        fBlueGauge = 0f;
        if (cTwinSword.IsRSkillTime() == true)
        {
            cTwinSword.SetIsRSkillTime(false);
        }
        else if (cGreatSword.IsRSkillTime() == true)
        {
            cGreatSword.SetIsRSkillTime(false);
        }
    }


    public float GetRSkillGauge(PlayerbleCharacter character)
    {
        return character == cTwinSword ? fRedGauge : fBlueGauge;
    }

    public void EnablePlayerInput(bool canUseInput)
    {
        cCurrentCharacter.ReturnToIdle();

        PlayerInput wtdPlayerInput = cTwinSword.GetComponent<PlayerInput>();
        PlayerInput wgsPlayerInput = cGreatSword.GetComponent<PlayerInput>();
        
        wtdPlayerInput.enabled = canUseInput;
        wgsPlayerInput.enabled = canUseInput;
    }
}
