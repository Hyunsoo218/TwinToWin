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
    public readonly float fMaxStamina = 10f;
    public float fUsingDodgeStamina = 3f;

    private readonly float fPlayerMaxHealthPoint = 3000f;
    private float fPlayerCurrentHealthPoint = 0f;


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

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
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

        print("플레이어 피회복, 스테미너 회복, WTD WGS 생성");
    }
    public void ConvertCharacter()
    {
        ResetRSkill();
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
        return fPlayerCurrentHealthPoint >= 0f ? fPlayerCurrentHealthPoint : 0f;
    }

    public void SetPlayerHp(float hp)
    {
        this.fPlayerCurrentHealthPoint = hp;
    }

    private void ResetRSkill()
    {
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
    public void EnablePlayerInput(bool canUseInput)
    {
        PlayerInput wtdPlayerInput = cTwinSword.GetComponent<PlayerInput>();
        PlayerInput wgsPlayerInput = cGreatSword.GetComponent<PlayerInput>();
        
        wtdPlayerInput.enabled = canUseInput;
        wgsPlayerInput.enabled = canUseInput;
    }
}
