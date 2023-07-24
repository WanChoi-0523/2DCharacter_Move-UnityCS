using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsuleCollider;

    [SerializeField] private int speed;
    public int jump_power;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();  
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    void Update() {
        //점프(Jump)
        if(Input.GetButtonDown("Jump")) {

            rigid.AddForce(Vector2.up * jump_power, ForceMode2D.Impulse);
        }

        //스프라이트 좌우 바꾸기(Sprite X reverse)
        if(Input.GetButton("Horizontal")) {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //걷기 & 달리기 애니메이션(Walk & Run Animation)
        if(Mathf.Abs(rigid.velocity.x) > 4) {
            //anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", true);
        }
            
        else if(Mathf.Abs(rigid.velocity.x) > 2) {
            anim.SetBool("isWalking", true);
            //anim.SetBool("isRunning", false);
        }
        else {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);
        }
    }

    void FixedUpdate() {
        //걷기 & 달리기(Walk & Run)
        if(Input.GetKey(KeyCode.LeftShift)) {
            rigid.velocity = new Vector2(Input.GetAxisRaw("Horizontal")*6, rigid.velocity.y);
        }
        else {
            rigid.velocity = new Vector2(Input.GetAxisRaw("Horizontal")*3, rigid.velocity.y);
            //anim.SetBool("isWalking", true);
        }
    }
}
