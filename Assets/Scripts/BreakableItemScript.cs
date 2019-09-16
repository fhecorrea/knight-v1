using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableItemScript : MonoBehaviour
{
    public int hp = 100;
    private Animator anim;
    private BoxCollider2D boxCollider2d;

    void Start()
    {
        anim = GetComponent<Animator>();
        boxCollider2d = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        anim.SetInteger("hp", hp);
        if (hp < 1 && boxCollider2d.isTrigger == false)
        {
            boxCollider2d.isTrigger = true;
        }
    }
}
