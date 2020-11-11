using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Combat;
using UnityEngine;

namespace RPG.Skill
{
    public class CombatSkill : MonoBehaviour
    {

        [SerializeField] WeaponConfig currentWeapon;
        [SerializeField] private Skill currentCombatSkill;

        private Fighter fighter = null;
        private Equipment equipment = null;


        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            equipment = GetComponent<Equipment>();
        }

        private void Start()
        {
            currentWeapon = fighter.currentWeaponConfig;
        }

        private void OnEnable() //Called around same time as awake but always after
        {
            if (equipment != null)                                             //if we have an experience object
            {
                equipment.equipmentUpdated += SetCurrentWeapon;
            }
        }
        private void OnDisable() //Called around same time as awake but always after
        {
            if (equipment != null)                                             //if we have an experience object
            {
                equipment.equipmentUpdated -= SetCurrentWeapon;
            }
        }

        public IEnumerable ReturnSkillType()
        {
            List<Skill> skill = new List<Skill>();

            SetCurrentWeapon();
            SetCurrentSkill();

            skill.Add(currentCombatSkill);
            return skill;
        }

        private void SetCurrentSkill()
        {

            if (currentWeapon.weaponType == WeaponType.Attack)
            {
                currentCombatSkill = Skill.Attack;
            }

            if (currentWeapon.weaponType == WeaponType.Strength)
            {
                currentCombatSkill = Skill.Strength;
            }

            if (currentWeapon.weaponType == WeaponType.Defence)
            {
                currentCombatSkill = Skill.Defence;
            }

            if (currentWeapon.weaponType == WeaponType.Archery)
            {
                currentCombatSkill = Skill.Archery;
            }

            if (currentWeapon.weaponType == WeaponType.Sorcery)
            {
                currentCombatSkill = Skill.Magic;
            }
        }

        public void SetCurrentWeapon()
        {
            currentWeapon = fighter.currentWeaponConfig;
        }

        public Skill GetCurrentSkill()
        {
            return currentCombatSkill;
        }
    }
}
