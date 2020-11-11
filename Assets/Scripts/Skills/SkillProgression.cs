using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Skill
{
    [CreateAssetMenu(fileName = "ProgressionSkill", menuName = "Skills/New Progression", order = 0)]
    public class SkillProgression : ScriptableObject
    {
        [SerializeField] public List<ProgressionSkill> skills = null;
        private Dictionary<Skill, float[]> lookupTable = null;          //PERFORMANCE  //Create a nested dictionary lookups CharacterClass First //Secondary lookup is Stat, then LEVEL int or float


        public float GetStats(Skill skill, int level)                 //Gets the stats
        {
            BuildLookup();                                          //PERFORMANCE

            float[] levels = lookupTable[skill];     //USING LOOKUP TABLE ALL IT TAKES IS INSERTING characterclass, stat

            if (levels.Length < level)
            {
                return 0;                                           //If we dont have a level for this we return 0
            }

            return levels[level - 1];
        }

        private void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<Skill, float[]>();

            foreach (ProgressionSkill progressionSkill in skills)
            {
                lookupTable[progressionSkill.skill] = progressionSkill.levels;                     //Foreach of the progressionClass items assign the lookupTable at each characterclass and assin it = statlookup dictionary
            }
        }


        public int GetLevels(Skill skill)
        {
            BuildLookup();                                                  //Double check lookup has been built. Cant default cause of check in method

            float[] levels = lookupTable[skill];             //Gets the[Array of levels from lookup
            return levels.Length;
        }

        [System.Serializable]
        public class ProgressionSkill
        {
            public Skill skill;
            public float[] levels;

        }
    }
}

