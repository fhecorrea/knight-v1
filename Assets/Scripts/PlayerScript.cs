using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerScript : MonoBehaviour
{
    public float walkingSpeed;
    public float runningSpeed;
    public float jumpingForce;
    public bool isJumping = false;
    public bool inContactWith = false;
    public int hp = 100;
    public int ap = 100;
    public int basicDamage = 10;
    public bool hasPieceOfGold = false;
    public string strAction = "isWalking";
    public float actionSpeed = 0f;
    public float currentCmbLevel = 0f;
    private Animator anim;
    private SpriteRenderer sprRdr;
    private GameObject currentEnemy = null;
    // Update is called once per frame
    void Update()
    {
        anim = GetComponent<Animator>();
        sprRdr = GetComponent<SpriteRenderer>();
        float moviment = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown("left")) {
            // Se o personagem já estiver caminhando, ele corre. Caso contrário,
            // ele apenas caminha.
            if (anim.GetBool("isWalking") && moviment < 0) {
                strAction = "isRunning";
                actionSpeed = -runningSpeed;
            } else {
                strAction = "isWalking";
                actionSpeed = -walkingSpeed;
            }
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown("right")) {
            // O mesmo que o if anterior, só que no sentido contrário...
            if (anim.GetBool("isWalking") && moviment > 0) {
                strAction = "isRunning";
                actionSpeed = runningSpeed;
            } else {
                strAction = "isWalking";
                actionSpeed = walkingSpeed;
            }
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey("left") || Input.GetKeyDown("right")) {
            transform.Translate(actionSpeed, 0, 0);
            anim.SetBool(strAction, true);
        }
        // Se o personagem nào estiver realizado nenhum movimento, 
        // para as animações que ele estiver executando.
        if (moviment == 0)
        { 
            if (anim.GetBool("isWalking"))
                anim.SetBool("isWalking", false);
            if (anim.GetBool("isRunning"))
                anim.SetBool("isRunning", false);
        } 
        else
        {
            sprRdr.flipX = (moviment < 0);
        }
        if ((Input.GetKeyDown("up") || Input.GetKeyDown(KeyCode.W)) && !anim.GetBool("isJumping"))
        {
            transform.Translate(moviment / 2, jumpingForce, 0);
            //anim.Play("Jumping");
        }
        // Se o usuário tiver 'ativado' o personagem
        if (Input.GetKeyDown(KeyCode.X) && !anim.GetBool("isWalking") && !anim.GetBool("isRunning"))
        {
            anim.SetBool("hasGotSword", !anim.GetBool("hasGotSword"));
        }
        /**
            Rigidbody.distance ou OnTrigger
         */
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentEnemy != null)
            {
                if (currentCmbLevel >= 3f)
                {
                    currentCmbLevel = 0f;
                }
                else 
                {
                    currentCmbLevel += 1f;
                }
                // Personagem com espada causa maior dano
                if (anim.GetBool("hasGotSword")) {
                    basicDamage += 40;
                    // Personagem em pique causa mais dano ainda...
                    if (strAction == "isRunning") {
                        basicDamage += 10;
                    }
                }
                //Debug.Log("HP bef. atk: " + currentEnemy.GetComponent<BreakableItemScript>().hp);
                if (currentEnemy.GetComponent<BreakableItemScript>().hp > 0) {
                    currentEnemy.GetComponent<BreakableItemScript>().hp -= basicDamage * (int)Math.Ceiling(currentCmbLevel);
                }
                //Debug.Log("HP aft. atk: " + currentEnemy.GetComponent<BreakableItemScript>().hp);
                //Debug.Log("Coeficient: " + (int)Math.Ceiling(currentCmbLevel));
            }
            else
            {
                currentCmbLevel = 1f;
            }
        }
        else {
            // Compete com o próprio "clock" do Unity
            if (currentCmbLevel > 0)
                currentCmbLevel -= 0.1f;
        }
        anim.SetBool("isJumping", isJumping);
        anim.SetFloat("comboLevel", currentCmbLevel);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("BreakableItem"))
            currentEnemy = collision.gameObject;
    }
    void OnCollisionStay2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Platform"))
            isJumping = false;
        if (collision.gameObject.CompareTag("BreakableItem")) {
            currentEnemy = collision.gameObject;
        }
    }
    void OnCollisionExit2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Platform"))
            isJumping = true;
		if (collision.gameObject.CompareTag("BreakableItem"))
            currentEnemy = null;
    }
}
