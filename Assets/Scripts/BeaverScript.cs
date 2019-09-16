using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RectTransform;
using UnityEngine.UI;
using System;

public class BeaverScript : MonoBehaviour
{
    public bool canBeMad = false;
    public bool isDead = false;
    public int hp = 500;
    public int baseDamage = 5;
    public float walkSpeed = 0f;
    public float defaultWalkTime = 10f;
    public GameObject defaultPlayer;
    public GameObject pieceOfGold;
    public Image healthBar;
    private bool isMad = false;
    private bool isWalking = false;
    private float defaultMass;
    private float rageLevel = 0f;
    private float walkTime;
    private float walkInMilSec;
    private float disappearInMilSec = 15f;
    private Animator animator;
    private Collider2D collider2d;
    private GameObject target = null;
    private Rigidbody2D rigidbody2d;
    private SpriteRenderer spriteRenderer;
    // O valor pelo qual o HP será dividido e resultará num valor equivalente 
    // a uma taxa percentual (de zero a 100). Ex.: Se HP máximo do monstro == 500,
    // então o fator seria == 5.
    private int fatorHp;

    private void Start()
    {
        collider2d = GetComponent<Collider2D>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMass = rigidbody2d.mass;
        fatorHp = hp / 100;
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log("HP: " + hp.ToString() + " / RAGE: " + rageLevel.ToString());
        if (isDead)
        {
            if (disappearInMilSec > 0.1f)
            {
                Debug.Log(disappearInMilSec);
                disappearInMilSec -= 0.1f;
            }
            else
            {
                Destroy(gameObject);
            }
            return;
        }
        // Atualiza a situação do castor com base na caminhada
        // Se o mob estiver esperando para andar, reduz o tempo de espera
        if (walkInMilSec > 0)
        {
            walkInMilSec -= 0.1f;
        }
        if (walkInMilSec <= 0 && walkTime > 0)
        {
            if (walkSpeed > 0)
                spriteRenderer.flipX = true;
            else
                spriteRenderer.flipX = false;
            transform.Translate(walkSpeed, 0, 0);
            walkTime -= 0.1f;
        }
        //Debug.Log(walkTime.ToString() + " | " + rigidbody2d.velocity.x.ToString() + " | " + (walkTime > 0 && rigidbody2d.velocity.x != 0).ToString());
        isWalking = walkTime > 0;
        if (!isMad && rigidbody2d.mass != defaultMass)
        {
            rigidbody2d.mass = defaultMass;
        }
        // Se o castor puder enloquecer, mas não estiver enloquecido ainda,
        // possuir menos de 10 pontos de HP ou mais de 3pts de fúria.
        if (canBeMad && !isMad && (hp < 10 || rageLevel >= 3))
        {
            isMad = true;
            rigidbody2d.mass *= 2; // Aumenta o peso do castor
            hp *= 10; // multiplica o HP atual por 100 (min. 100, máx.: 5000)
            fatorHp = 50;
        }
        if (!target && rageLevel > 0.1f)
            rageLevel -= 0.1f;
        if (defaultPlayer)
        {
            float distance = defaultPlayer.transform.position.x - transform.position.x;
            float heightDiff = defaultPlayer.transform.position.y - transform.position.y;
            if (distance > -1f && distance < 1f && heightDiff > -0.6f && heightDiff < 0.6f)
            {
                if (distance > -0.3f && distance < 0.3f && heightDiff > -0.006f && heightDiff < 0.006f)
                {
                    if (!defaultPlayer.GetComponent<PlayerScript>().isDead)
                        target = defaultPlayer;
                }
                else if (walkTime <= 0)
                {
                    if (distance > 0 && walkSpeed < 0 || distance < 0 && walkSpeed > 0)
                    {
                        walkSpeed *= (-1);
                    }
                    // Define um tempo máximo em que o castor caminhará
                    walkTime += defaultWalkTime;
                    //transform.Translate(walkSpeed, 0, 0);
                    //Debug.Log(walkSpeed.ToString());
                    // Faz o castor andar de fato
                    rigidbody2d.velocity = new Vector2(walkSpeed, 0);
                    if (target != null)
                        target = null;
                }
            }
            //Debug.Log("Distância do castor para o alvo: " + (distance).ToString());
            //Debug.Log(distance > -1f && distance < 1f && heightDiff > -0.6f && heightDiff < 0.6f);
            //Debug.Log("Diferença de altura entre ambos: " + (heightDiff).ToString());
        }
    }

