/*using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DisplayInventory : MonoBehaviour
{
    public MouseItem mouseItem = new MouseItem();

    public GameObject inventoryPrefab; //All items use
    public InventoryObject inventory;

    public int X_START;
    public int Y_START;
    public int X_SPACE_BETWEEN_ITEMS;
    public int Y_SPACE_BETWEEN_ITEMS;
    public int NUMBER_OF_COLUMNS;

    Dictionary<GameObject, InventorySlot> itemsDisplayed = new Dictionary<GameObject, InventorySlot>();         //Seperating our display code from system code

    //Start is called before the first frame update
    void Start()
    {
        CreateSlots();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSlots();
    }

    public void UpdateSlots()
    {
        //Displays slots
        foreach (KeyValuePair<GameObject, InventorySlot> _slot in itemsDisplayed)           //Foreach slot in item displayed dictionary
        {
            if (_slot.Value.ID >= 0)            //If slot has item in it
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[_slot.Value.item.Id].uiDisplay;                      //Keeps load from being too heavy. Done this way to not actually save ui display
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);      //Sets color /alpha so we dont need a empty ivnentory pic

                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");       //Sets inventory text amount value = _slot item amount

            }                                                                                   //Keeps this part graphical and other part backend
            else
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;      //Keeps load from being too heavy. Done this way to not actually save ui display
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);      //Sets color /alpha = 0so we dont need a empty ivnentory pic

                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";     //Sets inventory text amount value = _slot item amount

            }
        }
    }

    public void CreateSlots()
    {
        //Create Inventory Slots
        itemsDisplayed = new Dictionary<GameObject, InventorySlot>();       //Just in case stop errors

        for (int i = 0; i < inventory.Container.Items.Length; i++)          //Goes through inventory sizes
        {
            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);           //prefab in editor, vector 0 initial position of 0, 0 rotation, set parent of object

            AddEvent(obj,EventTriggerType.PointerEnter, delegate {OnEnter(obj);});             //passes in object which is button we passed to prefab for inventory slots
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });             //passes in object which is button we passed to prefab for inventory slots
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });             //passes in object which is button we passed to prefab for inventory slots
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });             //passes in object which is button we passed to prefab for inventory slots
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });             //passes in object which is button we passed to prefab for inventory slots

            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            itemsDisplayed.Add(obj, inventory.Container.Items[i]);
        }
    }

    private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();

        eventTrigger.eventID = type;        //Passes event type and setting it
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);

    }

    public void OnEnter(GameObject obj)
    {

    }

    public void OnExit(GameObject obj)
    {

    }

    public void OnDragStart(GameObject obj)
    {
        //Visual rep of draging item nothing to do with system
        var mouseObject = new GameObject();
        var rt = mouseObject.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(75, 50);
        mouseObject.transform.SetParent(transform.parent);

        if (itemsDisplayed[obj].ID >= 0) //checks if item is on inventory slot
        {
            var img = mouseObject.AddComponent<Image>();
            img.sprite = inventory.database.GetItem[itemsDisplayed[obj].ID].uiDisplay;
            img.raycastTarget = false;          // has to be this way or will always block mouse
        }

        mouseItem.obj = mouseObject;            //Assigns mouse item.obj = mouseObject
        mouseItem.item = itemsDisplayed[obj];
    }

    public void OnDragEnd(GameObject obj)
    {
        if (mouseItem.hoverObj)
        {

        }
        else
        {
            
        }
        Destroy(mouseItem.obj);         //Destroys mouse item obj aka inventory icon copy?
        mouseItem.item = null;          //Says mouse no longer has item
    }

    public void OnDrag(GameObject obj)
    {
        if (mouseItem.obj != null) //we do have item on mouse
        {
            mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;     //Set copy position = mouse position
        }
    }

    public Vector3 GetPosition(int i)
    {
        return  new Vector3(X_START + (X_SPACE_BETWEEN_ITEMS * (i % NUMBER_OF_COLUMNS)), Y_START + (-Y_SPACE_BETWEEN_ITEMS * (i/NUMBER_OF_COLUMNS)), 0f);
    }
}

public class MouseItem
{
    //Handles mouse events related to inventory slots and mouse events
    public GameObject obj;
    public InventorySlot item;
    public InventorySlot hoverItem;
    public GameObject hoverObj;
}*/