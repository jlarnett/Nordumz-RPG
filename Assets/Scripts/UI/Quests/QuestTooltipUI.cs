using RPG.Quest;
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

        public void Setup(QuestStatus status)
        {
            Quest.Quest quest = status.GetQuest();
            title.text = quest.GetTitle();

            foreach (Transform child in objectiveContainer)
            {
                Destroy(child.gameObject);   
            }

            foreach (string objective in quest.GetObjectives())
            {
                GameObject prefab = objectiveIncompletePrefab;

                if (status.IsObjectiveComplete(objective))
                {
                    prefab = objectivePrefab;
                }
                    
                GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);
                TextMeshProUGUI objectiveText = objectiveInstance.GetComponentInChildren<TextMeshProUGUI>();
                objectiveText.text = objective;
            }
        }
    }
}




