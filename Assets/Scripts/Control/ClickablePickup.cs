using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;



namespace RPG.Control
{
    [RequireComponent(typeof(Pickup))]
    public class ClickablePickup : MonoBehaviour, IRaycastable
    {
        private Pickup pickup;

        public CursorType GetCursorType()
        {
            if (pickup.CanBePickedUp())
            {
                return CursorType.Pickup;
            }
            else
            {
                return CursorType.Movement;
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                pickup.PickupItem();
            }

            return true;
        }

        public Vector3 GetPosition()
        {
            if (transform.position == null) return Vector3.zero;
            return transform.position;
        }

        private void Awake()
        {
            pickup = GetComponent<Pickup>();
        }

    }
}

