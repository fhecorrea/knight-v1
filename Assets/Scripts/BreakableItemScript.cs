using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableItemScript : MonoBehaviour
{
    public int hp = 100;
    private Animator anim;

    // Update is called once per frame
    void Update()
    {
        anim = GetComponent<Animator>();
        anim.SetInteger("hp", hp);
    }
}
