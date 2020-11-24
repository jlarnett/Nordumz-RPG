using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Skill;
using RPG.UI;
using UnityEngine;

public class InitializeSkillUI : MonoBehaviour
{

    [SerializeField] private GameObject skillPrefab;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Enum.GetValues(typeof(Skill)).Length; i++)
        {
            CreateSkillPrefabs((Skill)i);
        }
    }

    void CreateSkillPrefabs(Skill currentSkill)
    {
        if (skillPrefab != null)
        {
            var skill = Instantiate(skillPrefab, transform);
            skill.GetComponent<SkillUI>().SetCurrentSkill(currentSkill);
        }
    }

}
