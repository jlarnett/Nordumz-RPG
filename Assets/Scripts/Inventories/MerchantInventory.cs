using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.Inventories
{
    [CreateAssetMenu(menuName = ("RPG/Inventory/MerchantItemTable"))]
    public class MerchantInventory : ScriptableObject
    {
        [SerializeField] private MerchantItemList[] ItemList;
        [SerializeField] private string shopName;

        [System.Serializable]
        public class MerchantItemList
        {
            public MerchantItemHolder[] items;
        }

        [System.Serializable]
        public class MerchantItemHolder
        {
            public InventoryItem item;
            public int initialItemQuantity;
            public int sellPrice;
            public int BuyPrice;
        }
    }
}

