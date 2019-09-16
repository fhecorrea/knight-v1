using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItemScript : MonoBehaviour
{
    public GameObject player;
    //public GameObject beaver;
    private void OnTriggerEnter2D(Collider2D collider2d)
    {
        if (collider2d.gameObject.CompareTag("Player"))
        {
            OnPlayerCollect();
            //collider2d.gameObject.GetComponent<PlayerScript>().hasPieceOfGold = true;
            Destroy(gameObject);
        }
    }
    private void OnPlayerCollect()
    {
        // It does something...!
    }
}
