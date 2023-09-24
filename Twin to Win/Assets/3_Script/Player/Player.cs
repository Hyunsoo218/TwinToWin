using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player instance;
    public PlayerbleCharacter cCurrentCharacter;
    [SerializeField] private PlayerbleCharacter cGreatSword;
    [SerializeField] private PlayerbleCharacter cTwinSword;

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

    #region Fever Var
    [HideInInspector] public bool isDoubleFeverTime = false;
    [HideInInspector] public float fDoubleFeverTimer = 0f;
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

        StartCoroutine(cCurrentCharacter.StartTagCoolDown());
    }

    private void ResetFever()
    {
        if (FeverGauge.Instance.IsDoubleFeverGaugeFull() == false)
        {
            cCurrentCharacter.RestoreCoolDown(cCurrentCharacter.GetCoolDownCutAndRestoreTime());
            FeverGauge.Instance.ResetGaugeWhenTag();
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
}
