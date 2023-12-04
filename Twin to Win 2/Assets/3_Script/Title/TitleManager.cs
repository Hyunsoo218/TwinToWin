using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cinemachine;
using UnityEngine.SceneManagement;
public enum CharacterType 
{
    none, wtd, wgs, temp
}
public class TitleManager : MonoBehaviour
{
    [SerializeField] private Animator camAnimator;
    [SerializeField] private Animator uiMainAnimator;
    [SerializeField] private Animator uiSelectedAnimator;
    [SerializeField] private Animator uiInfoAnimator;
    [SerializeField] private Animator background;
    [SerializeField] private Image leftBtnImg;
    [SerializeField] private Image rigthBtnImg;
    [SerializeField] private Transform characterBtnParent;
    [SerializeField] private Transform characterLeft;
    [SerializeField] private Transform characterRigth;
    [SerializeField] private GameObject characterBtn;
    [SerializeField] private CharacterInfoUI infoUI;

    private CharacterType leftCharacter = CharacterType.none;
    private CharacterType rigthCharacter = CharacterType.none;
    private StateMachine uiAnimatorCmd = new StateMachine();
    private State uiMainState = new State("main");
    private State uiSelectedState = new State("selected");
    private State uiInfoState = new State("info");
    private bool canAction = true;
    private bool selectLeft = true;

