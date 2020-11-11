using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Dialogue;
using UnityEngine;




namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] private float interactDistance = 5f;
        [SerializeField] private Dialogue AIDialogue = null;
        [SerializeField] private string conversantName;

        GameObject player;


        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        private void Update()
        {
        }

        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (CheckDistance() != true)            //If player is not within InteractDistance player cant handle raycast
            {
                return false;
            }

            if (AIDialogue == null)
            {
                return false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<PlayerConversant>().StartDialogue(this, AIDialogue);
            }
            return true;
        }

        public string GetName()
        {
            return conversantName;
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



