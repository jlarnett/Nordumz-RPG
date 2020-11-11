using System;
using GameDevTV.Inventories;
using RPG.Movement;
using UnityEngine;
using RPG.Attributes;
using RPG.Dialogue;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
       // public InventoryObject inventory;

        [SerializeField] public float playerSpeedFraction = 1f; //Handles player run speed when attacking.
        [SerializeField] private CursorMapping[] cursorMappings = null;
        [SerializeField] public float maxNavMeshProjectionDistance = 1f;
        [SerializeField] public float raycastRadius = 1f;
        [SerializeField] public float teleportDistance = 40;

        private Health health;
        private bool isDraggingUI = false;

        private void Awake()
        {
            health = GetComponent<Health>();    //On Start initialize Health component
        }
        void Update()
        {
            if (InteractWithUI()) return;   //If player is interacting with UI stop here and dont interact with anything else.      //This is here to give player option to interact with UI while dead even

            if(health.IsDead())
            {
                SetCursor(CursorType.None);     //movement cursor cause it is best
                return; //if character is dead do no interacting at all. - So takes control away if dead
            }

            CheckSpecialAbilityKeys();      //Update

            if (InteractWithComponent()) return;            //Interact with IRAYCASTABLE components in the world
            if (InteractWithMovement())
            {
                if (Input.GetMouseButtonDown(0))    //This handles canceling dialogue if the player clicks to run after dialogue starts.
                {
                    MovementDialogueCancel();
                }

                return;
            }                 

            SetCursor(CursorType.None);     //Default Cursor set if we make it this far in Update
        }

        private bool InteractWithUI()
        {

            //if (EventSystem.current == null) return false;

            if (Input.GetMouseButtonUp(0))          //If mouse button is up is dragging is = false
            {
                isDraggingUI = false;
            }

            //Checks if cursor is over a UI gameobject
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))            //Is dragging is set to true when over UI & left mouse button is down
                {
                    isDraggingUI = true;
                }

                SetCursor(CursorType.UI);   //SETS ui cursor at right time.
                return true;
            }

            if (isDraggingUI)
            {
                return true;
            }

            return false;
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();      // Get all hits from mouse raycast

            foreach (RaycastHit hit in hits)        //We go through all hits in the raycast
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();      //Check all hits to sett if it has Iraycastable component. and then store them in array.
                foreach (IRaycastable raycastable in raycastables)      //We cycle through all the raycastable components we collected in our hits
                {
                    if (raycastable.HandleRaycast(this))            //If the raycastables Handle Raycast method is true we return true & set combat curser
                    {
                        SetCursor(raycastable.GetCursorType());     //Sets the cursor = too the value set in iraycastable implementation
                        return true;
                    }
                }
            }

            return false;               //If we make it here on the update cycle that means there were no raycastable components
        }

        private bool InteractWithMovement()
        {
            //Interact Order = 2
            //Creates a bool = raycast at mouseposition and out sends parameter hit. if hit is true we get Character Mover.StartMoveAction Component. 
            //returns true if raycast hit = true, returns false if no Raycast hit. 
            //            RaycastHit hit;
            //            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            Vector3 target;
            bool hasHit =
                RaycastNavMesh(
                    out target); //bool that verifies if we hit a navemesh target & passes it out to vecto3 target

            if (hasHit) //If we hit
            {
                if (!GetComponent<Mover>().CanMoveTo(target)) return false; //If we cant move to this target stop

                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>()
                        .StartMoveAction(target,
                            playerSpeedFraction); //Speed fraction alters player speed mainly for AI dashing
                }

                SetCursor(CursorType.Movement);
                return true;

            }

            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            //Get all hits
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);

            //Sort by distance
            float[] distances = new float[hits.Length];     //Create an aray same size as hits 

            //build array distances
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            //sort the hits
            Array.Sort(distances, hits);

            //Return sortged array
            return hits;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            //Raycast to terrain
            RaycastHit hit;

            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;        //Hey we didnt get any hits or informaiton

            //Find nearest Navmesh point
            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);

            if (!hasCastToNavMesh) return false;
            target = navMeshHit.position;               //Updates the player target move position = the hit position

            return true;
        }

        private static Ray GetMouseRay()
        {
            //Sends a ray where player clicks within camera & Detects colisions for player movement
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)       // foreach mapping item in cursorMappings. If it equals the type passed in we return matched mapping
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            }

            return cursorMappings[0];
        }

        [System.Serializable]
        struct CursorMapping                //Mapping between cursor type & texture?
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        public void OnTriggerEnter(Collider other)
        {
            //var item = other.GetComponent<GroundItem>();  //set item = item of colliders component
            //if (item)
            //{
              //  Destroy(other.gameObject);  //destroys object that was added to inventory
            //}
        }

        private void CheckSpecialAbilityKeys()
        {
            var actionStore = GetComponent<ActionStore>();
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                actionStore.Use(0, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                actionStore.Use(1, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                actionStore.Use(2, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                actionStore.Use(3, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                actionStore.Use(4, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                actionStore.Use(5, gameObject);
            }

            if (Input.GetMouseButtonDown(1))
            {
                Teleport();
            }
        }

        private void Teleport()
        {
            Vector3 target;

            bool hasHit = RaycastNavMesh(out target);

            if (hasHit)
            {
                if (GetComponent<Mover>().CanMoveTo(target))
                {
                    if (Vector3.Distance(transform.position, target) > teleportDistance) return;
                    GetComponent<Mover>().Teleport(target);
                }
            }
        }

        private void MovementDialogueCancel()
        {
            //This handles canceling player dialogue if they click and begin moving after they start dialogue
            GetComponent<PlayerConversant>().Quit();
        }
    }
}
