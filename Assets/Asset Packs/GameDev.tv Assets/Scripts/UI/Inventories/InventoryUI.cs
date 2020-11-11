using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Inventories;
using TMPro;

namespace GameDevTV.UI.Inventories
{
    /// <summary>
    /// To be placed on the root of the inventory UI. Handles spawning all the
    /// inventory slot prefabs.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        // CACHE
        Inventory selectedInventory;

        // LIFECYCLE METHODS
        [SerializeField] InventorySlotUI InventoryItemPrefab = null;
        [SerializeField] private bool isPlayerInventory = true;
        [SerializeField] private TextMeshProUGUI Title = null;

        private void Awake() 
        {
            if (isPlayerInventory)
            {
                selectedInventory = Inventory.GetPlayerInventory();
                selectedInventory.inventoryUpdated += Redraw;
            }
        }

        private void Start()
        {
            if (isPlayerInventory)
            {
                Redraw();
            }
        }

        // PRIVATE

        private void Redraw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < selectedInventory.GetSize(); i++)
            {
                var itemUI = Instantiate(InventoryItemPrefab, transform);
                itemUI.Setup(selectedInventory, i);
            }
        }

        public bool Setup(GameObject user)
        {
            if (user.TryGetComponent(out selectedInventory))
            {
                selectedInventory.inventoryUpdated += Redraw;
                Title.text = selectedInventory.name;

                Redraw();
                return true;
            }
            return false;
        }
    }
}