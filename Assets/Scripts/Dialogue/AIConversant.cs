using RPG.Attributes;
using RPG.Combat;
using RPG.Control;
using UnityEngine;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] private Dialogue AIDialogue = null;
        [SerializeField] private string conversantName;

        GameObject player;
        private Health health;
        private Fighter fighter;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            health = GetComponent<Health>();
            fighter = GetComponent<Fighter>();
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
            if (health.IsDead())            //If player is not within InteractDistance player cant handle raycast
            {
                return false;
            }

            if (AIDialogue == null || enabled == false || fighter.enabled)
            {
                return false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<PlayerConversant>().StartDialogue(this, AIDialogue);
            }
            return true;
        }

        public Vector3 GetPosition()
        {
            if (transform.position == null) return Vector3.zero;
            return transform.position;
        }

        public string GetName()
        {
            return conversantName;
        }
    }
}



