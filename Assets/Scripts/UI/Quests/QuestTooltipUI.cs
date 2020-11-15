﻿using RPG.Quest;
using TMPro;
using UnityEngine;


namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Transform objectiveContainer;
        [SerializeField] private GameObject objectivePrefab;
        [SerializeField] private GameObject objectiveIncompletePrefab;
        [SerializeField] private TextMeshProUGUI rewardText;

        public void Setup(QuestStatus status)
        {
            Quest.Quest quest = status.GetQuest();
            title.text = quest.GetTitle();

            foreach (Transform child in objectiveContainer)
            {
                Destroy(child.gameObject);   
            }

            foreach (var objective in quest.GetObjectives())
            {
                GameObject prefab = objectiveIncompletePrefab;

                if (status.IsObjectiveComplete(objective.reference))
                {
                    prefab = objectivePrefab;
                }
                    
                GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);
                TextMeshProUGUI objectiveText = objectiveInstance.GetComponentInChildren<TextMeshProUGUI>();
                objectiveText.text = objective.description;
            }

            rewardText.text = GetRewardText(quest);
        }

        private string GetRewardText(Quest.Quest quest)
        {
            string rewardText = "";

            foreach (var  reward in quest.GetRewards())
            {
                if (rewardText != "")
                {
                    rewardText += ", ";
                }

                if (reward.number > 1)
                {
                    rewardText += reward.number + " ";
                }
                rewardText += reward.item.GetDisplayName();
            }

            if (rewardText == "")
            {
                rewardText = "No reward, Complain to devteam";
            }

            rewardText += ".";
            return rewardText;
        }
    }
}




