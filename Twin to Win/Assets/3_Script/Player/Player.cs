using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player instance;
	public PlayerbleCharacter cCurrentCharacter;
	public PlayerbleCharacter cWaitingCharacter;
	[SerializeField] private PlayerbleCharacter cGreatSword;
	[SerializeField] private PlayerbleCharacter cTwinSword;
	private void Awake()
	{
		instance = this;
		cCurrentCharacter = cTwinSword.gameObject.activeSelf ? cTwinSword : cGreatSword;
		cWaitingCharacter = cGreatSword.gameObject.activeSelf ? cTwinSword : cGreatSword;
		cWaitingCharacter.GetComponent<PlayerbleCharacter>().enabled = false;
    }
	public void ConvertCharacter()
	{
		Transform tmpTransform;
		bool isHoldingState = false;

        cWaitingCharacter = cCurrentCharacter;
        tmpTransform = cWaitingCharacter.transform;
        if (cCurrentCharacter.eMouseState == mouseState.Hold)
        {
            isHoldingState = true;
        }
		cCurrentCharacter.ResetAllTimer();
        cCurrentCharacter.GetComponent<PlayerbleCharacter>().enabled = false;
        cCurrentCharacter.gameObject.SetActive(false);


		cCurrentCharacter = (cCurrentCharacter == cTwinSword) ? cGreatSword : cTwinSword;

		cCurrentCharacter.transform.position = tmpTransform.position;
		if (isHoldingState == true)
		{
            cCurrentCharacter.eMouseState = mouseState.Hold;
        }
        cCurrentCharacter.GetComponent<PlayerbleCharacter>().enabled = true;
        cCurrentCharacter.gameObject.SetActive(true);

        StartCoroutine(cCurrentCharacter.StartTagCoolDown());
    }
	
	public string GetCurrentCharacterStateName()
	{
		return cCurrentCharacter.GetCurrentStateName();
	}
}
