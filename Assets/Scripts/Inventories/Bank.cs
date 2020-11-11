using System.Collections;
using System.Collections.Generic;
using GameDevTV.UI;
using RPG.Control;
using UnityEngine;


namespace GameDevTV.Inventories
{
    [RequireComponent(typeof(Inventory))]
    public class Bank : MonoBehaviour, IRaycastable
    {
        [SerializeField] private float interactDistance;
        GameObject player;


        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }



        public CursorType GetCursorType()
        {
            return CursorType.UI;
        }


        void Update()
        {
            if (CheckDistance()) return;
            else
            {
                FindObjectOfType<ShowHideUI>().HideOtherInventory(gameObject);
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!CheckDistance())
                {
                    return false;
                }

                else
                {
                    ShowHideUI ui = FindObjectOfType<ShowHideUI>();
                    if (ui == null) return false;

                    ui.ShowOtherInventory(gameObject);
                }
            }
            return true;
        }

        public bool CheckDistance()
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance > interactDistance)
            {
                return false;
            }

            return true;
        }
    }
}

