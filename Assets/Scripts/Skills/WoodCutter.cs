using RPG.Core;
using RPG.Movement;
using UnityEngine;


namespace RPG.Skill
{
    public class WoodCutter : MonoBehaviour, IAction, ISkiller
    {
        [SerializeField] private int treeBufferDistance = 2;

        private int runSpeed = 1;
        private Harvest target;
        private Mover mover;
        private Animator animator;
        private ActionScheduler actionScheduler;

        void Awake()
        {
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
        }


        // Update is called once per frame
        void Update()
        {
            if (target == null) return;

            if (target != null && !GetInRange(target.transform))
            {
                mover.MoveTo(target.transform.position, runSpeed); //Passes in Agent speed of attack.
            }
            else
            {
                mover.Cancel();
                LoggerBehavior();
            }

        }

        private void LoggerBehavior()
        {
            transform.LookAt(
                new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
            TriggerChop();
        }

        private void TriggerChop()
        {
            animator.ResetTrigger("stopChop");
            animator.SetTrigger("chop");
        }


        public void Hit()
        {

            //If target is null do nothing
            if (target == null)
                return;

            int damage = 10; //Gets the damage from stats.Damage enum table

            //This is an ANIMATION Event called from within animator not code!
            target.TakeDamage(gameObject,
                damage); //WE pass in the fighters gameobject E.G. Player. && the current weapon damage
        }

        private void StopAttack()
        {
            animator.ResetTrigger("chop"); //resetting attack trigger before canceling
            animator.SetTrigger("stopChop");
        }

        public void Cancel()
        {
            target = null;
            StopAttack();
            mover.Cancel();

        }

        private bool GetInRange(Transform targetTransfrom)
        {
            //Returns bool about whether we are in distance between fighter gameobject and target
            return Vector3.Distance(transform.position, targetTransfrom.position) < treeBufferDistance;
        }

        public bool CanAction(GameObject target)
        {
            if (target == null) //If target is = null we return false and say we cant attack
            {
                return false;
            }

            if (!GetInRange(target.transform) && !target.GetComponent<Harvest>().isHarvested())
                return false; //Return false cant attack target out of range

            //Assigns the health component of combatTarget. and returns if false if target is null or dead.
            Harvest targetToTest = target.GetComponent<Harvest>();
            return targetToTest != null && !targetToTest.isHarvested();
        }

        public void Action(GameObject actiontarget)
        {
            actionScheduler.StartAction(this);
            target = actiontarget.GetComponent<Harvest>();
        }
    }
}
