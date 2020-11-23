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

        private void OnEnable() //Called around same time as awake but always after
        {
            if (skillHandler != null)                                             //if we have an experience object
            {
                skillHandler.onLevelUp += SetupSkill;
            }
        }

        private void OnDisable()
        {
            if (skillHandler != null)                                             //if we have an experience object
            {
                skillHandler.onLevelUp -= SetupSkill;
            }
        }

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
            ClearText();

            int Level = skillHandler.GetLevelFromList(skill);
            int MaxLevel = skillHandler.GetMaxSkillLevel(skill) + 1;

            skillName.text = skill.ToString();
            skillLevel.text =  "Level: " + Level.ToString() + "/" + MaxLevel;
        }

        private void ClearText()
        {
            skillName.text = null;
            skillLevel.text = null;
        }
    }
}

