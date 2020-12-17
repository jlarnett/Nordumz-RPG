using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDevTV.Inventories;
using RPG.Core;
using UnityEngine;
using Random = UnityEngine.Random;


namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private string playerName;     //Shows in dialogue UI

        private Dialogue currentDialogue;
        private DialogueNode currentNode = null;
        private AIConversant currentConversant = null;
        private bool isChoosing = false;

        public event Action onConversationUpdated;


        private Inventory playerInventory;

        void Awake()
        {
            playerInventory = GetComponent<Inventory>();
        }
        
        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {

            if (currentDialogue != null)
                Quit();
            
            currentConversant = newConversant;      //References whoever is currently talking
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            onConversationUpdated();
        }

        public void Quit()
        {
            //This functions is called when player clicks quit button and triggers event. This basically just set all our state trackers to null
            currentDialogue = null;
            TriggerExitAction();
            currentNode = null;
            isChoosing = false;
            currentConversant = null;   //On quitting a dialogue sets the conversant to null
            onConversationUpdated();
        }

        public void RemoveInventoryItem()
        {
            //playerInventory.RemoveItemFromInventory(itemId, amount);
        }
        

        public bool IsActive()
        {
            return currentDialogue != null;
        }

        public bool IsChoosing()
        {
            //Returns whether the playerConversant is in isChoosing state. meaning player dialogue options are shown
            return isChoosing;
        }


        public string GetText()
        {
            //Gets the text for the current DialogueNOde
            if (currentNode == null)
            {
                return "";
            }

            return currentNode.GetText();
        }

        public string GetCurrentConversantName()
        {
            if (isChoosing)
            {
                return playerName;
            }
            else
            {
                return currentConversant.GetName();
            }
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            //returns all player dialogue children for current node and return choices
            return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;

            TriggerEnterAction();
            isChoosing = false;
            Next(); //Fowards the game so it doesn't repeat the text the character choose after he selects it. GAME OPTION / CHOICE
        }

        public void Next()
        {
            //Checks if the next step in the dialogue is a player choice or not. and then progresses the dialogue text with randomness included
            int numPlayerResponses = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();
            if (numPlayerResponses > 0)
            {
                isChoosing = true;

                TriggerExitAction();
                onConversationUpdated();
                return;
            }

            //Gets all children for AI dialogue and randomly selects which route it goes
            DialogueNode[] children = FilterOnCondition(currentDialogue.GetAIChildren(currentNode)).ToArray();
            int randomIndex = Random.Range(0, children.Count());

            TriggerExitAction();
            currentNode = children[randomIndex];
            TriggerEnterAction();

            onConversationUpdated?.Invoke();
        }

        public bool HasNext()
        {
            //returns if we are at end of dialogue or not.
            return FilterOnCondition(currentDialogue.GetAllChildren(currentNode)).Count() > 0;
        }

        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach (var node in inputNode)
            {
                if (node.CheckCondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }

        private void TriggerEnterAction()
        {
            if (currentNode != null)
            {
                TriggerAction(currentNode.GetOnEnterAction());
            }
        }

        private void TriggerExitAction()
        {
            if (currentNode != null)
            {
                TriggerAction(currentNode.GetOnExitAction());
            }
        }

        private void TriggerAction(string action)
        {
            if (action == "") return;

            foreach (DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }

        

    }
}

