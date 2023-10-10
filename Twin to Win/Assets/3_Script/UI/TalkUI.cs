using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkUI : MonoBehaviour
{
    [SerializeField] private GameObject talkBox;
    [SerializeField] private TextMeshProUGUI talker;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image line;
    [SerializeField] private GameObject nextObj;
    [SerializeField] private GameObject nextClickObj;
    private void Awake()
    {
        talkBox.SetActive(false);
    }
    public IEnumerator ShowText(string name, string script, float autoClickTime = -1f) 
    {
        talkBox.SetActive(true);
        nextObj.SetActive(false);
        nextClickObj.SetActive(false);

        text.text = script;
        float lineDist;
        if (text.preferredWidth <= 200f) lineDist = 300f; 
        else if (text.preferredWidth >= 800f) lineDist = 900f; 
        else lineDist = text.preferredWidth + 100f; 
        line.rectTransform.sizeDelta = new Vector2 (lineDist, 15f);

        talker.text = name;
        text.text = "";

        for (int i = 0; i < script.Length; i++)
        {
            text.text += script[i];
            if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
            {
                text.text = script;
                break;
            }
            yield return new WaitForSeconds(0.025f);
        }

        if (autoClickTime == -1f)
        {
            nextObj.SetActive(true);

            bool wait = true;
            while (wait)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(0))
                    wait = false;
                yield return null;
            }

            nextObj.SetActive(false);
            nextClickObj.SetActive(true);

            yield return new WaitForSeconds(0.25f);
        }
        else
        {
            yield return new WaitForSeconds(autoClickTime);
        }
       
        talkBox.SetActive(false);
    }
}
