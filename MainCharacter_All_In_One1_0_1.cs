using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    CapsuleCollider2D capsuleCollider;

    [SerializeField] private int speed;
    private string curState;
    private int attackStack;
    private int walkSpeed;
    private int runSpeed;
    public int jump_power;
    private bool isFall;
    private bool isJump;
    private bool isAttack;
    private bool isAirAttack;

    const string IDLE = "Crimson_Idle";
    const string WALK = "Crimson_Walk";
    const string RUN = "Crimson_Run";
    const string JUMP_UP = "Crimson_Jump_Up";
    const string JUMP_DOWN = "Crimson_Jump_Down";
    const string ATTACK1 = "Crimson_Attack1";
    const string ATTACK2 = "Crimson_Attack2";
    const string ATTACK3 = "Crimson_Attack3";
    const string ATTACK4 = "Crimson_Attack4";
    const string AIRATK1 = "Crimson_AirAttack1";
    const string AIRATK2 = "Crimson_AirAttack2";
    const string AIRATK3 = "Crimson_AirAttack3";

    public float shortDelay;
    public float longDelay;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();  
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        attackStack = 0;
        walkSpeed = 3;
        runSpeed = 6;

        isJump = false;
        isAttack = false;
        isAirAttack = false;
    }

    void Update() {

        //일반 공격(Attack)
        if(Input.GetKeyDown(KeyCode.J)) {
            if(isJump == true)
                AirAttack();
            else if(isIdleState()) {
                if(attackStack == 4) 
                    attackStack %= 4;
                isAttack = true;
                Attack();
            }
        }

        //점프(Jump)
        if(Input.GetKey(KeyCode.W) && isIdleState()) {
            Jump();
            isJump = true;
        }

        //스프라이트 좌우 바꾸기(Sprite X reverse)
        if(Input.GetButton("Horizontal") && !isAttackState()) {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        if(isIdleState()) {
            //걷기 & 달리기 애니메이션(Walk & Run animatoration)
            if(Mathf.Abs(rigid.velocity.x) > 4) {
                ChangeAnimation(RUN);
            }
            
            else if(Mathf.Abs(rigid.velocity.x) > 2) {
                ChangeAnimation(WALK);
            }
            
            else {
                ChangeAnimation(IDLE);
            }
        }
    }

    void FixedUpdate() {
        //걷기 & 달리기(Walk & Run)
        if(!isAttackState()) {
            if(Input.GetKey(KeyCode.LeftShift)) {
                rigid.velocity = new Vector2(Input.GetAxisRaw("Horizontal")*runSpeed, rigid.velocity.y);
            }
            else {
                rigid.velocity = new Vector2(Input.GetAxisRaw("Horizontal")*walkSpeed, rigid.velocity.y);
            }
        }
        
        //바닥에 닿았는지 확인(Check wether character is on the floor)
        if(rigid.velocity.y < 0) {
            isFall = true;

            Vector2 downVec = new Vector2(rigid.position.x, rigid.position.y-1);
            Debug.DrawRay(downVec, Vector3.down, new Color(1, 0, 0));
            RaycastHit2D RayHit = Physics2D.Raycast(downVec, Vector3.down, 1, LayerMask.GetMask("Floor"));


            if(RayHit.collider != null) {
                if(RayHit.distance < 0.55f) {
                    isJump = false;
                    isFall = false;
                    if(isAirAttack == true) {
                        ChangeAnimation(AIRATK3);
                        //rigid.velocity = new Vector2(rigid.velocity.x, -5);
                        Invoke("AttackComplete", 0.25f);
                    }
                    
                    //isAirAttack = false;
                }    
            }
            else {
                if(isAirAttack) {
                ChangeAnimation(AIRATK2);
                //rigid.velocity = new Vector2(rigid.velocity.x, -15);
                }
                else {
                    ChangeAnimation(JUMP_DOWN);
                }
            }      
        }
        if(Mathf.Abs(rigid.velocity.x) > 2)
            attackStack = 0;
    }

    //점프(Jump)
    private void Jump() {
        rigid.velocity = new Vector2(rigid.velocity.x, jump_power);
        ChangeAnimation(JUMP_UP);
    }

    //공격(Attack)
    private void Attack() {
        switch(attackStack) {
            case 0 :
                ChangeAnimation(ATTACK1);
                break;
            case 1 :
                ChangeAnimation(ATTACK2);
                break;
            case 2 :
                ChangeAnimation(ATTACK1);
                break;
            case 3 :
                ChangeAnimation(ATTACK4);
                break;
        }
        //공격하면 공격하는 방향으로 조금씩 이동(If you attack, move little by little in the direction of attack)
        if(spriteRenderer.flipX)
            rigid.velocity = new Vector2(-2, rigid.velocity.y);
        else
            rigid.velocity = new Vector2(2, rigid.velocity.y);

        
        if(attackStack == 1)
            //Invoke("AttackComplete", 0.238f);
            //Invoke("AttackComplete", 0.22f);
            Invoke("AttackComplete", shortDelay);
        else
            //Invoke("AttackComplete", 0.357f);
            //Invoke("AttackComplete", 0.34f);
            Invoke("AttackComplete", longDelay);
        attackStack++;
    }

    //공중 공격(Air Attack)
    private void AirAttack() {
        isAirAttack = true;
        rigid.velocity = new Vector2(0, 13);
        ChangeAnimation(AIRATK1);
        Invoke("AirFall", 0.3f);
        //Debug.Log("AirAttack");
    }

    //공중 공격을 실행하면 빠르게 낙하(Drop quickly when you run 'AirAttack')
    void AirFall() {
        rigid.velocity = new Vector2(rigid.velocity.x, -20);
    }

    //공격 끝(Attack Complete)
    private void AttackComplete() {
        isAttack = false;
        isAirAttack = false;
    }
    
    //가만히 있는 상태인지 체크(Check wether state is 'idle')
    public bool isIdleState() {
        return !(isJump || isAttack || isFall || isAirAttack);
    }


    //공격 상태인지 체크(Check wether state is 'attack')
    public bool isAttackState() {
        return isAttack || isAirAttack;
    }

    //움직임에 따라 애니메이션 전환(Change animation based on current state)
    void ChangeAnimation(string newState) {
        if(curState == newState) {
            return;
        }
        animator.Play(newState);
    }
    
}
