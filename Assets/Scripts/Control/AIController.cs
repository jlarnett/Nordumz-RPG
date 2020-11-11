using GameDevTV.Utils;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using RPG.Attributes;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] public float chaseDistance = 5f;
        [SerializeField] public float suspicionTime = 3f;
        [SerializeField] public float aggroCooldownTime = 3f;
        [SerializeField] public PatrolPath patrolPath;
        [SerializeField] public float waypointTolerence = 1f;
        [SerializeField] public float waypointDwellTime = 3f;
        [Range(0, 1)]   //WHATEVE Patrol speed fraction can only be between 0 & 1
        [SerializeField] public float patrolSpeedFraction = 0.2f; //percentage of maxSpeed in Mover

        [SerializeField] public float shoutDistance = 5f;

        private Fighter fighter;
        private Health health;
        private GameObject player;
        private Mover mover;

        private LazyValue <Vector3> guardPosition;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private int currentWaypointIndex = 0;
        private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private float timeSinceAggrevated = Mathf.Infinity;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();            //Gets AI fighter component
            health = GetComponent<Health>();             //Gets AI health Compononent
            player = GameObject.FindWithTag("Player");   // Sets the player target using tags
            mover = GetComponent<Mover>();                // Gets AI mover component

            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Start()                    //Assigns fighter component of current gameobject
        {
            guardPosition.ForceInit();
        }
        private void Update()
        {

            if (health.IsDead())    //If AI Health component method health.IsDead() is true do nothing!
            {
                //Death State
                return;
            }

            if (IsAggrevated() && fighter.CanAttack(player))       //Varifies we are in range && canAttack e.g. we have health component / not dead
            {
                //Attack State
                AttackBehavior();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                //Suspcian State
                SuspicionBehavior();
            }
            else
            {
                //Patrol /Guard State
                //Move Guard back to position or waypoint
                PatrolBehavior();
            }

            UpdateTimers();
        }

        public void Aggrevate()
        {
            timeSinceAggrevated = 0;
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private void PatrolBehavior()
        {

            //Patrol / Guard State
            Vector3 nextPosition = guardPosition.value;       // defaults nextposition to guard position, but if it has patrol it patrols

            if (patrolPath != null)     //if patrol isn't null
            {
                if (AtWaypoint())               //if we are at current waypoint get next waypoint
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();        //next position = Current waypoint Vector3

                if (timeSinceArrivedAtWaypoint > waypointDwellTime)
                {
                    mover.StartMoveAction(nextPosition, patrolSpeedFraction);        //Moves to next waypoint or guard position
                }
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            //returns Vector 3 of currentWaypointIndex
            return patrolPath.GetWaypoint(currentWaypointIndex);

        }

        private void CycleWaypoint()
        {
            //Gets next waypoint from patrolPath class GetNextIndex() method.
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            //returns bool true if at current waypoint
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerence;
        }

        private void SuspicionBehavior()
        {
            //Suspicion State
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehavior()
        {
            //Attack State
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);

            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);           //Array of all hits around our enemy player from spherecast. deciding when enemies are aggroed
            //Loop over all the hits
            foreach (RaycastHit hit in hits)
            {
                //find any enemy components
                AIController ai = hit.collider.GetComponent<AIController>();

                if (ai == null) continue;

                //aggrevate those enemies
                ai.Aggrevate();
            }
        }

        private bool IsAggrevated()
        {
            //returns bool true if in range for AI to attack and pursue player.
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

            //Check aggrevated


            return distanceToPlayer < chaseDistance || timeSinceAggrevated < aggroCooldownTime;
        }

        //Called by unity for gizmos
        private void OnDrawGizmosSelected()
        {
            //Draws Radius of Enemy Aggro
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
