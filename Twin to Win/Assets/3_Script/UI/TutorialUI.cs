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
    private int currentDataNum = -1;
    private void Awake()
    {
        cAnimator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { OnData(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { OnData(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { OnData(2); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { OnData(3); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { OnData(4); }
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

        bool wait = true;
        while (wait)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (type == TutorialType.Player)
                {
                    type = TutorialType.Tag;
                    cAnimator.SetTrigger("Tag");
                }
                else
                {
                    wait = false;
                    cAnimator.SetTrigger("End");
                }
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (type == TutorialType.Tag)
                {
                    type = TutorialType.Player;
                    cAnimator.SetTrigger("Player");
                }
            }
            yield return null;
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
}
public enum TutorialType 
{
    Player, Tag
}