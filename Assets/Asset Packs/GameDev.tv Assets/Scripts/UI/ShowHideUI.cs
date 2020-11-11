using GameDevTV.UI.Inventories;
using UnityEngine;

namespace GameDevTV.UI
{
    public class ShowHideUI : MonoBehaviour
    {
        [SerializeField] KeyCode toggleKey = KeyCode.Escape;
        [SerializeField] GameObject uiContainer = null;
        [SerializeField] private GameObject otherInventoryContainer = null;
        [SerializeField] private InventoryUI otherInventoryUI = null;


        // Start is called before the first frame update
        void Start()
        {
            uiContainer.SetActive(false);

            if(otherInventoryContainer == null) return;
            otherInventoryContainer.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                otherInventoryContainer.SetActive(false);
                Toggle();
            }
        }

        public void Toggle()
        {
            uiContainer.SetActive(!uiContainer.activeSelf);
        }

        public void ShowOtherInventory(GameObject go)           //Displays otehr inventory
        {
            uiContainer.SetActive(true);


            if (otherInventoryContainer == null) return;


            otherInventoryContainer.SetActive(true);
            otherInventoryUI.Setup(go);

        }

        public void HideOtherInventory(GameObject go)
        {
            if (otherInventoryContainer == null) return;

            otherInventoryContainer.SetActive(false);
        }
    }
}