    private void Awake()
    {
        uiMainState.onEnter = () => {
            StartCoroutine(DelayAction(0.5f));
            camAnimator.SetTrigger("reture");
            uiMainAnimator.SetTrigger("on");
			if (leftCharacter == CharacterType.none)
                leftBtnImg.enabled = true;
			else leftBtnImg.enabled = false;
            if (rigthCharacter == CharacterType.none)
                rigthBtnImg.enabled = true;
            else rigthBtnImg.enabled = false;
        };
        uiMainState.onExit = () => { 
            uiMainAnimator.SetTrigger("off"); 
        };
        uiSelectedState.onEnter = () => {
            StartCoroutine(DelayAction(0.5f));
            string camTrigger;
            string uiSelectedTrigger;
            CharacterType type;
            if (selectLeft){
                type = leftCharacter;
                camTrigger = "left";
                uiSelectedTrigger = "left on";
            }
            else{
                type = rigthCharacter;
                camTrigger = "rigth";
                uiSelectedTrigger = "rigth on";
            }
            if (uiAnimatorCmd.PrevState != uiInfoState)
                camAnimator.SetTrigger(camTrigger);
            uiSelectedAnimator.SetTrigger(uiSelectedTrigger);
            if (type != CharacterType.none)
                OnActiveCharacterInfoBtn(true);
        };
        uiSelectedState.onExit = () => {
            string uiSelectedTrigger = selectLeft ? "left off" : "rigth off";
            uiSelectedAnimator.SetTrigger(uiSelectedTrigger);
            OnActiveCharacterInfoBtn(false);
        };
        uiInfoState.onEnter = () => {
            StartCoroutine(DelayAction(0.5f));
            string uiInfoTrigger = selectLeft ? "left on" : "rigth on";
            uiInfoAnimator.SetTrigger(uiInfoTrigger);
            CharacterType type = selectLeft ? leftCharacter : rigthCharacter;
            CharacterInfo info = CharacterManager.instance.GetTypeToInfo(type);
            infoUI.Info = info;
        };
        uiInfoState.onExit = () => {
            string uiInfoTrigger = selectLeft ? "left off" : "rigth off";
            uiInfoAnimator.SetTrigger(uiInfoTrigger);
        };
    }
	private void Start()
	{
        SetGame();
        background.SetBool("Fade", true);
        StartCoroutine(DelayEvent(1f, () => { SetTitle(); }));
    }
	public void SelectLeft() 
    {
        if (!canAction) return;
        selectLeft = true;
        uiAnimatorCmd.ChangeState(uiSelectedState);
    }
    public void SelectRigth()
    {
        if (!canAction) return;
        selectLeft = false;
        uiAnimatorCmd.ChangeState(uiSelectedState);
    }
    public void CharacterInfo()
    {
        if (!canAction) return;
        uiAnimatorCmd.ChangeState(uiInfoState);
    }
    public void CharacterInfoToSelected()
    {
        if (!canAction) return;
        uiAnimatorCmd.ChangeState(uiSelectedState);
    }
    public void ReturnToDefault()
    {
        if (!canAction) return;
        uiAnimatorCmd.ChangeState(uiMainState);
    }
    public void GameStart() 
    {
        if (!canAction) return;
		if (leftCharacter == CharacterType.none || rigthCharacter == CharacterType.none)
		{
            print("¼±ÅÃ ÇÏ¼¼¿ê!");
            return;
		}
        StartCoroutine(DelayAction(5f));
        Player.instance.SetCharacterType(leftCharacter, rigthCharacter);
        UIManager.instance.SetSkillImage(leftCharacter, rigthCharacter);
        background.SetBool("Fade", false);
        StartCoroutine(DelayEvent(1f, () => { GameManager.instance.GameStart(); }));
    }
    public void SetGame()
    {
        for (int i = 0; i < characterBtnParent.childCount; i++)
            Destroy(characterBtnParent.GetChild(0).gameObject);
    }
    public void SetTitle()
    {
        leftCharacter = Player.instance.LeftType;
        rigthCharacter = Player.instance.RigthType;
        if (leftCharacter != CharacterType.none)
        {
            CharacterInfo info = CharacterManager.instance.GetTypeToInfo(leftCharacter);
            Instantiate(info.gameObject, characterLeft);
        }
        if (rigthCharacter != CharacterType.none)
        {
            CharacterInfo info = CharacterManager.instance.GetTypeToInfo(rigthCharacter);
            Instantiate(info.gameObject, characterRigth);
        }

        gameObject.SetActive(true);
        uiMainAnimator.gameObject.SetActive(true);
        uiSelectedAnimator.gameObject.SetActive(true);
        uiInfoAnimator.gameObject.SetActive(true); 
        uiAnimatorCmd.ChangeState(uiMainState);
		foreach (var type in Player.instance.MyCharacter)
		{
			if (CharacterManager.instance.ContainsKeyTypeToInfo(type))
            {
                GameObject obj = Instantiate(characterBtn, characterBtnParent);
                CharacterSelectBtn btn = obj.GetComponent<CharacterSelectBtn>();
                CharacterType temp = type;
                CharacterInfo info = CharacterManager.instance.GetTypeToInfo(type);

                btn.Image = info.Image;
                btn.Name = info.Name;
                btn.OnClick = () => {
                    if (!canAction) return;
                    StartCoroutine(DelayAction(0.5f));

                    if (selectLeft)
                    {
                        if (leftCharacter == temp) {
                            Destroy(characterLeft.GetChild(0).gameObject);
                            leftCharacter = CharacterType.none;
                            OnActiveCharacterInfoBtn(false);
                            return;
                        }
                        leftCharacter = temp;
                        if(characterLeft.childCount > 0)
                            Destroy(characterLeft.GetChild(0).gameObject);
                        if (rigthCharacter == temp) { 
                            Destroy(characterRigth.GetChild(0).gameObject);
                            rigthCharacter = CharacterType.none;
                        }
                        Instantiate(info.gameObject, characterLeft);
                        OnActiveCharacterInfoBtn(true);
                    }
                    else 
                    {
                        if (rigthCharacter == temp){
                            Destroy(characterRigth.GetChild(0).gameObject);
                            rigthCharacter = CharacterType.none;
                            OnActiveCharacterInfoBtn(false);
                            return;
                        }
                        rigthCharacter = temp;
                        if (characterRigth.childCount > 0)
                            Destroy(characterRigth.GetChild(0).gameObject);
                        if (leftCharacter == temp) { 
                            Destroy(characterLeft.GetChild(0).gameObject);
                            leftCharacter = CharacterType.none;
                        }
                        Instantiate(info.gameObject, characterRigth);
                        OnActiveCharacterInfoBtn(true);
                    }
                };
            }
        }
    }
    private void OnActiveCharacterInfoBtn(bool active) 
    {
        uiSelectedAnimator.SetBool("info", active);
    }
    private IEnumerator DelayAction(float time) 
    {
        canAction = false;
        yield return new WaitForSeconds(time);
        canAction = true;
    }
    private IEnumerator DelayEvent(float time, Action e)
    {
        yield return new WaitForSeconds(time);
        e?.Invoke();
    }
}
