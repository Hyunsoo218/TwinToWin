using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CodePreview : MonoBehaviour
{
    [Header("audio")]
    AudioSource run;

    [Header("data")]
    public int deatCnt = 0;
    public int hitCnt = 0;
    public int takeAttack = 0;
    public int soulMoney = 0;
    public int nowMoney = 0;
    public int jumpCnt = 0;
    public int attackCnt = 0;

    [Header("Movement")]
    public float moveSpeed = 3.3f;

    [Header("Jump")]
    public float baseJumpForce = 1.9f;
    public float maxJumpTime = 0.14f;
    public float jumpForcePerSecond = 0.32f;
    public bool isGrounded;
    public bool jumped;

    [Header("Attack")]
    private float attackCooldown;
    public float attackCooldownTime = 0.75f;
    public Transform attackPos;
    public Vector2 attackSize = new Vector2(1.9f, 1f);
    public Transform attackPosTop;
    public Vector2 attackSizeTop = new Vector2(1.5f, 1f);
    public Transform attackPosBottom;
    public Vector2 attackSizeBottom = new Vector2(0, 0);

    [Header("PlayerInfo")]
    public float healDuration;
    public int PlayerHp = 5;
    public int PlayerDamge = 5;
    public Transform PlayerPos;
    public Vector2 PlayerSize;
    public int Zio = 0;
    public int playerManaPoint = 0;

    private float xDirection;
    private bool isJumping;
    private float jumpTime;
    private Rigidbody2D rigidbody2d;
    private bool isInvincible = false;
    private bool isMove = false;
    public float knockbackForce = 5f;
    public Animator animator;
    private bool isAttacking = true;
    public GameObject[] mobPrefabs;
    private bool viewMap;

    private StateMachine sm;
    private State idleState;
    private State moveState;
    private State jumpState;
    private State damageState;
    private State dieState;
    private State attackState;
    private State attackTopState;
    private State attackBottomState;
    private State chairSate;
    private bool canMove = true;
    private bool canJump = true;
    private bool canDamage = true;
    private bool canAttack = true;
    private bool isDie = false;
    private bool lookLefr = true;

	public void OnMove(InputAction.CallbackContext context)
	{
        if (!canMove || isDie) return;
		if (context.phase == InputActionPhase.Started) // 화살표 클릭
		{

		}
	}
    public void OnJump(InputAction.CallbackContext context) 
    {
        if (!canJump || isDie) return;
        if (context.phase == InputActionPhase.Started) // Z 클릭
        {

        }
    }
    public void OnAttack(InputAction.CallbackContext context) 
    {
        if (!canAttack || isDie) return;
        if (context.phase == InputActionPhase.Started) // X 클릭
        {

        }
    }
    public void OnDamage()
    {
        if (!canJump || isDie) return;

    }
    public void OnDie()
    {

    }



    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        run = GetComponent<AudioSource>();
    }
    void Update()
    {
        // Physics2D.IgnoreLayerCollision(6, 7);
        TakeDamage();
        PlayerMove();
        playerDead();
        chairSave();
        dataSave();
        //미니맵
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            animator.SetTrigger("view_map");
            viewMap = true;
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            animator.SetTrigger("off_map");
            viewMap = false;
        }

        if (!viewMap)
        {
            //플레이어 회복 Z키 사용
            if (Input.GetKeyDown(KeyCode.Z))
            {
                healDuration = 0f;
            }

            if (Input.GetKey(KeyCode.Z) && PlayerHp < 5 && playerManaPoint > 32)
            {
                healDuration += Time.deltaTime;

                if (healDuration >= 2f)
                {
                    PlayerHp += 1;
                    playerManaPoint -= 33;
                    healDuration = 0f;
                }
            }
            else
            {
                healDuration = 0f; // Z 키를 떼면 healDuration을 초기화합니다.
            }



            //플레이어 점프
            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {

                isJumping = true;
                jumpTime = 0f;
                animator.SetTrigger("jump");
                jumpCnt++;

            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                isJumping = false;

                animator.SetTrigger("jump_end");

            }

            isGrounded = Physics2D.CircleCast(transform.position, 0.3f, Vector3.down, 0.7f, LayerMask.GetMask("Ground"));
            jumped = Physics2D.CircleCast(transform.position, 0.3f, Vector3.down, 1.3f, LayerMask.GetMask("Ground"));


            //플레이어 공격
            if (Input.GetKeyDown(KeyCode.X) && !Input.GetKey(KeyCode.UpArrow) && isAttacking && !Input.GetKey(KeyCode.DownArrow))
            {
                Attack();
            }

            if (Input.GetKey(KeyCode.UpArrow) && Input.GetKeyDown(KeyCode.X) && isAttacking)
            {
                AttackTop();
            }
            if (Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.X) && isAttacking && !isGrounded)
            {
                AttackBottom();
            }
        }
    }
    void PlayerMove()
    {
        if (isMove)
            return;


        if (viewMap)
        {
            xDirection = Input.GetAxis("Horizontal");
            rigidbody2d.velocity = new Vector2(xDirection * moveSpeed, rigidbody2d.velocity.y);
            animator.SetBool("move_map", true);


            if (Mathf.Abs(xDirection) > 0)
            {
                transform.localScale = new Vector3(xDirection > 0 ? -1 : 1, 1, 1);
                animator.SetBool("move_map", true);

            }
            else
                animator.SetBool("move_map", false);

        }
        else
        {
            xDirection = Input.GetAxis("Horizontal");
            rigidbody2d.velocity = new Vector2(xDirection * moveSpeed, rigidbody2d.velocity.y);
            animator.SetBool("is_run", true);


            if (Mathf.Abs(xDirection) > 0)
            {
                transform.localScale = new Vector3(xDirection > 0 ? -1 : 1, 1, 1);
                animator.SetBool("is_run", true);


            }
            else
                animator.SetBool("is_run", false);

        }
    }
    void FixedUpdate()
    {

        if (isJumping)
        {
            if (jumpTime < maxJumpTime)
            {
                rigidbody2d.AddForce(Vector2.up * (baseJumpForce + (jumpForcePerSecond * jumpTime)), ForceMode2D.Impulse);
                jumpTime += Time.fixedDeltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 0.7f);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.3f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackPos.position, attackSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(attackPosTop.position, attackSizeTop);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(attackPosBottom.position, attackSizeBottom);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(PlayerPos.position, PlayerSize);
    }
    private void TakeMana()
    {
        if (playerManaPoint < 100)
        {
            playerManaPoint += 12;
        }
        if (playerManaPoint >= 100)
        {
            playerManaPoint = 100;
        }
    }
    private void dataSave()
    {

        // DataManager.instance.nowPlayer.monsterCnt = 0;
        // DataManager.instance.nowPlayer.bossCnt = 0;
        //DataManager.instance.nowPlayer.playerOfHp = PlayerHp;
        //DataManager.instance.nowPlayer.playerMoney = nowMoney;
        //DataManager.instance.nowPlayer.attackCnt = attackCnt;
        //DataManager.instance.nowPlayer.deathCnt = deatCnt;
        //DataManager.instance.nowPlayer.hitCnt = hitCnt;
        //DataManager.instance.nowPlayer.jumpCnt = jumpCnt;
        //DataManager.instance.nowPlayer.moneyCnt = Zio;
        //DataManager.instance.nowPlayer.takeAttack = takeAttack;
        //DataManager.instance.nowPlayer.playerManaPoint = playerManaPoint;
        //DataManager.instance.nowPlayer.playerSoulMoney = soulMoney;
    }
    private void Attack()
    {
        Debug.Log("공격 진행");
        attackCnt++;
        animator.SetTrigger("is_attack");
        StartCoroutine(AttackCooldown());
        Collider2D[] colliders = Physics2D.OverlapBoxAll(attackPos.position, attackSize, 0);
        foreach (Collider2D collider in colliders)
        {

        }
    }
    private void AttackTop()
    {
        Debug.Log("상단공격 진행");
        attackCnt++;
        animator.SetTrigger("is_attack_top");
        StartCoroutine(AttackCooldown());
        Collider2D[] colliders = Physics2D.OverlapBoxAll(attackPosTop.position, attackSizeTop, 0);
        foreach (Collider2D collider in colliders)
        {
            

        }
    }
    private void AttackBottom()
    {
        Debug.Log("하단공격 진행");
        attackCnt++;
        animator.SetTrigger("is_attack_bottom");
        StartCoroutine(AttackCooldown());
        Collider2D[] colliders = Physics2D.OverlapBoxAll(attackPosBottom.position, attackSizeBottom, 0);
        foreach (Collider2D collider in colliders)
        {

        }
    }
    // hp 0이하 되면 좌표 생성
    private void playerDead()
    {
        if (PlayerHp <= 0)
        {

            float playerX = transform.position.x;
            float playerY = transform.position.y;
            soulMoney = nowMoney;
            nowMoney = 0;

            Debug.Log("플레이어가 죽었습니다.");
            /*
             플레이어 초기 위치로 이동 코드 작성 
             */
            /*
             mob(float x, float y)로 전송
             */
        }
    }

    private void TakeDamage()
    {
        if (isInvincible)
            return;


        Collider2D[] colliders = Physics2D.OverlapBoxAll(PlayerPos.position, PlayerSize, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.tag == "Mob")
            {
                Debug.Log("hit");
                takeAttack++;
                PlayerHp -= 1;
                StartCoroutine(InvincibilityFrames());



                Vector2 knockbackDirection = (transform.position - collider.transform.position).normalized;
                if (!isGrounded)
                {
                    knockbackDirection.y = 1f;
                    rigidbody2d.velocity = knockbackDirection * (knockbackForce + 5f);
                }
                else
                {
                    knockbackForce = 12f;
                    knockbackDirection.y = 0.3f;
                    rigidbody2d.velocity = knockbackDirection * knockbackForce;
                    knockbackForce = 5f;
                }
            }
        }
    }
    public void chairSave()
    {
        // chair의 boxCollider is trigger 체크하고 사용
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(PlayerPos.position, PlayerSize, 0);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("chair"))
                {
                    PlayerHp = 5;
                    break;
                }
            }
        }
    }
    // zio 충돌 시 삭제 후 zio증가
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Zio"))
        {
            Destroy(collision.gameObject); // 충돌한 "zio" 오브젝트 삭제
            Zio += 1;
            nowMoney = Zio;
        }
    }
    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        isMove = true;

        // Additional handling for invincibility state (e.g., changing animation)
        yield return new WaitForSeconds(0.25f);  // 넉백 시 move 돌아가면 y축으로 쳐밀려서 잠시 제어
        isMove = false;
        yield return new WaitForSeconds(1.5f);    // 피격 시 무적 판정 1초 

        // Additional handling after invincibility state ends (e.g., restoring player's original animation)

        isInvincible = false;
    }
    private IEnumerator AttackCooldown()
    {
        isAttacking = false;

        yield return new WaitForSeconds(attackCooldownTime); // 1초 대기

        isAttacking = true;
    }
}