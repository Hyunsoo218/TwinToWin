using System;

public class RSkillGauge 
{
    private static RSkillGauge instance;
    public static RSkillGauge Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new RSkillGauge();
            }
            return instance;
        }
    }

    public float fRedGauge = 0f;
    public float fBlueGauge = 0f;
    public float fIncreaseAttackGaugeAmount = 0.02f;
    public float fIncreaseSkillGaugeAmount = 1f; // default 0.1
    public RSkillGauge()
    {

    }

    public void IncreaseRSkillGaugeUsingAttack()
    {
        if (IsTwin() == true && Player.instance.cCurrentCharacter.IsRSkillTime() == false && fRedGauge < 1f)
        {
            fRedGauge += fIncreaseAttackGaugeAmount;
        }
        else if (IsTwin() == false && Player.instance.cCurrentCharacter.IsRSkillTime() == false && fBlueGauge < 1f)
        {
            fBlueGauge += fIncreaseAttackGaugeAmount;
        }
    }

    public void IncreaseRSkillGaugeUsingSkill()
    {
        if (IsTwin() == true && Player.instance.cCurrentCharacter.IsRSkillTime() == false && fRedGauge < 1f)
        {
            fRedGauge += fIncreaseSkillGaugeAmount;
        }
        else if (IsTwin() == false && Player.instance.cCurrentCharacter.IsRSkillTime() == false && fBlueGauge < 1f)
        {
            fBlueGauge += fIncreaseSkillGaugeAmount;
        }
    }

    public bool IsRSkillGaugeFull()
    {
        return IsTwin() == true ? Math.Round(fRedGauge, 2) >= 1f : Math.Round(fBlueGauge, 2) >= 1f;
    }

    public void ResetGaugeWhenTag()
    {
        if (Player.instance.cCurrentCharacter.IsRSkillTime() == true)
        {
            if (IsTwin() == true)
            {
                fRedGauge = 0f;
            }
            else if (IsTwin() == false)
            {
                fBlueGauge = 0f;
            }
            Player.instance.cCurrentCharacter.SetIsRSkillTime(false);
        }
    }

    private bool IsTwin()
    {
        return Player.instance.cCurrentCharacter == Player.instance.GetTwinSword();
    }

    public float GetRSkillGauge(PlayerbleCharacter playerType)
    {
        return playerType == Player.instance.GetTwinSword() ? fRedGauge : fBlueGauge;
    }
}
