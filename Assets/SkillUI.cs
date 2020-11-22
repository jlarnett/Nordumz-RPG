using System.Collections;
using System.Collections.Generic;
using RPG.Skill;
using TMPro;
using UnityEngine;


namespace RPG.UI
{
    public class SkillUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI skillName;
        [SerializeField] private TextMeshProUGUI skillLevel;

        [SerializeField] private Skill.Skill skill;

        private SkillHandler skillHandler;

        void Awake()
        {
            skillHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<SkillHandler>();
        }

        void Start()
        {
            SetupSkill();
        }

        private void SetupSkill()
        {
            int Level = skillHandler.GetLevelFromList(skill);

            skillName.text = skill.ToString();
            skillLevel.text = Level.ToString();
        }
    }
}

