using GameDevTV.Utils;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private float chaseDistance = 5f;                  //The distance the AI will begin chasing the player
        [SerializeField] private float suspicionTime = 3f;                  //How long after "Losing" sight of player before player returns to guard position
        [SerializeField] private float aggroCooldownTime = 3f;              //aggro CoolDown time?
        [SerializeField] private float waypointTolerence = 1f;              //How close the ai gets to waypoint before stopping
        [SerializeField] private float waypointDwellTime = 3f;              //How long ai waits at waypoint

        [Range(0, 1)]                                                       //WHATEVER Patrol speed fraction can only be between 0 & 1
        [SerializeField] private float patrolSpeedFraction = 0.2f;          //percentage of maxSpeed ai uses during patrolling behavior
        [SerializeField] private float shoutDistance = 5f;                  //how far the player can show to other ai to alert them

        private LazyValue <Vector3> guardPosition;                          //Guard position e.g. initial position
        private float timeSinceLastSawPlayer = Mathf.Infinity;              //time since ai last saw player
        private int currentWaypointIndex = 0;                               //Waypoint tracker
        private float timeSinceArrivedAtWaypoint = Mathf.Infinity;          //waypoint dwell tracker
        private float timeSinceAggrevated = Mathf.Infinity;                 //aggrevated timer tracker

        //Class References
        private Fighter fighter;
        private Health health;
        private GameObject player;
        private Mover mover;
        private ActionScheduler actionScheduler;


        private void Awake()
        {
            fighter = GetComponent<Fighter>();            //Gets AI fighter component
            health = GetComponent<Health>();             //Gets AI health Compononent
            player = GameObject.FindWithTag("Player");   // Sets the player target using tags
            mover = GetComponent<Mover>();                // Gets AI mover component
            actionScheduler = GetComponent<ActionScheduler>();

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
            actionScheduler.CancelCurrentAction();
        }

        private void AttackBehavior()
        {
            //Attack State Calls attack method and aggrevates nearby enemies based upon shout distance
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);

            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            //Cast a sphere of diameter shout distance and aggrevates all other enemy ai that it hits.
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
