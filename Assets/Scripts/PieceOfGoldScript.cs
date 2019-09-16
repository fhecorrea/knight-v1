using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceOfGoldScript : CollectableItemScript
{
    private void OnPlayerCollect()
    {
        player.GetComponent<PlayerScript>().hasPieceOfGold = true;
    }
}
