using System.Collections;
using System.Collections.Generic;
using RPG.Quest;
using UnityEngine;

public class QuestListUI : MonoBehaviour
{
    [SerializeField] private Quest[] tempQuest;
    [SerializeField] private QuestItemUI questPrefab;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Quest quest in tempQuest)
        {
            QuestItemUI uiInstance = Instantiate<QuestItemUI>(questPrefab, transform);
            uiInstance.Setup(quest);
        }
    }
}
