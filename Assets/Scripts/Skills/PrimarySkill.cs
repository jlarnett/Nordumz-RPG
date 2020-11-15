using System.Collections;
using System.Collections.Generic;
using RPG.Skill;
using UnityEngine;
using UnityEngine.Analytics;

[System.Serializable]
public class PrimarySkill
{
    public Skill skill;
    public int skillLevel;
    public float skillBonus = 0;

    public float GetSkillBonus()
    {
        skillBonus = 0;

        for (int i = 0; i < skillLevel; i++)
        {
            skillBonus += 2;
        }

        return skillBonus;
    }

    public PrimarySkill(Skill skill)
    {
        this.skill = skill;
        skillLevel = 1;
    }
}
