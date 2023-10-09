using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Hpbar : MonoBehaviour
{
    [SerializeField] private Slider main;
    [SerializeField] private Slider sub;
    private Coroutine SetSubSliderCoroutine = null;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void Initialize(float max, float min, float current) 
    {
        main.maxValue = max; 
        sub.maxValue = max; 

        main.minValue = min;
        sub.minValue = min;

        main.value = current;
        sub.value = current;
    }
    public void Set(float current)
    {
        main.value = current;
        if (SetSubSliderCoroutine != null ) StopCoroutine( SetSubSliderCoroutine ); 
        SetSubSliderCoroutine = StartCoroutine(SetSubSlider());
    }
    private IEnumerator SetSubSlider() 
    {
        yield return new WaitForSeconds(0.3f);
        float runTime = 0.2f;
        float timer = 0;
        float targetValue = main.value;
        float currentValue = sub.value;
        float moveValue = currentValue - targetValue;
        float progress;
        while (timer < runTime)
        {
            timer += Time.deltaTime;
            progress = timer / runTime;
            sub.value = currentValue - moveValue * progress;
            yield return null;
        }
    }
    public void Disable()
    {
        animator.SetTrigger("Disable");
        Invoke("Hide", 0.5f);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
