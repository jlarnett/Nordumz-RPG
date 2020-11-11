using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{

    public class CinematicDeployment : MonoBehaviour
    {
        private bool alreadyTriggered = false;

        
        private void OnTriggerEnter(Collider other)
        {
            //Checks if scene has already been played & makes sure only player can activate
            if (!alreadyTriggered && other.gameObject.tag == "Player")
            {
                print("yoooo");
                alreadyTriggered = true;
                GetComponent<PlayableDirector>().Play();
            }
            else
            {
                
            }
        }
    }
}