using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            //Interact Order = 1?
            //We simply check if we can attack and attack

            if (!enabled) return false;

            if (!callingController.GetComponent<Fighter>().CanAttack(gameObject))  //We check if the calling player can attack the gameobject enemy attached.
            {
                return false;                                           //if player cant attack we return false
            }

            if (Input.GetMouseButton(0))        //If Mouse button is held down  and all things are above qualifiers work we attack it
            {
                callingController.GetComponent<Fighter>().Attack(gameObject);       //We attack the gameobject enemey attached
            }

            return true;
        }
    }
}
