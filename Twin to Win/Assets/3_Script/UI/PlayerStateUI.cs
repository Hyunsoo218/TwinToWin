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
	[SerializeField] private Transform tWTDState;
	[SerializeField] private Slider sHp;
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
        sStamina.maxValue = Player.instance.fMaxStamina;
		float maxHp = Player.instance.GetPlayerMaxHp();
		float hp = Player.instance.GetPlayerHp();
		hpbar.Initialize(maxHp, 0 , hp);
	}
	private void OnEnable()
	{
		PlayerbleCharacter cWTD = Player.instance.GetTwinSword();
		PlayerbleCharacter cCur = Player.instance.cCurrentCharacter;
		string animationTrigger = (cCur == cWTD) ? "ToWTD" : "ToWGS";
		cAnimator.SetTrigger(animationTrigger);
	}
	public void Convert()
	{
		cConvertUIAnimator.SetTrigger("On");

        if (Player.instance.canTag == true && (Player.instance.fTagTimer >= Player.instance.fTagCoolDown || Player.instance.fTagTimer == 0f)) // 가능한지 확인후 교체
        {
            PlayerbleCharacter cWTD = Player.instance.GetTwinSword();
            PlayerbleCharacter cCur = Player.instance.cCurrentCharacter;
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
	}
	public void OnButton(KeyCode key, bool stay = false, bool stayEnd = false)
	{
		PlayerbleCharacter cWTD = Player.instance.GetTwinSword();
		PlayerbleCharacter cCur = Player.instance.cCurrentCharacter;

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
        character = Player.instance.GetTwinSword();
		SetSkillInfo(arrSkillFill_WTD, skillTimes_WTD, character, SkillType.QSkill);
        SetSkillInfo(arrSkillFill_WTD, skillTimes_WTD, character, SkillType.WSkill);
        SetSkillInfo(arrSkillFill_WTD, skillTimes_WTD, character, SkillType.ESkill);
        SetRSkillInfo(arrSkillFill_WTD, skillTimes_WTD, character, SkillType.RSkill);

		// WGS part
        character = Player.instance.GetGreatSword();
		SetSkillInfo(arrSkillFill_WGS, skillTimes_WGS, character, SkillType.QSkill);
		SetSkillInfo(arrSkillFill_WGS, skillTimes_WGS, character, SkillType.WSkill);
        SetSkillInfo(arrSkillFill_WGS, skillTimes_WGS, character, SkillType.ESkill);
        SetRSkillInfo(arrSkillFill_WGS, skillTimes_WGS, character, SkillType.RSkill);

		// etc
        imgConvertFill.fillAmount = Player.instance.GetTagTimer().percentage;
		sStamina.value = Player.instance.fCurrentStamina;
    }
	public void SetPlayerHealthPoint() 
	{
		float hp = Player.instance.GetPlayerHp();
		hpbar.Set(hp);
	}
	private void SetSkillInfo(List<Image> images, List<TextMeshProUGUI> times, PlayerbleCharacter character, SkillType type) 
	{
        PlayerSkillTimeInfo info = character.GetSkillTimer(type);
        images[(int)type].fillAmount = info.percentage;

        string text = "";
        if (info.percentage != 0)
		{
			float remainingTime = info.maxTime - info.currentTime;
			if (remainingTime > 10f)
            {
                text = remainingTime.ToString("N0") + "s";
            }
			else if(remainingTime > 0f)
            {
                text = remainingTime.ToString("N1") + "s";
            }
		}
        times[(int)type].text = text;
    }
    private void SetRSkillInfo(List<Image> images, List<TextMeshProUGUI> times, PlayerbleCharacter character, SkillType type)
    {
		Sprite onImg = (character == Player.instance.GetTwinSword()) ? WTD_R_ON : WGS_R_ON;
		Sprite offImg = (character == Player.instance.GetTwinSword()) ? WTD_R_OFF : WGS_R_OFF;

		float gauge = Player.instance.GetRSkillGauge(character);
        images[(int)type].fillAmount = gauge;
        images[(int)type].sprite = (gauge >= 1f) ? onImg : offImg;

		string text = "";
		if (images[(int)type].sprite != onImg)
		{
			gauge = gauge * 99f;
			text = gauge.ToString("N0") + "%";
		}
        times[(int)type].text = text;
    }
}
public struct PlayerSkillTimeInfo 
{
	public PlayerSkillTimeInfo(float maxTime, float currentTime) 
	{
		this.maxTime = maxTime;
		this.currentTime = currentTime;
		percentage = 1f - currentTime / maxTime;
    }
    public float maxTime;
	public float currentTime;
	public float percentage;
}