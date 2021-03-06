﻿using RPG.Core;
using RPG.Movement;
using UnityEngine;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using System;
using GameDevTV.Inventories;
using GameDevTV.Saving;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] private float timeBetweenAttacks = 1f;                     //Seconds between attacks
        [SerializeField] private float attackSpeedFraction = 1f;                    //Determines how fast fighter runs at target once Attack() is initiated 1 = max 
        [SerializeField] private Transform rightHandTransform = null;               //Gives our fighter the concept of using 2hands for animations
        [SerializeField] private Transform leftHandTransform = null;                //Left hand transform
        [SerializeField] private WeaponConfig defaultWeapon = null;

        //Timer and weapon for saving
        private float timeSinceLastAttack = Mathf.Infinity;                             //time since last attack that throttles attack animation.
        private LazyValue <Weapon> currentWeapon;                                       //Weapon component

        //Classes Reference
        private Health target;                                              //Health component of target for attack
        private Mover mover;
        private Equipment equipment;
        private Animator animator;
        private WeaponConfig currentWeaponConfig;                           //LazyValue for weapon config to make it serailizable?
        private BaseStats baseStats;
        private ActionScheduler actionScheduler;

        public void Awake()
        {
            //Initalize components
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();                          
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            equipment = GetComponent<Equipment>();
            baseStats = GetComponent<BaseStats>();
            actionScheduler = GetComponent<ActionScheduler>();

            if (equipment)      //we subscribe to equipment updated event & if we have got equipment call updateWeapon
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }


        public void Start()
        {
            currentWeapon.ForceInit();              //
            //currentWeaponConfig.ForceInit(); //force initalize weapon IMPORTANT 
        }

        private void Update()
        {
            //Just checks that target is null or dead. If we have a target but not in range we go too them and when in range we attack behavior
            timeSinceLastAttack += Time.deltaTime;                          //Vital to movement makes sure we dont stop every frame

            if (target == null) return;                                     //If target is null do nothing in Update

            if (target.IsDead())                                            //If target is dead do nothing & return / stop attacking
            {
                return;
            }

            //If we are not in range keep moving OR else stop moving and then begin attack animation / event.
            if (target != null && !GetInRange(target.transform))
            {
                mover.MoveTo(target.transform.position, attackSpeedFraction);               //Passes in Agent speed of attack.
            }
            else
            {
                mover.Cancel();                                                             //Cancels moving behavior when in range
                AttackBehavior();                                                           //Calls upon attack behavior
            }
        }

        public Health GetTarget()           //Returns the fighters Health Target. to EnemyHealthDisplay
        {
            return target;
        }

        public WeaponConfig GetCurrentWeaponConfig()
        {
            return currentWeaponConfig;
        }

        private void AttackBehavior()
        {
            //Turns fighter towawrds enemy before attacking
            transform.LookAt(target.transform);

            //Checks time between attacks and then triggers attack animation in animator
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                //This will trigger the hit() event
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            animator.ResetTrigger("stopAttack");      //FIRST BUG FIX WEIRD BOOL ISSUE WITH TRIGGERS RESET ATTACK BEFORE ATTACKING.
            animator.SetTrigger("attack");
        }

        void Hit()
        {

            //If target is null do nothing
            if (target == null)
                return;

            float damage = baseStats.GetStat(Stat.Damage);           //Gets the damage from stats.Damage enum table

            if (currentWeapon.value != null)                //We check current weapons value
            {
                currentWeapon.value.OnHit();
            }

            if (currentWeaponConfig.HasProjectile())          //checks if current weapon has projectile
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage); // if has projectile then launch projectile     //pass in fighter gameobject and damage from stats enum table
            }
            else
            {
                //This is an ANIMATION Event called from within animator not code!
                target.TakeDamage(gameObject, damage);   //WE pass in the fighters gameobject E.G. Player. && the current weapon damage
            }
        }

        void Shoot()    //Bow animation calls this so it calls hit
        {
            Hit();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null)   //If target is = null we return false and say we cant attack
            {
                return false;
            }

            if (!mover.CanMoveTo(combatTarget.transform.position) && !GetInRange(combatTarget.transform)) //If we can not move to that target
                return false;                                                       //Return false cant attack target out of range

            //Assigns the health component of combatTarget. and returns if false if target is null or dead.
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            //Calls schedule to start Attack() action / cancel old action  && assigns target = target = combatTarget
            actionScheduler.StartAction(this);
            target = combatTarget.transform.GetComponent<Health>();
        }

        public void Cancel()
        {
            //Cancel target to null && cancels fighters mover also
            target = null;
            mover.Cancel();
        }

        public void EquipWeapon(WeaponConfig weapon)                                                              //When someone calls equipweapon we need to know What weapon scriptable object
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        //Private methods---------------------------------------------------------------------------------------------------------------------------------------------------

        private void UpdateWeapon()
        {
            var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;     //We get weapon from weapon slot and cast as WeaponConfig
            //Equip Weapon
            if (weapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
            else
            {
                EquipWeapon(weapon);
            }
        }

        private void StopAttack()
        {
            animator.ResetTrigger("attack");        //resetting attack trigger before canceling
            animator.SetTrigger("stopAttack");
        }

        private bool GetInRange(Transform targetTransfrom)
        {
            //Returns bool about whether we are in distance between fighter gameobject and target
            return Vector3.Distance(transform.position, targetTransfrom.position) < currentWeaponConfig.GetRange();
        }

        private Weapon SetupDefaultWeapon()
        {
            //Spawns weapon & the default weapon and returns it too current weapon
            return AttachWeapon(defaultWeapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            //Attaches weapon too player along with getting animator component
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);      //spawns weapon in players hand
        }

        //Save methods -----------------------------------------------------------------------------------------------------------------------------------------------------------
        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string) state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);         //Weaponname
            EquipWeapon(weapon);
        }

    }
}
