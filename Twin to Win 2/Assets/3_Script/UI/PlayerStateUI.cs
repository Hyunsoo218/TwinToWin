using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStateUI : MonoBehaviour
{
	[SerializeField] private List<Animator> arrWTDSkills; 
	[SerializeField] private List<Animator> arrWGSSkills;
	[SerializeField] private List<Image> arrSkillFill_WTD;
	[SerializeField] private List<Image> arrSkillFill_WGS;
    [SerializeField] private List<TextMeshProUGUI> skillTimes_WTD;
    [SerializeField] private List<TextMeshProUGUI> skillTimes_WGS;
    [SerializeField] private Animator cConvertUIAnimator;
	[SerializeField] private Image imgConvertFill;
	[SerializeField] private Image imgLeft;
	[SerializeField] private Image imgRigth;
	[SerializeField] private Transform tWTDState;
	[SerializeField] private Slider sStamina;
	[SerializeField] private Sprite WTD_R_OFF;
	[SerializeField] private Sprite WTD_R_ON;
	[SerializeField] private Sprite WGS_R_OFF;
	[SerializeField] private Sprite WGS_R_ON;
	[SerializeField] private Hpbar hpbar;

    private Animator cAnimator;
    private void Awake()
	{
        cAnimator = GetComponent<Animator>();
	}
    private void Start()
    {
        sStamina.maxValue = Player.Instance.MaxStamina;
        sStamina.minValue = 0;
        sStamina.value = Player.Instance.CurrentStamina;

		float maxHp = Player.Instance.MaxHealthPoint;
		float hp = Player.Instance.CurrentHealthPoint;
		hpbar.Initialize(maxHp, 0 , hp);
	}
	private void OnEnable()
	{
		PlayerbleCharacter cWTD = Player.Instance.SecondCharacter;
		PlayerbleCharacter cCur = Player.Instance.CurrentCharacter;
		string animationTrigger = (cCur == cWTD) ? "ToWTD" : "ToWGS";
		cAnimator.SetTrigger(animationTrigger);
	}
	public void Convert()
	{
		cConvertUIAnimator.SetTrigger("On");

		PlayerbleCharacter cWTD = Player.Instance.SecondCharacter;
		PlayerbleCharacter cCur = Player.Instance.CurrentCharacter;
		string animationTrigger = "";

		if (cCur == cWTD)
		{
			animationTrigger = "ToWGS";
			tWTDState.SetAsLastSibling();
		}
		else
		{
			animationTrigger = "ToWTD";
			tWTDState.SetAsFirstSibling();
		}

		cAnimator.SetTrigger(animationTrigger);
	}
	public void OnButton(KeyCode key, bool stay = false, bool stayEnd = false)
	{
		PlayerbleCharacter cWTD = Player.Instance.SecondCharacter;
		PlayerbleCharacter cCur = Player.Instance.CurrentCharacter;

		List<Animator> animators = (cCur == cWTD) ? arrWTDSkills : arrWGSSkills;

		Animator animator = null;
		switch (key)
		{
			case KeyCode.Q: animator = animators[0]; break;
			case KeyCode.W: animator = animators[1]; break;
			case KeyCode.E: animator = animators[2]; break;
			case KeyCode.R: animator = animators[3]; break;
		}

		string trigger = stay ? "Stay" : "On";

		if (stay && stayEnd)
		{
			trigger = "StayEnd";
        }
		animator.SetTrigger(trigger);
	}
	public void SetSkillFill() 
	{
		PlayerbleCharacter character;
		// WTD part
        character = Player.Instance.SecondCharacter;
		SetSkillInfo(arrSkillFill_WTD, skillTimes_WTD, character, SkillType.Q);
        SetSkillInfo(arrSkillFill_WTD, skillTimes_WTD, character, SkillType.W);
        SetSkillInfo(arrSkillFill_WTD, skillTimes_WTD, character, SkillType.E);
        SetRSkillInfo(arrSkillFill_WTD, skillTimes_WTD, character, SkillType.R);

		// WGS part
        character = Player.Instance.FirstCharacter;
		SetSkillInfo(arrSkillFill_WGS, skillTimes_WGS, character, SkillType.Q);
		SetSkillInfo(arrSkillFill_WGS, skillTimes_WGS, character, SkillType.W);
        SetSkillInfo(arrSkillFill_WGS, skillTimes_WGS, character, SkillType.E);
        SetRSkillInfo(arrSkillFill_WGS, skillTimes_WGS, character, SkillType.R);

		// etc
        imgConvertFill.fillAmount = Player.Instance.GetTagTimer().percentage;
		sStamina.value = Player.Instance.CurrentStamina;
    }
	public void SetPlayerHealthPoint() 
	{
		hpbar.Set(Player.Instance.CurrentHealthPoint);
	}
	public void SetImage(CharacterType left, CharacterType rigth) 
	{
		CharacterInfo lInfo = CharacterManager.instance.GetTypeToInfo(left);
		arrSkillFill_WTD[0].transform.parent.GetComponent<Image>().sprite = lInfo.QSkillImage;
		arrSkillFill_WTD[1].transform.parent.GetComponent<Image>().sprite = lInfo.WSkillImage;
		arrSkillFill_WTD[2].transform.parent.GetComponent<Image>().sprite = lInfo.ESkillImage;
		arrSkillFill_WTD[3].transform.parent.GetComponent<Image>().sprite = lInfo.ROffSkillImage;
		WTD_R_OFF = lInfo.RChargeSkillImage;
		WTD_R_ON = lInfo.RSkillImage;
		imgLeft.sprite = lInfo.ImagePlay;
		CharacterInfo rInfo = CharacterManager.instance.GetTypeToInfo(rigth);
		arrSkillFill_WGS[0].transform.parent.GetComponent<Image>().sprite = rInfo.QSkillImage;
		arrSkillFill_WGS[1].transform.parent.GetComponent<Image>().sprite = rInfo.WSkillImage;
		arrSkillFill_WGS[2].transform.parent.GetComponent<Image>().sprite = rInfo.ESkillImage;
		arrSkillFill_WGS[3].transform.parent.GetComponent<Image>().sprite = rInfo.ROffSkillImage;
		WGS_R_OFF = rInfo.RChargeSkillImage;
		WGS_R_ON = rInfo.RSkillImage;
		imgRigth.sprite = rInfo.ImagePlay;
	}
	private void SetSkillInfo(List<Image> images, List<TextMeshProUGUI> times, PlayerbleCharacter character, SkillType type) 
	{
        SkillTimeInfo info = character.GetSkillTimer(type);
        images[(int)type].fillAmount = info.percentage;

        string text = "";
        if (info.percentage != 0)
		{
			float remainingTime = info.max - info.current;
			if (remainingTime > 10f)
                text = remainingTime.ToString("N0") + "s";
			else if(remainingTime > 0f)
                text = remainingTime.ToString("N1") + "s";
		}
        times[(int)type].text = text;
    }
    private void SetRSkillInfo(List<Image> images, List<TextMeshProUGUI> times, PlayerbleCharacter character, SkillType type)
    {
		Sprite onImg = (character == Player.Instance.SecondCharacter) ? WTD_R_ON : WGS_R_ON;
		Sprite offImg = (character == Player.Instance.SecondCharacter) ? WTD_R_OFF : WGS_R_OFF;

		float gauge = 1f - character.GetSkillTimer(type).percentage;
		images[(int)type].fillAmount = gauge;

		if (gauge >= 1f) 
		{ 
			images[(int)type].sprite = onImg;
			times[(int)type].text = "";
		}
		else
		{
			images[(int)type].sprite = offImg;
			times[(int)type].text = (gauge * 99f).ToString("N0") + "%";
		}
	}
}
