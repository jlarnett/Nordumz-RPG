using System.Collections;
using System.Collections.Generic;
using RPG.Quest;
using UnityEngine;

public class QuestListUI : MonoBehaviour
{
    [SerializeField] private QuestItemUI questPrefab;
    private QuestList questList;

    // Start is called before the first frame update
    void Start()
    {
        questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
        questList.onUpdate += Redraw;
        Redraw();
    }

    private void Redraw()
    {
        //Delete all current UI children
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        //Instantiate all the quest in questlist
        foreach (QuestStatus status in questList.GetStatuses())
        {
            QuestItemUI uiInstance = Instantiate<QuestItemUI>(questPrefab, transform);
            uiInstance.Setup(status);
        }
    }
}