    private void OnCollisionEnter2D(Collision2D collider2d)
    {
        if (collider2d.gameObject.CompareTag("Player"))
        {
            target = collider2d.gameObject;
            rageLevel += 0.5f;
            // Se o castor estiver andando, ele para.
            //if (walkTime > 0)
            //    walkTime = 0f;
            if (isWalking)
                isWalking = !isWalking;
            if (walkTime > 0f)
                walkTime = 0f;
        }
    }
    private void OnCollisionStay2D(Collision2D collider2d)
    {
        if (collider2d.gameObject.CompareTag("Player"))
        {
            // Incrementa o nível de raiva do castor até ele se sentir pronto para atacar
            if (rageLevel < 3f)
            {
                rageLevel += 0.25f;
            }
            if (target == null)
            {
                if (!collider2d.gameObject.GetComponent<PlayerScript>().isDead)
                {
                    target = collider2d.gameObject;
                }
                else
                {
                    target = null;
                }
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collider2d)
    {
        if (collider2d.gameObject.CompareTag("Player"))
        {
            target = null;
        }
    }

    private void LateUpdate()
    {
        animator.SetFloat("rageLevel", rageLevel);
        animator.SetInteger("hp", hp);
        animator.SetBool("isMad", isMad);
        animator.SetBool("isWalking", isWalking);
        float currentHealth = 0f;
        if (hp > 0)
        {
            //Debug.Log((100 * ((hp / fatorHp) * 0.01)).ToString());
            // Tira o percentual considerando o HP máximo do monstro
            currentHealth = (float)(100 * ((hp / fatorHp) * 0.01));
        }
        healthBar.rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, currentHealth);
        if (!isDead)
        {
            isDead = (hp <= 0);
            if (isDead)
            {
                // Elimina o corpo rígido, para o jogador poder transpassar o 'morto' sem empurrá-lo
                Destroy(rigidbody2d);
                collider2d.isTrigger = true;
                // Roda a animação de "queda" do morto
                animator.SetTrigger("isDead");
                // Derruba o item carregado, caso o castor carregue algum
                if (pieceOfGold != null)
                {
                    pieceOfGold.transform.position = transform.position;
                    // Para dar um efeito de item caído, dá velocidade ao item a ser dropado
                    pieceOfGold.SetActive(true);
                    transform.Translate(0.01f, 0.02f, 0f);
                }
            }
        }
        //    RandomWalk();
    }
    private void RandomWalk()
    {
        if (target == null)
        {
            if (!isWalking)
            {
                if (walkSpeed > 0)
                {
                    walkSpeed = -walkSpeed;
                }
                else if (walkSpeed < 0)
                {
                    walkSpeed *= (-1);
                }
                walkTime = defaultWalkTime;
                walkInMilSec = 2.0f;
            }
            //GetComponent<Rigidbody2D>().velocity = new Vector2(0f, jumpingForce);
        }
    }

    public void AttackTarget()
    {
        // O castor tem nível de raiva suficiente para atacar? Então ataca!
        //Debug.Log(rageLevel.ToString());
        if (target != null)
        {
            if (target.GetComponent<PlayerScript>().isDead)
            {
                target = null;
            }
            else if (rageLevel > 2f)
            {
                int dmg = baseDamage;
                if (isMad)
                {
                    dmg += 5;
                }
                //Debug.Log("Current Player HP: " + target.GetComponent<PlayerScript>().hp.ToString());
                if (target.GetComponent<PlayerScript>().hp > 0)
                {
                    target.GetComponent<PlayerScript>().hp -= dmg;
                }
                //Debug.Log("New Player HP: " + target.GetComponent<PlayerScript>().hp.ToString());
            }
            else
            {
                Debug.Log("Miss!");
            }
        }
    }

    public void DeclareDefeated()
    {
        hp = 2;
        isMad = false;
        rageLevel = 0f;
        target = null;
        walkTime = 0f;
        isDead = true;
    }
}
