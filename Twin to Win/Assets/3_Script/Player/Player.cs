using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    public PlayerbleCharacter cCurrentCharacter;
    [SerializeField] private PlayerbleCharacter cGreatSword;
    [SerializeField] private PlayerbleCharacter cTwinSword;

    #region Dodge Var

    public float fDodgePower = 30f;
    public float fDodgeCoolDown = 3f;
    [HideInInspector] public float fDodgePlayTime = 0.1f;
    [HideInInspector] public float fDodgeTimer = 99f;
    [HideInInspector] public float fDodgePlayTimer = 0f;
    [HideInInspector] public bool isDodging = false;
    [HideInInspector] public bool canDodge = true;
    #endregion

    #region Tag Var
    [HideInInspector] public bool canTag = true;
    [HideInInspector] public float fTagTimer = 99f;
    public float fTagCoolDown = 2f;
    #endregion

    #region Move Var
    private Transform tCurrentTrans;
    private bool isHoldingState = false;
    #endregion

    #region Fever Var
    [HideInInspector] public bool isDoubleFeverTime = false;
    #endregion

    private void Awake()
    {
        instance = this;
        cCurrentCharacter = cTwinSword.gameObject.activeSelf ? cTwinSword : cGreatSword;
    }
    public void ConvertCharacter()
    {
        ResetFever();
        InActiveCurrentCharacter();

        cCurrentCharacter = (cCurrentCharacter == cTwinSword) ? cGreatSword : cTwinSword;

        ActiveNextCharacter();
        FeverGauge.instance.ChangeState(cCurrentCharacter);

        StartCoroutine(cCurrentCharacter.StartTagCoolDown());
    }

    private void ResetFever()
    {
        if (FeverGauge.instance.IsDoubleFeverGaugeFull() == false)
        {
            cCurrentCharacter.RestoreCoolDown(cCurrentCharacter.GetCoolDownCutAndRestoreTime());
            FeverGauge.instance.ResetGaugeWhenTag();
        }
        
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

    public PlayerbleCharacter GetTwinSword()
    {
        return cTwinSword;
    }

    public PlayerbleCharacter GetGreatSword()
    {
        return cGreatSword;
    }
    public float GetTagTimer() 
    {
        return fTagTimer / fTagCoolDown;
    }
    public float GetDodgeTimer()
    {
        return fDodgeTimer / fDodgeCoolDown;
    }
}
