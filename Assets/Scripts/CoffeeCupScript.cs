using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeCupScript : CollectableItemScript
{
    private void OnPlayerCollect()
    {
        PlayerScript playerScriptObject = player.GetComponent<PlayerScript>();
        if (playerScriptObject.ap + 10 > 100)
        {
            playerScriptObject.ap = 100;
        }
        else
        {
            playerScriptObject.hp += 10;
        }
    }
}

