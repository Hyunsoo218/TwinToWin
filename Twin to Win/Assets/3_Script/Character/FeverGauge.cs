
using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FeverGauge : MonoBehaviour
{
    public static FeverGauge instance;

    private StateMachine cStateMachine;
    private State cRedGauge = new State("RedGauge");
    private State cBlueGauge = new State("BlueGauge");

    public Image imgGaugeFillImage;
    private Slider sldFeverGaugeSlider;

    private float fRedGauge = 0f;
    private float fBlueGauge = 0f;
    public float fIncreaseAttackGaugeAmount = 5f;
    public float fIncreaseSkillGaugeAmount = 10f;
    public float fFeverTime = 5f;

    private void Awake()
    {
        Initialize();
        StateInitalizeOnStay();
    }

    private void Update()
    {
        SetFeverGaugeValue();
    }

    private void Initialize()
    {
        if (instance == null)
        {
            instance = this;
        }
        fIncreaseAttackGaugeAmount /= 100f;
        fIncreaseSkillGaugeAmount /= 100f;
        sldFeverGaugeSlider = GetComponent<Slider>();
        cStateMachine = GetComponent<StateMachine>();
        ChangeState(Player.instance.cCurrentCharacter);
    }

    private void StateInitalizeOnStay()
    {
        cRedGauge.onStay += () =>
        {
            imgGaugeFillImage.color = Color.red;
        };
        cBlueGauge.onStay += () =>
        {
            imgGaugeFillImage.color = Color.blue;
        };
    }

    public void ChangeState(PlayerbleCharacter currentCharacter)
    {
        if (currentCharacter == Player.instance.GetTwinSword())
        {
            cStateMachine.ChangeState(cRedGauge);
        }
        else if (currentCharacter == Player.instance.GetGreatSword())
        {
            cStateMachine.ChangeState(cBlueGauge);
        }
    }

    public void IncreaseNormalAttackFeverGauge()
    {
        if (cStateMachine.GetCurrentState() == cRedGauge && Player.instance.cCurrentCharacter.IsFeverTime() == false && fRedGauge < 1f)
        {
            fRedGauge += fIncreaseAttackGaugeAmount;
        }
        else if (cStateMachine.GetCurrentState() == cBlueGauge && Player.instance.cCurrentCharacter.IsFeverTime() == false && fBlueGauge < 1f)
        {
            fBlueGauge += fIncreaseAttackGaugeAmount;
        }
    }

    public void IncreaseSkillFeverGauge()
    {
        if (cStateMachine.GetCurrentState() == cRedGauge && Player.instance.cCurrentCharacter.IsFeverTime() == false && fRedGauge < 1f)
        {
            fRedGauge += fIncreaseSkillGaugeAmount;
        }
        else if (cStateMachine.GetCurrentState() == cBlueGauge && Player.instance.cCurrentCharacter.IsFeverTime() == false && fBlueGauge < 1f)
        {
            fBlueGauge += fIncreaseSkillGaugeAmount;
        }
    }

    private void SetFeverGaugeValue()
    {
        if (cStateMachine.GetCurrentState() == cRedGauge)
        {
            sldFeverGaugeSlider.value = fRedGauge;
        }
        else if (cStateMachine.GetCurrentState() == cBlueGauge)
        {
            sldFeverGaugeSlider.value = fBlueGauge;
        }
    }

    public bool IsFeverGaugeFull()
    {
        return cStateMachine.GetCurrentState() == cRedGauge ? Math.Round(fRedGauge, 2) >= 1f : Math.Round(fBlueGauge, 2) >= 1f;
    }
    public bool IsDoubleFeverGaugeFull()
    {
        if (Math.Round(fRedGauge, 2) >= 1f && Math.Round(fBlueGauge, 2) >= 1f && Player.instance.isDoubleFeverTime == false)
        {
            Player.instance.isDoubleFeverTime = true;
        }
        return Player.instance.isDoubleFeverTime;
    }

    public IEnumerator StartRedFeverTime()
    {
        while (fRedGauge > 0)
        {
            fRedGauge -= Time.deltaTime / fFeverTime;
            yield return null;
        }
        Player.instance.GetTwinSword().RestoreCoolDown(Player.instance.GetTwinSword().GetCoolDownCutAndRestoreTime());
        Player.instance.GetTwinSword().SetIsFeverTime(false);
        Player.instance.isDoubleFeverTime = false;
        Constants.fSpeedConstant = 1f;
    }
    public IEnumerator StartBlueFeverTime()
    {
        while (fBlueGauge > 0)
        {
            fBlueGauge -= Time.deltaTime / fFeverTime;
            yield return null;
        }
        Player.instance.GetGreatSword().RestoreCoolDown(Player.instance.GetGreatSword().GetCoolDownCutAndRestoreTime());
        Player.instance.GetGreatSword().SetIsFeverTime(false);
        Player.instance.isDoubleFeverTime = false;
        Constants.fSpeedConstant = 1f;
    }

    public void ResetGaugeWhenTag()
    {
        if (Player.instance.cCurrentCharacter.IsFeverTime() == true)
        {
            if (cStateMachine.GetCurrentState() == cRedGauge)
            {
                fRedGauge = 0f;
            }
            else if (cStateMachine.GetCurrentState() == cBlueGauge)
            {
                fBlueGauge = 0f;
            }
            Player.instance.cCurrentCharacter.SetIsFeverTime(false);
        }
    }
}
