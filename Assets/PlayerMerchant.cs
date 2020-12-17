using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

public class PlayerMerchant : MonoBehaviour
{
    private MerchantInventory currentInventory;
    private bool isBuying = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerable GetCurrentItems()
    {
        return currentInventory.GetItems();
    }

    public bool IsActive()
    {
        return currentInventory != null;
    }

    public bool IsBuying()
    {
        return isBuying;
    }

    public string GetShopName()
    {
        throw new System.NotImplementedException();
    }

    public string GetShopCoins()
    {
        throw new System.NotImplementedException();
    }

    public string GetPlayerCoins()
    {
        return "";
    }
}
