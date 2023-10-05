using System;
using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class FeverGauge 
{
    private static FeverGauge instance;
    public static FeverGauge Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new FeverGauge();
            }
            return instance;
        }
    }

    private float fRedGauge = 0f;
    private float fBlueGauge = 0f;
    public float fIncreaseAttackGaugeAmount = 0.05f;
    public float fIncreaseSkillGaugeAmount = 0.1f;
    public float fFeverTimeTime = 5f;

    public FeverGauge()
    {

    }

    public void IncreaseNormalAttackFeverGauge()
    {
        if (IsTwin() == true && Player.instance.cCurrentCharacter.IsFeverTime() == false && fRedGauge < 1f)
        {
            fRedGauge += fIncreaseAttackGaugeAmount;
        }
        else if (IsTwin() == false && Player.instance.cCurrentCharacter.IsFeverTime() == false && fBlueGauge < 1f)
        {
            fBlueGauge += fIncreaseAttackGaugeAmount;
        }
    }

    public void IncreaseSkillFeverGauge()
    {
        if (IsTwin() == true && Player.instance.cCurrentCharacter.IsFeverTime() == false && fRedGauge < 1f)
        {
            fRedGauge += fIncreaseSkillGaugeAmount;
        }
        else if (IsTwin() == false && Player.instance.cCurrentCharacter.IsFeverTime() == false && fBlueGauge < 1f)
        {
            fBlueGauge += fIncreaseSkillGaugeAmount;
        }
    }

    public bool IsFeverGaugeFull()
    {
        return IsTwin() == true ? Math.Round(fRedGauge, 2) >= 1f : Math.Round(fBlueGauge, 2) >= 1f;
    }
    //public bool IsDoubleFeverGaugeFull()
    //{
    //    if (Math.Round(fRedGauge, 2) >= 1f && Math.Round(fBlueGauge, 2) >= 1f && Player.instance.isDoubleFeverTime == false)
    //    {
    //        Player.instance.isDoubleFeverTime = true;
    //    }
    //    return Player.instance.isDoubleFeverTime;
    //}

    public IEnumerator StartRedFeverTime()
    {
        bool isUsed = false;
        float fRedGaugeTimer = 0f;

        while (isUsed == false)
        {
            if (fRedGaugeTimer >= fFeverTimeTime)
            {
                isUsed = true;
            }
            fRedGaugeTimer += Time.deltaTime;
            yield return null;
        }
        fRedGauge = 0f;
        Player.instance.GetTwinSword().RestoreCoolDown(Player.instance.GetTwinSword().GetCoolDownCutAndRestoreTime());
        Player.instance.GetTwinSword().SetIsFeverTime(false);
        Constants.fSpeedConstant = 1f;
    }
    public IEnumerator StartBlueFeverTime()
    {
        bool isUsed = false;
        float fBlueGaugeTimer = 0f;

        while (isUsed == false)
        {
            if (fBlueGaugeTimer >= fFeverTimeTime)
            {
                isUsed = true;
            }
            fBlueGaugeTimer += Time.deltaTime;
            yield return null;
        }
        fBlueGauge = 0f;
        Player.instance.GetGreatSword().RestoreCoolDown(Player.instance.GetGreatSword().GetCoolDownCutAndRestoreTime());
        Player.instance.GetGreatSword().SetIsFeverTime(false);
        Constants.fSpeedConstant = 1f;
    }

    //public IEnumerator StartDoubleFeverTime()
    //{
    //    fRedGauge = 0f;
    //    fBlueGauge = 0f;
    //    while (Player.instance.fDoubleFeverTimer < fFeverTimeTime)
    //    {
    //        Player.instance.fDoubleFeverTimer += Time.deltaTime;
    //        yield return null;
    //    }
    //    Player.instance.GetGreatSword().RestoreCoolDown(Player.instance.GetGreatSword().GetCoolDownCutAndRestoreTime());
    //    Player.instance.GetTwinSword().RestoreCoolDown(Player.instance.GetTwinSword().GetCoolDownCutAndRestoreTime());
    //    Player.instance.GetGreatSword().SetIsFeverTime(false);
    //    Player.instance.GetTwinSword().SetIsFeverTime(false);
    //    Player.instance.isDoubleFeverTime = false;
    //    Player.instance.fDoubleFeverTimer = 0f;
    //    Constants.fSpeedConstant = 1f;
    //    Player.instance.GetTwinSword().GetAnimator().speed = Constants.fSpeedConstant;
    //    Player.instance.GetGreatSword().GetAnimator().speed = Constants.fSpeedConstant;
    //}

    public void ResetGaugeWhenTag()
    {
        if (Player.instance.cCurrentCharacter.IsFeverTime() == true)
        {
            if (IsTwin() == true)
            {
                fRedGauge = 0f;
            }
            else if (IsTwin() == false)
            {
                fBlueGauge = 0f;
            }
            Player.instance.cCurrentCharacter.SetIsFeverTime(false);
        }
    }

    private bool IsTwin()
    {
        return Player.instance.cCurrentCharacter == Player.instance.GetTwinSword();
    }

    public float GetFeverGauge(PlayerbleCharacter playerType)
    {
        return playerType == Player.instance.GetTwinSword() ? fRedGauge : fBlueGauge;
    }
}
