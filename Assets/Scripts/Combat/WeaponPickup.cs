using System.Collections;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] public float respawnTime = 5f;
        [SerializeField] public WeaponConfig weaponDrop = null;
        [SerializeField] public float healthToRestore = 0;

        private void OnTriggerEnter(Collider other)             //If the player enters our collider
        {
            if (other.tag == "Player")
            { 
                Pickup(other.gameObject);                          //gives player weapon and calls the hide and respawn method for drop
            }   
        }

        private void Pickup(GameObject subject)        //Takes in game object to make it more accesseble
        {
            if (weaponDrop != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weaponDrop);  //Assigns the fighter component to subject & equips weapon if we have one

            }

            if (healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }
            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);                              //Disabled pickup
            yield return new WaitForSeconds(seconds);           //Delays    
            ShowPickup(true);               //Enables pickup
        }

        private void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow; //Disable drop collider

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        public bool HandleRaycast(PlayerController callingController)               //We Handle what happens when players mouse raycast hits weapon pickup
        {
            if (Input.GetMouseButtonDown(0))                    //If mouse left button is down
            {
                Pickup(callingController.gameObject);      //Then we call Pickup and pass in the player controller of the player
            }

            return true;            //returns true that yes it is hitting me an raycastable object cant do other stuff
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}
