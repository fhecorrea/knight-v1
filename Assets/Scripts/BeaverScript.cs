using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BeaverScript : MonoBehaviour
{
    public bool canBeMad = false;
    public int hp = 500;
    public int baseDamage = 10;
    public float walkSpeed = 0f;
    public float defaultWalkTime = 10f;
    private GameObject player = null;
    private Animator animator;
    //private Rigidbody2D rigidbody2d;
    private Collider2D collider2d;
    private SpriteRenderer spriteRenderer;
    private bool isWalking = false;
    private bool isMad = false;
    private float rageLevel = 0f;
    private float walkTime;
    
    // Update is called once per frame
    void Update()
    {
        if (player != null) {
            if (player.GetComponent<PlayerScript>().IsAttacking()) {
                hp = player.GetComponent<PlayerScript>().GetReducedHp(hp);
            }
        }
        if (hp < 1) {
            isMad = false;
            rageLevel = 0f;
            player = null;
            walkTime = 0f;
        }
        //if (hp < 1) { TODO: Resolver: Rigidbody faz o objeto ultrapassar a plataforma
        //    collider2d = GetComponent<Collider2D>();
        //    collider2d.isTrigger = true;
        //}
        // O castor tem nível de raiva suficiente para atacar? Então ataca!
        if (rageLevel > 2f && player != null) {
            if (isMad) {
                baseDamage += 30;
            }
            int totalDmg = baseDamage * (int)Math.Ceiling(rageLevel);
            //Debug.Log("Current Player HP: " + player.GetComponent<PlayerScript>().hp.ToString());
            if (player.GetComponent<PlayerScript>().hp > 0) {
                player.GetComponent<PlayerScript>().hp -= totalDmg;
            }
            //Debug.Log("New Player HP: " + player.GetComponent<PlayerScript>().hp.ToString());
        }
        // Se o castor puder enloquecer, mas não estiver enloquecido ainda,
        // possuir menos de 10 pontos de HP ou mais de 3pts de fúria.
        if (canBeMad && !isMad && (hp < 10 || rageLevel >= 3)) {
            isMad = true;
            hp *= 100; // multiplica o HP atual por 100 (min. 1000, máx.: 50000)
        }
        if (animator == null) {
            animator = GetComponent<Animator>();
        }
        if (rageLevel > 0)
            rageLevel -= 0.1f;
        isWalking = walkTime > 0;
        animator.SetFloat("rageLevel", rageLevel);
        animator.SetInteger("hp", hp);
        animator.SetBool("isMad", isMad);
        animator.SetBool("isWalking", isWalking);
        if (walkTime > 0) {
            if (!spriteRenderer)
                spriteRenderer = GetComponent<SpriteRenderer>();
            //Debug.Log("Direção da caminhada: " + walkSpeed.ToString());
            if (walkSpeed > 0)
                spriteRenderer.flipX = true;
            else
                spriteRenderer.flipX = false;
            transform.Translate(walkSpeed, 0, 0);
            walkTime -= 0.1f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collider2d) {
        if (collider2d.gameObject.CompareTag("Player")) {
            player = collider2d.gameObject;
            rageLevel += 0.5f;
            // Se o castor estiver andando, ele para.
            if (walkTime > 0)
                walkTime = 0f;
        }
    }
    private void OnCollisionStay2D(Collision2D collider2d) {
        if (collider2d.gameObject.CompareTag("Player")) {
            // Incrementa o nível de raiva do castor até ele se sentir pronto para atacar
            //if (rageLevel < 3f) {
            rageLevel += 0.1f;
            //}
            if (player == null) {
                player = collider2d.gameObject;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collider2d) {
        if (collider2d.gameObject.CompareTag("Player")) {
            rageLevel = 0f;
            // Faz o castor caminhar um pouco em direção do jogador...
            if (player != null) {
                //Debug.Log("Pos. Player: " + player.transform.position.x.ToString() + "Pos. Beaver: " + transform.position.x.ToString());
                if (player.transform.position.x < transform.position.x) {
                    walkSpeed = -walkSpeed;
                }
                else if (walkSpeed < 0) {
                    walkSpeed *= (-1);
                }
                walkTime = defaultWalkTime;
                player = null;
            }
        }
    }
}
