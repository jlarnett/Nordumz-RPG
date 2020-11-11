using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        public enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField] public int sceneToLoad = -1;
        [SerializeField] public Transform spawnSpoint;
        [SerializeField] public DestinationIdentifier destination;
        [SerializeField] public float fadeOutTime = 1f;
        [SerializeField] public float fadeInTime = 2f;
        [SerializeField] public float fadeWaitTime = 1f;


        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")  //Makes sure only player can trigger poral
            {
                StartCoroutine(Transition());   //Start Corotine Transition() BELOW
            }
        }


        private IEnumerator Transition()    //Ienumator basically means this runs and holds data in between runs and a
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load is not set");
                yield break;
            }

            DontDestroyOnLoad(gameObject);  //Dont destroy Scene Portal On load.

            Fader fader = FindObjectOfType<Fader>();    //gets fader object
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();      //Gets the savingWrapper gameobject

            //REMOVE PLAYER CONTROL HERE
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();      //We get access to the players controller
            playerController.enabled = false;                                                                           //Then we disable the players control

            yield return fader.FadeOut(fadeOutTime);    //calls fader fadeout method and passes in a time


            //Save current level before portaling
            wrapper.Save();                                                 //Saves current Level

            yield return SceneManager.LoadSceneAsync(sceneToLoad);  //Yield Returns 

            //REMOVE CONTROL FROM NEW PLAYER
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();      //We get access to the players controller
            newPlayerController.enabled = false;                                                                            //Then we remove players control

            wrapper.Load();

            Portal otherPortal = GetOtherPortal();      //assigns portal to method get other portal
            UpdatePlayer(otherPortal);          //Calls Update Player

            wrapper.Save();                     //Helps with loading a player in the scene they were previously in 

            yield return new WaitForSeconds(fadeWaitTime);  // wait time
            fader.FadeIn(fadeInTime);       //Gives the player controll back immediately after. After changes to fader no worries of faders fighting

            //rESTORE CONTROL HERE
            newPlayerController.enabled = true;
            Destroy(gameObject);        //destroy
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");   //assign player
            player.GetComponent<NavMeshAgent>().enabled = false;

            player.transform.position = otherPortal.spawnSpoint.position; // tells navmeshagent to do teleport? May stop issues in future with portal
            player.transform.rotation = otherPortal.spawnSpoint.rotation;

            player.GetComponent<NavMeshAgent>().enabled = true;

        }

        private Portal GetOtherPortal()
        {
            //Foreach portal in all portal objects if portal is = to th one im eone jump out
            foreach (Portal portal in FindObjectsOfType<Portal>() )
            {
                if (portal == this) continue;   //Jump out if same map

                if(portal.destination != destination) continue; //we are only going to return the  portal if it has right destination && not us.

                return portal;          //Returns portal to method that we are going to?
            }

            return null;    //if no portals return null
        }
    }
}
