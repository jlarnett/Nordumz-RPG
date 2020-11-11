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

    public PrimarySkill(Skill skill)
    {
        this.skill = skill;
        skillLevel = 1;
    }
}
