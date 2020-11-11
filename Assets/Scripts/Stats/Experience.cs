using System;
using GameDevTV.Saving;
using UnityEngine;


namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] public float experiencePoints = 0;


        //public delegate void ExperienceGainedDelegate();                      //Expereinced gained action / delegate
        public event Action onExperienceGained;                                 //Expereinced gained action / delegate  -> ACTION IS A EVENT DELAGATE WITH NO RETURN TYPE

        void Awake()
        {
        }

        public void GainExperience(float exp, GameObject instigator)                       //Handles experience gained event
        {
            experiencePoints += exp;                                //adds experience
            onExperienceGained();                                   //calls method 
        }

        public object CaptureState()
        {
            return experiencePoints;
        }

        public float GetPoints()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            float savedPlayerXP = (float) state;
            experiencePoints = savedPlayerXP;
        }
    }
}

