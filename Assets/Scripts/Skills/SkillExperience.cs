using System;
using System.Collections.Generic;
using GameDevTV.Saving;
using UnityEngine;

namespace RPG.Skill
{
    public class SkillExperience : MonoBehaviour, ISaveable
    {
        [SerializeField] public SkillHolder[] SkillXpDisplayList = null;
        private Dictionary<Skill, SkillHolder> skillXpList = null;

        private Skill currentSkill;

        //public delegate void ExperienceGainedDelegate();                      //Expereinced gained action / delegate
        public event Action onAttackExperienceGained;                                 //Expereinced gained action / delegate  -> ACTION IS A EVENT DELAGATE WITH NO RETURN TYPE\
        public event Action onStrengthExperienceGained;                                 //Expereinced gained action / delegate  -> ACTION IS A EVENT DELAGATE WITH NO RETURN TYPE
        public event Action onDefenceExperienceGained;                                 //Expereinced gained action / delegate  -> ACTION IS A EVENT DELAGATE WITH NO RETURN TYPE
        public event Action onArcheryExperienceGained;                                 //Expereinced gained action / delegate  -> ACTION IS A EVENT DELAGATE WITH NO RETURN TYPE
        public event Action onMagicExperienceGained;                                 //Expereinced gained action / delegate  -> ACTION IS A EVENT DELAGATE WITH NO RETURN TYPE
        public event Action onWoodcuttingExperienceGained;
        public event Action onMiningExperienceGained;

        private CombatSkill combat;

        public void Awake()
        {
            combat = GetComponent<CombatSkill>();
        }
        
        public void Start()
        {
            BuildList();
        }

        private void BuildList()
        {
            if (skillXpList != null) return;

            skillXpList = new Dictionary<Skill, SkillHolder>();

            foreach (Skill skill in Enum.GetValues(typeof(Skill)))
            {
                skillXpList.Add(skill, new SkillHolder(skill));
            }

            BuildArrayList();
        }

        private void BuildArrayList()
        {

            if (SkillXpDisplayList.Length != 0) return;

            SkillXpDisplayList = new SkillHolder[Enum.GetValues(typeof(Skill)).Length];

            for (int i = 0; i < SkillXpDisplayList.Length; i++)
            {
                SkillXpDisplayList[i] = new SkillHolder((Skill)i);
            }
        }

        private void AssignSkillArray()
        {
            if (SkillXpDisplayList.Length == 0)
                SkillXpDisplayList = new SkillHolder[skillXpList.Count];

            for (int i = 0; i < SkillXpDisplayList.Length; i++)
            {
                SkillXpDisplayList[i] = (skillXpList[(Skill)i]);
            }
        }

        public void GainCombatExperience(float exp, GameObject instigator)                       //Handles experience gained event
        {
            BuildList();

            Skill rewardSkill = combat.ReturnSkillType();

            skillXpList[rewardSkill].skillExperience += exp;
            InvokeCorrectAction(rewardSkill);
        }

        public void GainSkillExperience(float exp, GameObject instigator, Skill skill)
        {
            BuildList();

            skillXpList[skill].skillExperience += exp;
            InvokeCorrectAction(skill);
        }

        private void InvokeCorrectAction(Skill skill)
        {
            if (skill == Skill.Attack) onAttackExperienceGained();
            if (skill == Skill.Strength) onStrengthExperienceGained();
            if (skill == Skill.Defence) onDefenceExperienceGained();
            if (skill == Skill.Archery) onArcheryExperienceGained();
            if (skill == Skill.Magic) onMagicExperienceGained();
            if (skill == Skill.Woodcutting) onWoodcuttingExperienceGained();
            if (skill == Skill.Mining) onMiningExperienceGained();

            AssignSkillArray();
        }


        public float GetPoints(Skill skill)
        {
            if (skillXpList == null)
            {
                BuildList();
            }

            return skillXpList[skill].skillExperience;
        }

        public object CaptureState()
        {
            return skillXpList;
        }

        public void RestoreState(object state)
        {
            skillXpList = state as Dictionary<Skill, SkillHolder>;
        }

        [System.Serializable]
        public class SkillHolder
        {
            public Skill skill;
            public float skillExperience;

            public SkillHolder(Skill skill, float exp)
            {
                this.skill = skill;
                skillExperience = exp;
            }

            public SkillHolder(Skill skill)
            {
                this.skill = skill;
                skillExperience = 0;
            }
        }

        [System.Serializable]
        public class SkillClass
        {
            public SkillHolder holder;

            public SkillClass(SkillHolder skillHolder)
            {
                holder = skillHolder;
            }
        }
    }
}

