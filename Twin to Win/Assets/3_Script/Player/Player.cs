using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player instance;
	public PlayerbleCharacter cCurrentCharacter;
	[SerializeField] private PlayerbleCharacter cGreatSword;
	[SerializeField] private PlayerbleCharacter cTwinSword;
	private void Awake()
	{
		instance = this;
		cCurrentCharacter = cTwinSword;
	}
	public void ConvertCharacter()
	{
		cCurrentCharacter = (cCurrentCharacter == cTwinSword) ? cGreatSword : cTwinSword;
	}
}
