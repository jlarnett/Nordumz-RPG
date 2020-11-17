using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

public class WoodCutter : MonoBehaviour, IAction
{
    [SerializeField] private int treeBufferDistance = 2;

    private int runSpeed = 1;
    private Harvest target;
    private Mover mover;
    private Animator animator;

    void Awake()
    {
        mover = GetComponent<Mover>();
        animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        if (target != null && !GetInRange(target.transform) )
        {
            mover.MoveTo(target.transform.position, runSpeed);               //Passes in Agent speed of attack.
        }
        else
        {
            mover.Cancel();
            LoggerBehavior();
        }

    }

    private void LoggerBehavior()
    {
        transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
        TriggerChop();
    }

    private void TriggerChop()
    {
        animator.ResetTrigger("stopChop");
        animator.SetTrigger("chop");
    }

    public void Chop(GameObject chopTarget)
    {
        GetComponent<ActionScheduler>().StartAction(this);
        target = chopTarget.GetComponent<Harvest>();
    }

    public bool CanChop(GameObject chopTarget)
    {
        if (chopTarget == null)   //If target is = null we return false and say we cant attack
        {
            return false;
        }

        if (!GetInRange(chopTarget.transform) && !chopTarget.GetComponent<Harvest>().isHarvested()) //If we can not move to that target
            return false;                                                       //Return false cant attack target out of range

        //Assigns the health component of combatTarget. and returns if false if target is null or dead.
        Harvest targetToTest = chopTarget.GetComponent<Harvest>();
        return targetToTest != null && !targetToTest.isHarvested();
    }
    
    public void Hit()
    {

        //If target is null do nothing
        if (target == null)
            return;

        int damage = 10;           //Gets the damage from stats.Damage enum table

        //This is an ANIMATION Event called from within animator not code!
        target.TakeDamage(gameObject, damage);   //WE pass in the fighters gameobject E.G. Player. && the current weapon damage
    }

    private void StopAttack()
    {
        animator.ResetTrigger("chop");        //resetting attack trigger before canceling
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
}
