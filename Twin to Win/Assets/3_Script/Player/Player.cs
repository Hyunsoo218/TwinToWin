using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player instance;
	public PlayerbleCharacter cCurrentCharacter;
	[SerializeField] private PlayerbleCharacter cGreatSword;
	[SerializeField] private PlayerbleCharacter cTwinSword;

    private Transform tCurrentTrans;
    private bool isHoldingState = false;
	private void Awake()
	{
		instance = this;
		cCurrentCharacter = cTwinSword.gameObject.activeSelf ? cTwinSword : cGreatSword;
    }
    public void ConvertCharacter()
	{
        InActiveCurrentCharacter();
        cCurrentCharacter = (cCurrentCharacter == cTwinSword) ? cGreatSword : cTwinSword;
        ActiveNextCharacter();

        StartCoroutine(cCurrentCharacter.StartTagCoolDown());
    }

    private void InActiveCurrentCharacter()
	{
        tCurrentTrans = cCurrentCharacter.transform;
        if (cCurrentCharacter.eMouseState == mouseState.Hold)
        {
            isHoldingState = true;
        }
        cCurrentCharacter.gameObject.SetActive(false);
    }
	private void ActiveNextCharacter()
    {
        cCurrentCharacter.transform.position = tCurrentTrans.position;
        if (isHoldingState == true)
        {
            cCurrentCharacter.eMouseState = mouseState.Hold;
            isHoldingState = false;
        }
        cCurrentCharacter.gameObject.SetActive(true);
    }
	public string GetCurrentCharacterStateName()
	{
		return cCurrentCharacter.GetCurrentStateName();
	}

    public PlayerbleCharacter GetTwinSword()
    {
        return cTwinSword;
    }

    public PlayerbleCharacter GetGreatSword()
    {
        return cGreatSword;
    }
}
