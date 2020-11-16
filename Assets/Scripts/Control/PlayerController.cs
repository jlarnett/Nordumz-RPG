using System;
using GameDevTV.Inventories;
using GameDevTV.UI;
using GameDevTV.UI.Inventories;
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
        [SerializeField] private float playerSpeedFraction = 1f;                     //Handles player run speed when attacking. 1 = max
        [SerializeField] private CursorMapping[] cursorMappings = null;             //Array of cursorMappings for hovering over raycastable items
        [SerializeField] private float maxNavMeshProjectionDistance = 1f;            //Not sure
        [SerializeField] private float raycastRadius = 1f;                          //raycast size
        [SerializeField] private float teleportDistance = 40;                       //total teleport distance in vector3 distance
        [SerializeField] private float MovementBufferDistance = 1;                   //Movement buffer distance
        [SerializeField] private float InteractBufferDistance = 7;                  //interact buffer distance
        [SerializeField] private ShowHideUI ShowHideUI = null;

        private bool isDraggingUI = false;

        //Class References
        private Mover mover;
        private Health health;
        private PlayerConversant playerConversant;

        private void Awake()
        {
            //Initialize components
            health = GetComponent<Health>();            
            mover = GetComponent<Mover>();
        }
        void Update()
        {
            //On Update Interaction Manager function. UI, Components, Movement
            if (InteractWithUI()) return;                   //If player is interacting with UI stop here and dont interact with anything else.      //This is here to give player option to interact with UI while dead even

            if(health.IsDead())
            {
                SetCursor(CursorType.None);                 //movement cursor cause it is best
                return;                                     //if character is dead do no interacting at all. - So takes control away if dead
            }

            CheckSpecialAbilityKeys();                      //Update

            if (InteractWithComponent()) return;            //Interact with IRAYCASTABLE components in the world
            if (InteractWithMovement())
            {
                if (Input.GetMouseButtonDown(0))            //This handles canceling dialogue if the player clicks to run after dialogue starts.
                {
                    MovementCancel();
                }

                return;
            }                 

            SetCursor(CursorType.None);                         //Default Cursor set if we make it this far in Update
        }

        private bool InteractWithUI()
        {
            //Interact Order 0
            //Handles player controller UI interaction behavior
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
            //Interact Order 1
            //Extremely important. Interacts with all raycastable items that can be clicked. Sorted in order of closeness of position.
            RaycastHit[] hits = RaycastAllSorted();                                 // Get all hits from mouse raycast

            foreach (RaycastHit hit in hits)                                        //We go through all hits in the raycast
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();      //Check all hits to sett if it has Iraycastable component. and then store them in array.
                foreach (IRaycastable raycastable in raycastables)                              //We cycle through all the raycastable components we collected in our hits
                {

                    //Positionaly blocker. A little blocky but keeps from interacting with items too soon. Want to maybe make Iactionable later so that it smoother?
                    if (Vector3.Distance(transform.position, raycastable.GetPosition()) < InteractBufferDistance)
                    {
                        mover.Cancel();

                        if (raycastable.HandleRaycast(this))            //If the raycastables Handle Raycast method is true we return true & set combat curser
                        {
                            SetCursor(raycastable.GetCursorType());     //Sets the cursor = too the value set in iraycastable implementation
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;                                                   //If we make it here on the update cycle that means there were no raycastable components
        }

        private bool InteractWithMovement()
        {
            //Interact Order = 2
            //Creates a bool = raycast at mouseposition and out sends parameter hit. if hit is true we get Character Mover.StartMoveAction Component. 
            //returns true if raycast hit = true, returns false if no Raycast hit. 
            //            RaycastHit hit;
            //            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            Vector3 target;
            bool hasHit = RaycastNavMesh(out target); //bool that verifies if we hit a navemesh target & passes it out to vecto3 target

            if (hasHit) //If we hit
            {
                if (!mover.CanMoveTo(target)) return false; //If we cant move to this target stop

                if (Input.GetMouseButton(0))
                {
                    mover.StartMoveAction(target, playerSpeedFraction);
                    //Speed fraction alters player speed mainly for AI dashing
                }

                //Test experiement
                if (Vector3.Distance(transform.position, target) < MovementBufferDistance)
                {
                    mover.Cancel();
                }

                SetCursor(CursorType.Movement);
                return true;

            }

            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            //Sorts the order of raycast relative to distance of camera
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
            //Raycast the navmesh and out returns a vector3 to our private target variable.
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
            //Setrs the cursor depending on the cursorType passed in.
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            //Returns cursormapping based on the type pased in.
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
            bool hasHit = RaycastNavMesh(out var target);

            if (hasHit)
            {
                if (mover.CanMoveTo(target))
                {
                    if (Vector3.Distance(transform.position, target) > teleportDistance) return;
                    mover.Teleport(target);
                }
            }
        }

        private void MovementCancel()
        {
            //This handles canceling player dialogue if they click and begin moving after they start dialogue
            if (playerConversant != null)
            {
                playerConversant.Quit();
            }

            if (ShowHideUI != null)
            {
                ShowHideUI.HideOtherInventory(ShowHideUI.gameObject);
            }
        }
    }
}
