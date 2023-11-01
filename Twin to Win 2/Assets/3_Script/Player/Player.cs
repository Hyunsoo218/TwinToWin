using System;
using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player instance;
    [SerializeField] private GameObject objWTD;
    [SerializeField] private GameObject objWGS;
    
    public PlayerbleCharacter CurrentCharacter { get; private set; }
    public PlayerbleCharacter GreatSword { get; private set; }
    public PlayerbleCharacter TwinSword { get; private set; }

    private Coroutine movementCo;
    private Coroutine autoAttackCo;
    private PlayerInput playerInput;

    private (float max, float current) healthPoint = (10000f, 10000f);
    public float MaxHealthPoint => healthPoint.max;
    public float CurrentHealthPoint { 
        get => healthPoint.current;
        set { 
            healthPoint.current = value;
            UIManager.instance.SetPlayerHealthPoint();
            if (healthPoint.current <= 0)
			{
                CurrentCharacter.Die();
                EnablePlayerInput(false);
                GameManager.instance.GameLose();
            }
        }
    }

    [SerializeField] private (float max, float current) stamina = (10f, 10f);
    public float MaxStamina => stamina.max;
    public float CurrentStamina => stamina.current;

    private (float max, float current) tagTime = (3f, 3f);
    public float MaxTagTime => tagTime.max;
    public float CurrentTagTime => tagTime.current;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        playerInput = GetComponent<PlayerInput>();
    }
    
	private void Update()
	{
		if (GameManager.instance.gameStage == GameStage.Game)
		{
            RecoverStamina();
        }
	}
    public void SetGame()
    {
        UIManager.instance.SetPlayerHealthPoint();

        GameObject wtd = Instantiate(objWTD, new Vector3(4f, 0.5f, 4f), Quaternion.Euler(0, 45f, 0));
        GameObject wgs = Instantiate(objWGS, new Vector3(4f, 0.5f, 4f), Quaternion.Euler(0, 45f, 0));

        TwinSword = wtd.GetComponent<PlayerbleCharacter>();
        GreatSword = wgs.GetComponent<PlayerbleCharacter>();

        GreatSword.gameObject.SetActive(false);

        CurrentCharacter = TwinSword.gameObject.activeSelf ? TwinSword : GreatSword;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            movementCo = StartCoroutine(Movement());
        else if (context.phase == InputActionPhase.Canceled)
            StopCoroutine(movementCo);
    }
    private IEnumerator Movement()
    {
        CurrentCharacter.MoveStart(GetRayPos());
        while (true)
        {
            CurrentCharacter.Move(GetRayPos());
            yield return null;
        }
    }

    public void OnQSkill(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Started)
        {
            CurrentCharacter.OnSkill(SkillType.Q, GetRayPos());
        }
    }
    public void OnWSkill(InputAction.CallbackContext context) 
    { 
        if(context.phase == InputActionPhase.Started)
        {
            CurrentCharacter.OnSkill(SkillType.W, GetRayPos());
        }
    }
    public void OnESkill(InputAction.CallbackContext context) 
    { 
        if(context.phase == InputActionPhase.Started)
        {
            CurrentCharacter.OnSkill(SkillType.E, GetRayPos());
        }
    }
    public void OnRSkill(InputAction.CallbackContext context) 
    { 
        if(context.phase == InputActionPhase.Started)
        {
            CurrentCharacter.OnSkill(SkillType.R, GetRayPos());
        }
    }

    public void OnTag(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Started) 
        {
            DoTag();
        }
    }
    public void DoTag()
    {
        UIManager.instance.ConvertPlayer();

        PlayerbleCharacter currentCharacter = CurrentCharacter;
        PlayerbleCharacter nextCharacter = (CurrentCharacter == TwinSword) ? GreatSword : TwinSword;

        nextCharacter.transform.position = currentCharacter.transform.position;
        nextCharacter.transform.eulerAngles = currentCharacter.transform.eulerAngles;

        nextCharacter.gameObject.SetActive(true);
        currentCharacter.gameObject.SetActive(false);

        CurrentCharacter = nextCharacter;

        CameraManager.instance.ResetCamera();
    }

    public void OnDodge(InputAction.CallbackContext context) 
    {
        float usingStamina = 3f;

        if (context.phase == InputActionPhase.Started && UseStamina(usingStamina)) 
        {
            if (!CurrentCharacter.Dodge(GetRayPos()))  
                stamina.current += usingStamina; 
        }
    }

    public void OnNormalAttack(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Started)
            autoAttackCo = StartCoroutine(AutoAttack());
        else if (context.phase == InputActionPhase.Canceled)
            StopCoroutine(autoAttackCo);
    }
    private IEnumerator AutoAttack()
    {
        while (true)
        {
            CurrentCharacter.Attack(GetRayPos());
            yield return null;
        }
    }


    private Vector3 GetRayPos()
    {
        Ray ray;
        NavMeshHit navMeshHit;
        RaycastHit hit;
        Vector3 pos = Vector3.zero;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (NavMesh.SamplePosition(hit.point, out navMeshHit, 1f, NavMesh.AllAreas))
            {
                pos = navMeshHit.position;
            }
        }
        return pos;
    }

    public void EnablePlayerInput(bool active)
    {
        playerInput.enabled = active;
		if (GameManager.instance.gameStage == GameStage.Game)
            CurrentCharacter.ReturnToIdle(); 
    }
    public string GetCurrentCharacterStateName()
	{
		return CurrentCharacter.GetCurrentStateName();
	}
    public SkillTimeInfo GetTagTimer() => new SkillTimeInfo(MaxTagTime, CurrentTagTime); 

    private bool UseStamina(float usingAmount) 
    {
        if (stamina.current >= usingAmount)
        {
            stamina.current -= usingAmount;
            return true;
        }
        return false;
    }

    private void RecoverStamina() 
    {
        if (stamina.current < stamina.max)
        {
            stamina.current += Time.deltaTime;
			if (stamina.current > stamina.max)
                stamina.current = stamina.max;
		}
    }
}