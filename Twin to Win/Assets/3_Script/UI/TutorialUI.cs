using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    private TutorialType type;
    private Animator cAnimator;
    [SerializeField] private List<Animator> dataAnimators;   
    [SerializeField] private List<Animator> navAnimators;
    [SerializeField] private Animator lArrowBtnAnimator;
    [SerializeField] private Animator rArrowBtnAnimator;
    private int currentDataNum = -1;
    private bool waitTutorial = true;
    private void Awake()
    {
        cAnimator = GetComponent<Animator>();
    }
    public void SetPlayer(bool active) 
    {
        string trigger = (active) ? "OnPlayer" : "OffPlayer";
        cAnimator.SetTrigger(trigger);
    }
    public IEnumerator WaitForTutorial()
    {
        type = TutorialType.Player;
        cAnimator.SetTrigger("Start");
        waitTutorial = true;
        while (waitTutorial)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnMouseClickArrowBtn(false);
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                OnMouseClickArrowBtn(true);
            }
            yield return null;
        }
    }
    private void NextBtn() 
    {
        if (type == TutorialType.Player)
        {
            type = TutorialType.Tag;
            cAnimator.SetTrigger("Tag");
        }
        else
        {
            if (currentDataNum == navAnimators.Count - 1)
            {
                cAnimator.SetTrigger("End");
                waitTutorial = false;
            }
            else
            {
                OnData(currentDataNum + 1);
            }
        }
    }
    private void PrevBtn() 
    {
        if (type == TutorialType.Tag)
        {
            if (currentDataNum == 0)
            {
                type = TutorialType.Player;
                cAnimator.SetTrigger("Player");
            }
            else
            {
                OnData(currentDataNum - 1);
            }
        }
    }
    public void OnData(int num) 
    {
        if (currentDataNum == num) return; 
        if (currentDataNum != -1)
        {
            dataAnimators[currentDataNum].SetTrigger("Off");
            navAnimators[currentDataNum].SetTrigger("Off");
        }
        currentDataNum = num;
        dataAnimators[num].SetTrigger("On");
        navAnimators[num].SetTrigger("On");
    }
    public void OnMouseEnterArrowBtn(bool left)
    {
        if (left) 
        {
            lArrowBtnAnimator.SetTrigger("L_On");
        }
        else
        {
            rArrowBtnAnimator.SetTrigger("R_On");
        }
    }
    public void OnMouseExitArrowBtn(bool left)
    {
        if (left)
        {
            lArrowBtnAnimator.SetTrigger("L_Off");
        }
        else
        {
            rArrowBtnAnimator.SetTrigger("R_Off");
        }
    }
    public void OnMouseClickArrowBtn(bool left)
    {
        if (left)
        {
            lArrowBtnAnimator.SetTrigger("Click");
            PrevBtn();
        }
        else
        {
            rArrowBtnAnimator.SetTrigger("Click");
            NextBtn();
        }
    }
}
public enum TutorialType 
{
    Player, Tag
}