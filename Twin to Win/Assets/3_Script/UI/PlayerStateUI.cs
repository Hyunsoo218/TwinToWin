using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateUI : MonoBehaviour
{
	[SerializeField] private List<Animator> arrWTDSkills; 
	[SerializeField] private List<Animator> arrWGSSkills;
	[SerializeField] private List<Image> arrSkillFill_WTD;
	[SerializeField] private List<Image> arrSkillFill_WGS;
	[SerializeField] private Animator cConvertUIAnimator;
	[SerializeField] private Image imgConvertFill;
	[SerializeField] private Transform tWTDState;
	[SerializeField] private Slider sHp;
	[SerializeField] private Slider sStamina;
	[SerializeField] private Sprite WTD_R_OFF;
	[SerializeField] private Sprite WTD_R_ON;
	[SerializeField] private Sprite WGS_R_OFF;
	[SerializeField] private Sprite WGS_R_ON;
	[SerializeField] private Slider cHealthPoint;
    private Animator cAnimator;

	private void Awake()
	{
		cAnimator = GetComponent<Animator>();
	}
    private void Start()
    {
        sStamina.maxValue = Player.instance.fMaxStamina;
		cHealthPoint.maxValue = 100f;
		cHealthPoint.value = 100f;
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
	public void OnButton(KeyCode key) 
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
		animator.SetTrigger("On");
	}
	public void SetSkillFill() 
	{
		PlayerbleCharacter cWTD = Player.instance.GetTwinSword();
		arrSkillFill_WTD[0].fillAmount = 1f - cWTD.GetSkillTimer(SkillType.QSkill);
        arrSkillFill_WTD[1].fillAmount = 1f - cWTD.GetSkillTimer(SkillType.WSkill);
        arrSkillFill_WTD[2].fillAmount = 1f - cWTD.GetSkillTimer(SkillType.ESkill);

        PlayerbleCharacter cWGS = Player.instance.GetGreatSword();
		arrSkillFill_WGS[0].fillAmount = 1f - cWGS.GetSkillTimer(SkillType.QSkill);
		arrSkillFill_WGS[1].fillAmount = 1f - cWGS.GetSkillTimer(SkillType.WSkill);
		arrSkillFill_WGS[2].fillAmount = 1f - cWGS.GetSkillTimer(SkillType.ESkill);

        imgConvertFill.fillAmount = 1f - Player.instance.GetTagTimer();
		sStamina.value = Player.instance.fCurrentStamina;

        arrSkillFill_WTD[3].fillAmount = FeverGauge.Instance.GetFeverGauge(cWTD);
        arrSkillFill_WTD[3].sprite = arrSkillFill_WTD[3].fillAmount == 1f ? WTD_R_ON : WTD_R_OFF;

        arrSkillFill_WGS[3].fillAmount = FeverGauge.Instance.GetFeverGauge(cWGS);
        arrSkillFill_WGS[3].sprite = arrSkillFill_WGS[3].fillAmount == 1f ? WGS_R_ON : WGS_R_OFF;
    }
	public void SetPlayerHealthPoint(float healthPoint) 
	{
        cHealthPoint.value = healthPoint;
    }
}