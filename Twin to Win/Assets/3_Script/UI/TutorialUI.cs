using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    private TutorialType type;
    private Animator cAnimator;
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
}
public enum TutorialType 
{
    Player, Tag
}