using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

public class QuestItemRemover : MonoBehaviour
{
    [SerializeField] private string ItemID;
    [SerializeField] private int amount;

    public void RemoveInventoryQuestItem()
    {
        Inventory playerInventory = Inventory.GetPlayerInventory();

        if(playerInventory == null) return;
        playerInventory.RemoveItemFromInventory(ItemID, amount);
    }
}
