﻿using GameDevTV.Saving;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] private Transform target;              //target walk position
        [SerializeField] private float maxSpeed = 6;            //Max walk speed. Somehow bottlenecked by animator.
        [SerializeField] private float maxNavPathLength = 40f;  //Max click distance.

        //Class References
        private ActionScheduler actionScheduler;
        private NavMeshAgent NavMeshAgent;
        private Health health;
        private Animator animator;

        private void Awake()
        {
            //Assigns NavmeshAgent to Whatever Gameobject Assigned (Character) Component<NavMeshAgent>
            NavMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();

        }

        void Update()
        {
            NavMeshAgent.enabled = !health.IsDead();            //enables navmesh = if character is alive.
            //Calls Update Animator                             //Just allows movement over dead bodies
            UpdateAnimator();
        }
        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);          //We get the path our player is trying to take

            if (!hasPath) return false;                                             //If we dont have path return false
            if (path.status != NavMeshPathStatus.PathComplete) return false;        //Keep from using incomplete paths... may change later for teleporting over water
            if (GetPathLength(path) > maxNavPathLength) return false;

            return true;
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            //Calls actionscheduler to start action. This takes care of any overhead the action needs before starting.
            actionScheduler.StartAction(this);
            MoveTo(destination, speedFraction);
        }

        private float GetPathLength(NavMeshPath path)
        {
            //CALCULATES PLAYERS TOTAL NAVMESH PATH DISTANCE
            float totalPathLength = 0;

            if (path.corners.Length < 2) return totalPathLength;        //Protects against short path error

            for (int i = 0; i < path.corners.Length - 1; i++)           //Loops through and sums total distance between path corners
            {
                totalPathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return totalPathLength;     //RETURNS TOTAL LENGTH
        }


        public void MoveTo(Vector3 destination,float speedFraction)
        {

            if (NavMeshAgent.isOnNavMesh)
            {
                if (NavMeshAgent.isStopped)
                {
                    NavMeshAgent.isStopped = false;
                }
            }

            //Sets NavMeshAgents destination to Vector3 Destination && makes sure agents.isStopped = false -> Moves character
            NavMeshAgent.enabled = true;
            NavMeshAgent.destination = destination;
            NavMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            NavMeshAgent.isStopped = false;

        }

        public void Cancel()
        {
            //Sets Agent.isStopped = true -> Stop character
            NavMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            //Tells animator your moving foward at X units istead of global works better for animators.
            Vector3 velocity = NavMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);  // Takes the global Vector3 position & turns into local
            float speed = localVelocity.z;

            animator.SetFloat("fowardSpeed", speed); //Passes speed value into Animator blend or fowardspeed value
        }

        public object CaptureState()        //Notice return type is object
        {                   
            //All things captured by method have to be marked as Serializable E.G System.Serializable on class
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.eulerAngles);

            return data;
        }                                                      //Directly passed via objects

        public void RestoreState(object state)      //Notice parameter is object
        {
            //Restores state upon load
            MoverSaveData data = (MoverSaveData)state;   //------------------------------- WATCH FOR ERROR IF SO USE OTHER CAST METHOD

            NavMeshAgent.enabled = false;

            transform.position = data.position.ToVector();                      //Sets our movers position back to saved value
            transform.eulerAngles = data.rotation.ToVector();                   //Sets our movers position back to saved value

            NavMeshAgent.enabled = true;                                        //Stops navmeshagent from messing with us chaning position

        }

        public void Teleport(Vector3 destination)
        {
            //Extremely primitive teleport. Use for Dev purposes mainly. Want to implement better teleports based on inventory.
            NavMeshAgent.enabled = false;
            transform.position = destination;
            NavMeshAgent.enabled = true;
        }

        [System.Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }
    }
}
