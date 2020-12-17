using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Control;
using UnityEngine;
using Random = System.Random;

namespace RPG.Skill 
{
    [CreateAssetMenu(fileName = "Harvest", menuName = "Harvestable/newItem", order = 2)]  //Scriptable object!
    public class Harvestable : ScriptableObject
    {
        private Dictionary<Skill, ISkiller> harvestableSkillList;

        //name
        [SerializeField] private string harvestableName;                //Harvestable Name\

        //Health and resource respawn time
        [SerializeField] private float startingHealth = 10;
        [SerializeField] private float respawnTime = 5;

        //Rewards
        [SerializeField] private float harvestExpReward = 50;
        [SerializeField] private int maxResourcesRewarded = 1;
        [SerializeField] private ActionItem resourceRewardItem;
        [SerializeField] private Skill rewardSkill;

        //cursorType
        [SerializeField] private CursorType cursorType = CursorType.Woodcutting;


        private void BuildList(PlayerController playerController)
        {
            if (harvestableSkillList != null) return;

            harvestableSkillList = new Dictionary<Skill, ISkiller>();

            foreach (Skill skill in Enum.GetValues(typeof(Skill)))
            {
                if (skill == Skill.Woodcutting)
                {
                    harvestableSkillList[skill] = playerController.GetComponent<WoodCutter>();
                }

                if (skill == Skill.Mining)
                {
                    harvestableSkillList[skill] = playerController.GetComponent<Miner>();
                }
            }
        }

        public void AwardHarvestableReward(GameObject instigator)
        {
            System.Random random = new Random();
            int randomResource = random.Next(maxResourcesRewarded);


            Inventory playerInventory = instigator.GetComponent<Inventory>();
            playerInventory.AddToFirstEmptySlot(resourceRewardItem, randomResource);
        }

        public void Cancel(GameObject instigator)
        {
            if (rewardSkill == Skill.Woodcutting)
                instigator.GetComponent<WoodCutter>().Cancel();

            if (rewardSkill == Skill.Mining)
                instigator.GetComponent<Miner>().Cancel();
        }

        public bool CanActuate(GameObject target, PlayerController playerController)
        {
            BuildList(playerController);
            return harvestableSkillList[rewardSkill].CanAction(target);
        }

        public void Actuate(GameObject target, PlayerController playerController)
        {
            BuildList(playerController);
            harvestableSkillList[rewardSkill].Action(target);
        }

        public float GetExperienceReward()
        {
            return harvestExpReward;
        }

        public Skill GetSkill()
        {
            return rewardSkill;
        }

        public float GetStartingHealth()
        {
            return startingHealth;
        }

        public float GetRespawnTime()
        {
            return respawnTime;
        }

        public CursorType GetHarvestCursor()
        {
            return cursorType;
        }
    }
}
