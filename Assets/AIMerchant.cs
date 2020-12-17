using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Attributes;
using RPG.Combat;
using RPG.Control;
using UnityEngine;

namespace RPG.Inventories
{
    public class AIMerchant : MonoBehaviour, IRaycastable
    {
        [SerializeField] MerchantInventory merchantInventory = null;
        [SerializeField] string storeName = null;

        private GameObject player;
        private Health health;
        private Fighter fighter;


        //This need to hold an initialState merchantInventory
        //This needs to track the Merchants current Inventory / GOLD
        //Upon Action we need to be able to "Buy or Sell" items to merchant & Update gold and inventory accordingly.

        //We need to somehow update the UI


        void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            health = GetComponent<Health>();
            fighter = GetComponent<Fighter>();
        }



        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public CursorType GetCursorType()
        {
            return CursorType.UI;
        }

        public bool HandleRaycast(PlayerController callingController)
        {

            if (health.IsDead()) return false;
            if (merchantInventory == null || enabled || fighter.enabled) return false;

            if (Input.GetMouseButtonDown(0))
            {
            }
            return false;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}

