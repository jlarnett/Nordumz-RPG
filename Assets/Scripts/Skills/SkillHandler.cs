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

        private float attackBonus = 0;
        private float strengthBonus = 0;
        private float DefenceBonus = 0;
        private float ArcheryBonus = 0;
        private float MagicBonus = 0;

        public event Action onLevelUp;

        private void Awake()
        {
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
                skillLookup.Add(skill, new PrimarySkill(skill));
            }
        }

        private void OnEnable() //Called around same time as awake but always after
        {
            if (experience != null)                                             //if we have an experience object
            {
                experience.onAttackExperienceGained += UpdateCurrentLevel; //ADDS UPDATELEVEL TO LIST OF METHODS HELD IN EVENTACTION
                experience.onStrengthExperienceGained += UpdateCurrentLevel; //ADDS UPDATELEVEL TO LIST OF METHODS HELD IN EVENTACTION
                experience.onDefenceExperienceGained += UpdateCurrentLevel;
                experience.onArcheryExperienceGained += UpdateCurrentLevel;
                experience.onMagicExperienceGained += UpdateCurrentLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)                                             //if we have an experience object
            {
                experience.onAttackExperienceGained -= UpdateCurrentLevel;                   //DROPS UPDATELEVEL TO LIST OF METHODS HELD IN EVENTACTION IN CASE IT IS DISABLED? NO CALLBACKS WHILE DISBALED
                experience.onStrengthExperienceGained -= UpdateCurrentLevel; //ADDS UPDATELEVEL TO LIST OF METHODS HELD IN EVENTACTION
                experience.onDefenceExperienceGained -= UpdateCurrentLevel;
                experience.onArcheryExperienceGained -= UpdateCurrentLevel;
                experience.onMagicExperienceGained -= UpdateCurrentLevel;
            }
        }

        public int CalculateLevel(Skill skill)                                             //Calculates gameobjects level based upon Expereience component.
        {
            SkillExperience experience = GetComponent<SkillExperience>();                 //Gets experience component of gameobject
            if (experience == null) return startingLevel;                       //if we are an enemy we stop here

            float currentXP = GetComponent<SkillExperience>().GetPoints(skill);                                           //Gets current XP value from Experience component
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

        private void UpdateCurrentLevel()
        {
            if (GetComponent<CombatSkill>() == null) return;
            currentSkill = GetComponent<CombatSkill>().GetCurrentSkill();

            UpdateLevel(currentSkill);
        }

        private void UpdateLevel(Skill currentSkill)
        {
            int newLevel = CalculateLevel(currentSkill);

            if (newLevel > GetLevelFromList(currentSkill))
            {
                UpdateSkillLevelList(currentSkill, newLevel);
                LevelUpEfffect();
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
                for (int i = 0; i < VARIABLE.Value.skillLevel; i++)
                {
                    if (VARIABLE.Key == Skill.Attack)
                    {
                        attackBonus += 2;
                    }
                    if (VARIABLE.Key == Skill.Strength)
                    {
                       strengthBonus += 2;
                    }
                    if (VARIABLE.Key == Skill.Defence)
                    {
                        DefenceBonus += 2;
                    }
                    if (VARIABLE.Key == Skill.Archery)
                    {
                        ArcheryBonus += 2;
                    }
                    if (VARIABLE.Key == Skill.Magic)
                    {
                        MagicBonus += 2;
                    }
                }
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
    }
}

