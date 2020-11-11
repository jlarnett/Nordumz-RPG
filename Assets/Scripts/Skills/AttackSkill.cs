using System.Collections;
using System.Collections.Generic;
using RPG.Skill;
using UnityEngine;

public class AttackSkill : ISkill
{
    const Skill skill = Skill.Attack;
    private int skillLevel = 1;

    public float CalculateBonus()
    {
        return 0;
    }

    public void CalculateLevel()
    {
    }

    public void GainExperience()
    {

    }
}
