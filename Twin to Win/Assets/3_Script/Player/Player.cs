using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player instance;
    public PlayerbleCharacter cCurrentCharacter;
    [SerializeField] private GameObject objWTD;
    [SerializeField] private GameObject objWGS;
    private WGSPlayableCharacter cGreatSword;
    private WTDPlayableCharacter cTwinSword;
    public float fCurrentStamina = 10f;
    public float fMaxStamina = 10f;
    public float fDodgeStamina = 3f;


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
    #endregion

    //#region Fever Var
    //[HideInInspector] public bool isDoubleFeverTime = false;
    //[HideInInspector] public float fDoubleFeverTimer = 0f;
    //#endregion

    private void Awake()
    {
        instance = this;
        StartCoroutine(RecoverStamina());
        GameObject wtd = Instantiate(objWTD, new Vector3(0, 0.5f, 0), Quaternion.Euler(0, 45f, 0));
        GameObject wgs = Instantiate(objWGS, new Vector3(0, 0.5f, 0), Quaternion.Euler(0, 45f, 0));

        cTwinSword = (WTDPlayableCharacter)wtd.GetComponent<PlayerbleCharacter>();
        cGreatSword = (WGSPlayableCharacter)wgs.GetComponent<PlayerbleCharacter>();

        cGreatSword.gameObject.SetActive(false);

        cCurrentCharacter = cTwinSword.gameObject.activeSelf ? cTwinSword : cGreatSword;
    }
    public void ConvertCharacter()
    {
        ResetRSkill();
        InActiveCurrentCharacter();

        cCurrentCharacter = (cCurrentCharacter == cTwinSword) ? cGreatSword : cTwinSword;

        ActiveNextCharacter();

        StartCoroutine(cCurrentCharacter.StartTagCoolDown());
    }
    private void ResetRSkill()
    {
        //if (FeverGauge.Instance.IsDoubleFeverGaugeFull() == false)
        //{
        //    cCurrentCharacter.RestoreCoolDown(cCurrentCharacter.GetCoolDownCutAndRestoreTime());
        //    FeverGauge.Instance.ResetGaugeWhenTag();
        //}

        cCurrentCharacter.RestoreCoolDown(cCurrentCharacter.GetCoolDownCutAndRestoreTime());
        RSkillGauge.Instance.ResetGaugeWhenTag();

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
    public float GetTagTimer() 
    {
        return fTagTimer / fTagCoolDown;
    }
    public bool CanDodge() 
    {
        if (fCurrentStamina >= fDodgeStamina)
        {
            fCurrentStamina -= fDodgeStamina;
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
    public void EnableCurrentPlayerInput(bool canUseInput)
    {
        // 둘 다 안되게
        PlayerInput wtdPlayerInput = cGreatSword.GetComponent<PlayerInput>();
        PlayerInput wgsPlayerInput = cGreatSword.GetComponent<PlayerInput>();
        
        wtdPlayerInput.enabled = canUseInput;
        wgsPlayerInput.enabled = canUseInput;
    }
}
