using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using static UnityEditor.Progress;

public class TalkUI : MonoBehaviour
{
    [SerializeField] private GameObject talkBox;
    [SerializeField] private TextMeshProUGUI talker;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image line;
    [SerializeField] private GameObject nextObj;
    [SerializeField] private GameObject nextClickObj;
    [SerializeField] private List<ChoiceBtnInfo> choiceBtnInfos;
    [SerializeField] private Sprite normalBtnImg;
    [SerializeField] private Sprite selectedBtnImg;
    [SerializeField] private RectTransform cursor;

    private int playersChoice;
    private bool waitChoice;
    private void Awake()
    {
        talkBox.SetActive(false);
        cursor.gameObject.SetActive(false);
        foreach (var item in choiceBtnInfos)
        {
            item.image.gameObject.SetActive(false);
        }
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
            if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && autoClickTime == -1f)
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
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
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
    public IEnumerator Choice(string name, string script, List<string> options) 
    {
        talkBox.SetActive(true);
        nextObj.SetActive(false);
        nextClickObj.SetActive(false);

        text.text = script;
        float lineDist;
        if (text.preferredWidth <= 200f) lineDist = 300f;
        else if (text.preferredWidth >= 800f) lineDist = 900f;
        else lineDist = text.preferredWidth + 100f;
        line.rectTransform.sizeDelta = new Vector2(lineDist, 15f);

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

		for (int i = 0; i < options.Count; i++)
		{
            choiceBtnInfos[i].text.text = options[i];
            choiceBtnInfos[i].image.gameObject.SetActive(true);
        }
        waitChoice = true;
        SelectBtn(0);
        cursor.gameObject.SetActive(true);

        while (waitChoice)
		{
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Choice(playersChoice);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow)) 
            {
                playersChoice = playersChoice + 1 == options.Count ? 0 : playersChoice + 1;
                SelectBtn(playersChoice);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                playersChoice = playersChoice - 1 == -1 ? options.Count - 1 : playersChoice - 1;
                SelectBtn(playersChoice);
            }
            yield return null;
		}
		foreach (var item in choiceBtnInfos)
		{
            item.image.gameObject.SetActive(false);
        }
        talkBox.SetActive(false);
    }
    public void Choice(int number)
    {
        playersChoice = number;
        waitChoice = false;
        cursor.gameObject.SetActive(false);
    }
    public int GetPlayersChoice()
    {
        return playersChoice;
    }
    public void SelectBtn(int num) 
    {
        float addYPos = num * 80f;
        float cursorYPos = -210f + addYPos;
        cursor.anchoredPosition = new Vector2(390f, cursorYPos);
        playersChoice = num;
        foreach (var item in choiceBtnInfos) 
        {
            item.image.sprite = normalBtnImg;
        }
        choiceBtnInfos[num].image.sprite = selectedBtnImg;
    }
}
[Serializable]
public struct ChoiceBtnInfo 
{
    public Image image;
    public TextMeshProUGUI text;
}