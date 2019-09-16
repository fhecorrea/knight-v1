using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RectTransform;
using UnityEngine.UI;
using System;

public class PlayerScript : MonoBehaviour
{
    /** Variáveis públicas */
    public float walkingSpeed = 0.5f;
    public float runningSpeed = 1.5f;
    public float jumpingForce;
    public bool isJumping = false;
    public bool canAttack = false;
    public bool hasPieceOfGold = false;
    public bool isDead = false;
    public int hp = 100;
    public int ap = 100;
    public int basicDamage = 5;
    public Image healthBar;
    /** Variáveis de uso interno na classe */
    private bool isWalking = false;
    private bool isRunning = false;
    private string strAction = "isWalking";
    private float actionSpeed = 0f;
    private float currentCmbLevel = 0f;
    private float moviment = 0f;
    private Animator anim;
    private SpriteRenderer sprRdr;
    private GameObject currentTarget;
    private const string LEFT = "left";
    private const string RIGHT = "right";
    private Rigidbody2D rgb2d;
    // Update is called once per frame
    private void Start()
    {
        // Inicializa as variáveis...
        anim = GetComponent<Animator>();
        sprRdr = GetComponent<SpriteRenderer>();
        rgb2d = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        // Desabilita o jogador após a morte
        if (isDead) { return; }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(LEFT))
        {
            SpeedUp(LEFT);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(RIGHT))
        {
            SpeedUp(RIGHT);
        }
        MakePlayerMove();
        // Pulo 
        if ((Input.GetKeyDown("up") || Input.GetKeyDown(KeyCode.W)) && !isJumping)
        {
            //transform.Translate(0, jumpingForce, 0); Pulo sem física (fazer transição)
            // Pulo com física
            rgb2d.velocity = new Vector2(0f, jumpingForce);
        }
        // Se o usuário tiver 'ativado' o personagem
        if (Input.GetKeyDown(KeyCode.X) && !isWalking && !isRunning)
        {
            anim.SetBool("hasGotSword", !anim.GetBool("hasGotSword"));
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            IncreaseAttackLevel();
        }
        else
        {
            // Compete com o próprio "clock" do Unity
            if (currentCmbLevel > 0)
                currentCmbLevel -= 0.1f;
        }
        //VerifyIfGameIsOver();
    }

    private void LateUpdate()
    {
        anim.SetBool("isJumping", isJumping);
        anim.SetFloat("comboLevel", currentCmbLevel);
        anim.SetInteger("hp", hp);
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isRunning", isRunning);
        //anim.SetBool("isDead", isDead);
        if (!isDead)
            VerifyIfGameIsOver();
        // https://docs.unity3d.com/2018.3/Documentation/ScriptReference/RectTransform.html
        //healthBar.GetComponent<RectTransform>().width = hp >= 0 ? hp : 0;
        //Debug.Log(healthBar.rectTransform);//.SetWidth(hp >= 0 ? hp : 0);
        healthBar.rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, (float)hp);
    }

    private void IncreaseAttackLevel()
    {
        /** Controle de ataque do personagem */
        if (currentTarget) //(canAttack)
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

    private void MakePlayerMove()
    {
        //transform.Translate(actionSpeed, 0, 0);
        rgb2d.velocity = new Vector2(actionSpeed, rgb2d.velocity.y);
        // Se o personagem nào estiver realizado nenhum movimento, 
        // para as animações que ele estiver executando.
        moviment = Input.GetAxis("Horizontal");
        if (moviment == 0)
        {
            isWalking = isRunning = false;
            SpeedDown();
        }
        else
        {
            //Debug.Log(actionSpeed > 0);
            //Debug.Log(actionSpeed > walkingSpeed);
            if (actionSpeed != 0)
                isWalking = true;
            if (actionSpeed > 0 && actionSpeed >= runningSpeed || actionSpeed < 0 && actionSpeed <= runningSpeed)
                isRunning = true;
            sprRdr.flipX = (moviment < 0);
        }
    }
    private void SpeedUp(string direction)
    {
        // Se o personagem já estiver caminhando, ele corre. Caso contrário,
        // ele apenas caminha.
        if (!isWalking)
        {
            actionSpeed = walkingSpeed;
        }
        else if (isWalking && (actionSpeed > 0 && actionSpeed >= walkingSpeed || actionSpeed < 0 && actionSpeed <= -walkingSpeed))
        {
            actionSpeed = runningSpeed;
        }
        //Debug.Log(isWalking);
        //Debug.Log(actionSpeed);
        //Debug.Log(walkingSpeed);
        //Debug.Log(actionSpeed == walkingSpeed);
        // Corrige a mudança de direção, em caso de mudaça abrupta de direção
        /*if (direction == LEFT && actionSpeed > 0 || direction == RIGHT && actionSpeed < 0)
        {
            Debug.Log("Invertendo...");
            actionSpeed *= (-1);
            walkingSpeed *= (-1);
            runningSpeed *= (-1);
        }*/
        if (direction == LEFT && actionSpeed > 0)
        {
            actionSpeed = -actionSpeed;
        }
        else if (direction == RIGHT && actionSpeed < 0)
        {
            actionSpeed *= (-1);
        }
        //Debug.Log(actionSpeed.ToString() + " | " + walkingSpeed.ToString() + " | " + runningSpeed.ToString());
    }
    private void SpeedDown()
    {
        //Debug.Log("Prev. speed: " + actionSpeed.ToString());
        // Se o usuário não estiver pressionado nenhum botão, mas estiver
        // ainda em movimento, parte para o efeito de desaceleração.
        if (actionSpeed > 0)
        {
            actionSpeed -= walkingSpeed * 0.1f;
        }
        else if (actionSpeed < 0)
        {
            actionSpeed += walkingSpeed * 0.1f;
        }
        //Debug.Log("Next speed: " + actionSpeed.ToString());
    }
    private void VerifyIfGameIsOver()
    {
        //Debug.Log(hp.ToString() + "     " + (hp < 1).ToString());
        isDead = (hp <= 0);
        //if (isDead && hp <= 0)
        //    Debug.Log(isDead.ToString() + " | " + hp + " | ");
        if (isDead && hp <= 0) //(anim.GetBool("isDead"))
        {
            anim.SetTrigger("isDead");
            //    Debug.Log("Player HP: " + hp.ToString());
            //    Time.timeScale = 0;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BreakableItem") || collision.gameObject.CompareTag("Enemy"))
        {
            //canAttack = true;
            currentTarget = collision.gameObject;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("BreakableItem"))
            isJumping = false;
        //if ((collision.gameObject.CompareTag("BreakableItem") || collision.gameObject.CompareTag("Enemy")) && canAttack)
        //{
        //    canAttack = true;
        //}
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("BreakableItem"))
            isJumping = true;
        if (collision.gameObject.CompareTag("BreakableItem") || collision.gameObject.CompareTag("Enemy"))
        {
            //canAttack = false;
            currentTarget = null;
        }
    }
    public void AttackTarget()
    {
        //Debug.Log("Current GameObject HP: " + currentHp.ToString());
        if (currentTarget)
        {
            int totalDmg = basicDamage;
            // Personagem com espada causa maior dano
            if (anim.GetBool("hasGotSword"))
            {
                totalDmg += 35;
                // Personagem em pique causa mais dano ainda...
                if (strAction == "isRunning")
                {
                    totalDmg += 10;
                    actionSpeed = 0f;
                }
            }
            if (currentTarget.CompareTag("BreakableItem"))
            {
                if (currentTarget.GetComponent<BreakableItemScript>().hp > 0)
                {
                    //Debug.Log(basicDamage * (int)Math.Floor(currentCmbLevel));
                    currentTarget.GetComponent<BreakableItemScript>().hp -= totalDmg * (int)Math.Floor(currentCmbLevel);
                }
            }
            if (currentTarget.CompareTag("Enemy"))
            {
                if (!currentTarget.GetComponent<BeaverScript>().isDead && currentTarget.GetComponent<BeaverScript>().hp > 0)
                {
                    //Debug.Log(basicDamage * (int)Math.Floor(currentCmbLevel));
                    currentTarget.GetComponent<BeaverScript>().hp -= totalDmg * (int)Math.Floor(currentCmbLevel);
                }
            }
            //Debug.Log("New GameObject HP: " + currentHp.ToString());
            //return currentTarget;
        }
    }

    public void DeclarePlayerAsDead()
    {

        // Provisório, apenas para diblar o problema com as animaçòes:
        hp = 2;
    }
}
