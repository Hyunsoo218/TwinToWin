using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateUI : MonoBehaviour
{
	[SerializeField] private List<Animator> arrWTDSkills; 
	[SerializeField] private List<Animator> arrWGSSkills; 
	[SerializeField] private Animator cConvertUIAnimator;
    private Animator cAnimator;

	private void Awake()
	{
		cAnimator = GetComponent<Animator>();
	}
	public void Convert()
	{
		cConvertUIAnimator.SetTrigger("On");
		if (true) // 가능한지 확인후 교체
		{
			cAnimator.SetTrigger("Convert");
		}
	}
	public void OnButton(KeyCode key) 
	{
		PlayerbleCharacter cWTD = Player.instance.GetTwinSword();
		PlayerbleCharacter cCur = Player.instance.cCurrentCharacter;

		List<Animator> animators = (cCur == cWTD) ? arrWTDSkills : arrWGSSkills;

		Animator animator = null;
		switch (key)
		{
			case KeyCode.Q: animator = animators[0]; break; 
			case KeyCode.W: animator = animators[1]; break;
			case KeyCode.E: animator = animators[2]; break;
			case KeyCode.R: animator = animators[3]; break;
		}
		animator.SetTrigger("On");
	}
}