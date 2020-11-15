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
        GameObject player;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        public CursorType GetCursorType()
        {
            return CursorType.UI;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                    ShowHideUI ui = FindObjectOfType<ShowHideUI>();
                    if (ui == null) return false;

                    ui.ShowOtherInventory(gameObject);

                return true;
            }
            return true;
        }

        public Vector3 GetPosition()
        {
            if (transform.position == null) return Vector3.zero;
            return transform.position;
        }

    }
}

