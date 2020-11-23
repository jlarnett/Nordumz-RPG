using System;
using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;

namespace RPG.Skill
{
    public class SkillHandler : MonoBehaviour, IModifierProvider
    {

        [Range(1, 100)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] private SkillProgression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] bool shouldUseModifiers = false;

        //------------------------------------------------------------------
        private Dictionary<Skill, PrimarySkill> skillLookup = null;

        private SkillExperience experience;
        private Skill currentSkill;
        private CombatSkill combatSkill;

        private float attackBonus = 0;
        private float strengthBonus = 0;
        private float DefenceBonus = 0;
        private float ArcheryBonus = 0;
        private float MagicBonus = 0;

        public event Action onLevelUp;

        private void Awake()
        {

            combatSkill = GetComponent<CombatSkill>();
            experience = GetComponent<SkillExperience>();                 //Get the Experience component connected to gameobject
        }

        private void Start()
        {
            BuildList();
        }

        private void BuildList()
        {
            if (skillLookup != null) return;

            skillLookup = new Dictionary<Skill, PrimarySkill>();

            foreach (Skill skill in Enum.GetValues(typeof(Skill)))
            {
                skillLookup.Add(skill, new PrimarySkill(skill, CalculateLevel(skill)));
            }
        }

        #region EventHookups

        private void OnEnable() //Called around same time as awake but always after
        {
            if (experience != null)                                             //if we have an experience object
            {
                experience.onAttackExperienceGained += UpdateCombatLevel; //ADDS UPDATELEVEL TO LIST OF METHODS HELD IN EVENTACTION
                experience.onStrengthExperienceGained += UpdateCombatLevel; //ADDS UPDATELEVEL TO LIST OF METHODS HELD IN EVENTACTION
                experience.onDefenceExperienceGained += UpdateCombatLevel;
                experience.onArcheryExperienceGained += UpdateCombatLevel;
                experience.onMagicExperienceGained += UpdateCombatLevel;

                experience.onWoodcuttingExperienceGained += UpdateSkillLevel;
                experience.onMiningExperienceGained += UpdateSkillLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)                                             //if we have an experience object
            {
                experience.onAttackExperienceGained -= UpdateCombatLevel;                   //DROPS UPDATELEVEL TO LIST OF METHODS HELD IN EVENTACTION IN CASE IT IS DISABLED? NO CALLBACKS WHILE DISBALED
                experience.onStrengthExperienceGained -= UpdateCombatLevel; //ADDS UPDATELEVEL TO LIST OF METHODS HELD IN EVENTACTION
                experience.onDefenceExperienceGained -= UpdateCombatLevel;
                experience.onArcheryExperienceGained -= UpdateCombatLevel;
                experience.onMagicExperienceGained -= UpdateCombatLevel;

                experience.onWoodcuttingExperienceGained -= UpdateSkillLevel;
                experience.onMiningExperienceGained -= UpdateSkillLevel;
            }
        }
        
        #endregion

        public int CalculateLevel(Skill skill)                                             //Calculates gameobjects level based upon Expereience component.
        {
            if (experience == null) return startingLevel;                       //if we are an enemy we stop here

            float currentXP = experience.GetPoints(skill);                                           //Gets current XP value from Experience component
            int penultimateLevel = progression.GetLevels(skill);            //is level before max level 

            for (int levels = 1; levels <= penultimateLevel; levels++)                                          //Loops through all levels until = to penultimateLevel
            {
                float XPToLevelUp = progression.GetStats(skill, levels);    //Gets the XP of nEXT LEVEL

                if (XPToLevelUp > currentXP)                            //keeps checking until currentXP is less than next levels xp
                {
                    //return current level 
                    return levels;                                      //returns the players level
                }
            }

            return penultimateLevel + 1;                                //return max level if makes it this fare
        }

        public int GetMaxSkillLevel(Skill skill)
        {
            return progression.GetLevels(skill);
        }

        //--------------------------------------------------------Effects & Getters--------------------------------------------
        private void LevelUpEfffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        //=
        public int GetLevelFromList(Skill skill)
        {
            BuildList();
            return skillLookup[skill].skillLevel;
        }

        //-------------------------------------------------------------tEST CODE

        private void UpdateCombatLevel()
        {
            if (combatSkill == null) return;
            currentSkill = combatSkill.GetCurrentSkill();

            UpdateLevel(currentSkill);

            PrintLevels();
        }

        private void UpdateSkillLevel()
        {
            if (currentSkill == null) return;
            UpdateLevel(currentSkill);


            PrintLevels();
        }

        private void UpdateLevel(Skill currentSkill)
        {
            int newLevel = CalculateLevel(currentSkill);

            if (newLevel > GetLevelFromList(currentSkill))
            {
                UpdateSkillLevelList(currentSkill, newLevel);
                LevelUpEfffect();

                onLevelUp?.Invoke();
            }
        }


        public bool UpdateSkillLevelList(Skill skill, int value)
        {
            BuildList();
            skillLookup[skill].skillLevel = value;

            return true;
        }

        private void CalculateBonuses()
        {
            attackBonus = 0;
            strengthBonus = 0;
            DefenceBonus = 0;
            ArcheryBonus = 0;
            MagicBonus = 0;

            BuildList();

            foreach (var VARIABLE in skillLookup)
            {
                if (VARIABLE.Key == Skill.Attack)
                {
                    attackBonus = VARIABLE.Value.GetSkillBonus();
                }

                if (VARIABLE.Key == Skill.Strength)
                {
                    strengthBonus += VARIABLE.Value.GetSkillBonus();
                }

                if (VARIABLE.Key == Skill.Defence)
                {
                    DefenceBonus += VARIABLE.Value.GetSkillBonus();
                }

                if (VARIABLE.Key == Skill.Archery)
                {
                    ArcheryBonus += VARIABLE.Value.GetSkillBonus();
                }

                if (VARIABLE.Key == Skill.Magic)
                {
                    MagicBonus += VARIABLE.Value.GetSkillBonus();
                }
            }
        }

        private void PrintLevels()
        {
            foreach (var VARIABLE in skillLookup)
            {
                Debug.Log("Skill: " + VARIABLE.Key + " Level: " + VARIABLE.Value.skillLevel);
            }
        }
        

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                CalculateBonuses();
                yield return attackBonus;
                yield return strengthBonus;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            yield return 0;
        }

        public void SetCurrentSkill(Skill newSkill)
        {
            currentSkill = newSkill;
        }

    }
}

