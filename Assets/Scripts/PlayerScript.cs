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
    public bool canAttack = false;
    public int hp = 100;
    public int ap = 100;
    public int basicDamage = 10;
    public bool hasPieceOfGold = false;
    public string strAction = "isWalking";
    public float actionSpeed = 0f;
    public float currentCmbLevel = 0f;
    private float moviment = 0f;
    private Animator anim;
    private SpriteRenderer sprRdr;
    //private GameObject currentEnemy = null;
    private Rigidbody2D rgb2d;
    // Update is called once per frame
    void Update()
    {
        // Desabilita o jogador após a morte
        anim = GetComponent<Animator>();
        sprRdr = GetComponent<SpriteRenderer>();
        moviment = Input.GetAxis("Horizontal");
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
        if ((Input.GetKeyDown("up") || Input.GetKeyDown(KeyCode.W)) && !anim.GetBool("isJumping"))
        {
            //transform.Translate(0, jumpingForce, 0); Pulo sem física (fazer transição)
            // Pulo com física
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, jumpingForce);
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
            if (canAttack)
            {
                if (currentCmbLevel >= 3f)
                {
                    currentCmbLevel = 0f;
                }
                else 
                {
                    currentCmbLevel += 1f;
                }
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
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey("left") || Input.GetKeyDown("right")) {
            transform.Translate(actionSpeed, 0, 0);
            anim.SetBool(strAction, true);
        }/* else if (actionSpeed != 0) { 
            Debug.Log("Running speed: " + actionSpeed.ToString());
            // Se o usuário não estiver pressionado nenhum botão, mas estiver
            // ainda em movimento, parte para o efeito de desaceleração.
            if (actionSpeed > 0) {
                actionSpeed -= 0.005f;
            } else if (actionSpeed < 0) {
                actionSpeed += 0.005f;
            }
            transform.Translate(actionSpeed, 0, 0);
        }*/
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
        anim.SetBool("isJumping", isJumping);
        anim.SetFloat("comboLevel", currentCmbLevel);
        anim.SetInteger("hp", hp);
        verifyIfGameIsOver();
    }
    private void verifyIfGameIsOver() {
        if (hp <= 0) {
            Time.timeScale = 0;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("BreakableItem") || collision.gameObject.CompareTag("Enemy")) {
            canAttack = true;
        }
    }
    private void OnCollisionStay2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Platform"))
            isJumping = false;
        if ((collision.gameObject.CompareTag("BreakableItem") || collision.gameObject.CompareTag("Enemy")) && canAttack) {
            canAttack = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Platform"))
            isJumping = true;
		if (collision.gameObject.CompareTag("BreakableItem") || collision.gameObject.CompareTag("Enemy"))
            canAttack = false;
    }
    // Função para uso em outros scripts para identificar se o player está atacando
    public bool IsAttacking() {
        return currentCmbLevel >= 1f && canAttack;
    }

    public int GetReducedHp(int currentHp) {
        Debug.Log("Current GameObject HP: " + currentHp.ToString());
        // Personagem com espada causa maior dano
        if (anim.GetBool("hasGotSword")) {
            basicDamage += 40;
            // Personagem em pique causa mais dano ainda...
            if (strAction == "isRunning") {
                basicDamage += 10;
                actionSpeed = 0f;
            }
        }
        if (currentHp > 0) {
            currentHp -= basicDamage * (int)Math.Ceiling(currentCmbLevel);
        }
        Debug.Log("New GameObject HP: " + currentHp.ToString());
        return currentHp;
    }
}
