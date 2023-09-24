using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DodgeGauge : MonoBehaviour
{
    public static DodgeGauge instance { get; private set; }

    public float fFillTime = 10f;
    public float fDecreaseAmountUsingDodge = 0.25f;

    private Slider sldDodgeGaugeSlider;

    private void Awake()
    {
        Initialize();
        
    }

    private void Start()
    {
        GameManager.instance.AsynchronousExecution(StartFillDodgeGauge());
    }

    private void Initialize()
    {
        if (instance == null)
        {
            instance = this;
        }
        sldDodgeGaugeSlider = GetComponent<Slider>();
        sldDodgeGaugeSlider.value = 0f;
    }

    private IEnumerator StartFillDodgeGauge()
    {
        while (sldDodgeGaugeSlider.value < 1.2f)
        {
            if (sldDodgeGaugeSlider.value < 1f)
            {
                sldDodgeGaugeSlider.value += Time.deltaTime / fFillTime;
            }
            yield return null;
        }
    }

    public bool IsUsedDodge()
    {
        if (sldDodgeGaugeSlider.value > 0.25f)
        {
            sldDodgeGaugeSlider.value -= fDecreaseAmountUsingDodge;
            return true;
        }
        return false;
    }
}
