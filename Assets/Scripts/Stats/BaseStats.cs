using GameDevTV.Utils;
using System;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 100)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] public CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] bool shouldUseModifiers = false;

        public event Action onLevelUp;
        private LazyValue<int> currentLevel;
        private Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();                 //Get the Experience component connected to gameobject
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();                                    //ONSTART CALL CALCULATE LEVEL TO FIGURE OUT GAMEOBJECTS current level
        }

        private void OnEnable() //Called around same time as awake but always after
        {
            if (experience != null)                                             //if we have an experience object
            {
                experience.onExperienceGained += UpdateLevel;                   //ADDS UPDATELEVEL TO LIST OF METHODS HELD IN EVENTACTION
            }
        }

        private void OnDisable()
        {
            if (experience != null)                                             //if we have an experience object
            {
                experience.onExperienceGained -= UpdateLevel;                   //DROPS UPDATELEVEL TO LIST OF METHODS HELD IN EVENTACTION IN CASE IT IS DISABLED? NO CALLBACKS WHILE DISBALED
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();                                    //Gets value from calculate levels

            if (newLevel > currentLevel.value)                                        //Check if we leveled up
            {
                currentLevel.value = newLevel;
                LevelUpEfffect();           //Level up particle effect
                onLevelUp();                //Calls level up event action
            }

        }

        private float GetBaseStat(Stat stat)       //RETURNS THE PLAYERS progressional damage
        {
            return progression.GetStats(stat, characterClass, GetLevel());
        }

        public float GetStat(Stat stat)                                                
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);       //Sums the base damage stat + weapon additive modifiers
        }

        public float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;          //STOPS ENEMIES FROM GETTING ADDITIVE MODIFIERS

            float modifierTotal = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())          //We collect all of the Damage modifier providers
            {
                foreach (float modifiers in provider.GetAdditiveModifiers(stat))         //Calls the providers Interface method which gets the damage modifier value
                {
                    modifierTotal += modifiers;
                }   
            }

            return modifierTotal;           //returns total modifier amount (float)
        }

        public float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;         //STOPS ENEMIES FROM GETTING ADDITIVE MODIFIERS

            float modifierTotal = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())          //We collect all of the Damage modifier providers
            {
                foreach (float modifiers in provider.GetPercentageModifiers(stat))         //Calls the providers Interface method which gets the damage modifier value
                {
                    modifierTotal += modifiers;
                }
            }

            return modifierTotal;           //returns total modifier amount (float)
        }

        public int CalculateLevel()                                             //Calculates gameobjects level based upon Expereience component.
        {
            Experience experience = GetComponent<Experience>();                 //Gets experience component of gameobject

            if (experience == null) return startingLevel;                       //if we are an enemy we stop here


            float currentXP = GetComponent<Experience>().GetPoints();                                           //Gets current XP value from Experience component
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);            //is level before max level 

            for (int levels = 1; levels <= penultimateLevel; levels++)                                          //Loops through all levels until = to penultimateLevel
            {
                float XPToLevelUp = progression.GetStats(Stat.ExperienceToLevelUp, characterClass, levels);    //Gets the XP of nEXT LEVEL

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

        public int GetLevel()
        {
            return currentLevel.value;                                                //returns currentLevel already initialized because of using lazy values
        }
    }
}
