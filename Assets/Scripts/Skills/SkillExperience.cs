using System;
using System.Collections.Generic;
using GameDevTV.Saving;
using UnityEngine;

namespace RPG.Skill
{
    public class SkillExperience : MonoBehaviour, ISaveable
    {
        [SerializeField] public SkillClass[] skillExpList = null;
        [SerializeField] public Dictionary<Skill, SkillHolder> skillXpList = null;

        private Skill currentSkill;

        //public delegate void ExperienceGainedDelegate();                      //Expereinced gained action / delegate
        public event Action onAttackExperienceGained;                                 //Expereinced gained action / delegate  -> ACTION IS A EVENT DELAGATE WITH NO RETURN TYPE\
        public event Action onStrengthExperienceGained;                                 //Expereinced gained action / delegate  -> ACTION IS A EVENT DELAGATE WITH NO RETURN TYPE
        public event Action onDefenceExperienceGained;                                 //Expereinced gained action / delegate  -> ACTION IS A EVENT DELAGATE WITH NO RETURN TYPE
        public event Action onArcheryExperienceGained;                                 //Expereinced gained action / delegate  -> ACTION IS A EVENT DELAGATE WITH NO RETURN TYPE
        public event Action onMagicExperienceGained;                                 //Expereinced gained action / delegate  -> ACTION IS A EVENT DELAGATE WITH NO RETURN TYPE
        public event Action onWoodcuttingExperienceGained;

        private CombatSkill combat = null;

        void Awake()
        {
            combat = GetComponent<CombatSkill>();
        }
        
        private void Start()
        {
            BuildList();
        }

        public void GainCombatExperience(float exp, GameObject instigator)                       //Handles experience gained event
        {
            BuildList();

            if (combat.ReturnSkillType() == null)
            {
                return;
            }

            foreach (Skill combatSkill in combat.ReturnSkillType())
            {
                BuildList();

                skillXpList[combatSkill].skillExperience += exp;
                InvokeCorrectAction(combatSkill);
            }
        }

        private void InvokeCorrectAction(Skill skill)
        {
                if (skill == Skill.Attack)
                    onAttackExperienceGained();

                if (skill == Skill.Strength)
                    onStrengthExperienceGained();

                if (skill == Skill.Defence)
                    onDefenceExperienceGained();

                if (skill == Skill.Archery)
                    onArcheryExperienceGained();

                if (skill == Skill.Magic)
                    onMagicExperienceGained();

                if (skill == Skill.Woodcutting)
                    onWoodcuttingExperienceGained();


            AssignSkillArray();
        }

        private void AssignSkillArray()
        {
            if (skillExpList == null)
                skillExpList = new SkillClass[skillXpList.Count];

            for (int i = 0; i < skillExpList.Length; i++)
            {
                skillExpList[i] = new SkillClass(skillXpList[(Skill)i]);
            }
        }

        private void BuildList()
        {
            BuildArrayList();

            if (skillXpList != null) return;

            skillXpList = new Dictionary<Skill, SkillHolder>();

            foreach (SkillClass skillClass in skillExpList)
            {
                skillXpList.Add(skillClass.holder.skill, skillClass.holder);
            }
        }

        private void BuildArrayList()
        {

            if (skillExpList != null) return;

            skillExpList = new SkillClass[Enum.GetValues(typeof(Skill)).Length];

            for(int i = 0; i < skillExpList.Length;)
            {
                skillExpList[i] = new SkillClass(new SkillHolder((Skill)i));
            }
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

