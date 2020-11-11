using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] public ProgressionCharacterClass[] CharacterClasses = null;                //We have a Array of progression character classes.
        private Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;          //PERFORMANCE  //Create a nested dictionary lookups CharacterClass First //Secondary lookup is Stat, then LEVEL int or float

        public float GetStats(Stat stat, CharacterClass characterClass, int level)                 //Gets the stats
        {
            BuildLookup();                                          //PERFORMANCE

            float[] levels = lookupTable[characterClass][stat];     //USING LOOKUP TABLE ALL IT TAKES IS INSERTING characterclass, stat

            if (levels.Length < level)
            {
                return 0;                                           //If we dont have a level for this we return 0
            }

            return levels[level - 1];
        }

        private void BuildLookup()
        {
            if (lookupTable != null) return;                        //Builds lookup tabel only if it hasn't been built before
                                                                    //This gives saving to table
            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();             //A new empty dictionary of type

            foreach (ProgressionCharacterClass progressionClass in CharacterClasses)                //Iterate through CharacterClasses array that holdes classes & stats        PERFORMANCE
            {
                var statLookupTable = new Dictionary<Stat, float[]>();                             //Create new statlookuptable

                foreach (ProgressionStat progressionStat in progressionClass.stats)                 //Foreach stat with that class          PERFORMANCE
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;                 //Assigns statlookup table at each stat and assign the value based uppon levels     //performance
                }

                lookupTable[progressionClass.characterClass] = statLookupTable;                     //Foreach of the progressionClass items assign the lookupTable at each characterclass and assin it = statlookup dictionary
            }
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();                                                  //Double check lookup has been built. Cant default cause of check in method

            float[] levels = lookupTable[characterClass][stat];             //Gets the[Array of levels from lookup
            return levels.Length;
        }


        [System.Serializable]
        public class ProgressionCharacterClass                              //We have a character class & Array of ProgressionSTATs that each have a Stat & array of floats for said stat
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
            //public float[] health;
        }

        [System.Serializable]
        public class ProgressionStat                                        // PROGRESSIONSTAT HOLDS Stat item & Array of levels
        {
            public Stat stat;
            public float[] levels;
        }
    }
}
