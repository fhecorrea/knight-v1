using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodScript : CollectableItemScript
{
    private void OnPlayerCollect()
    {
        PlayerScript playerScriptObject = player.GetComponent<PlayerScript>();
        if (playerScriptObject.hp + 50 > 100)
        {
            playerScriptObject.hp = 100;
        }
        else
        {
            playerScriptObject.hp += 50;
        }
    }
}
