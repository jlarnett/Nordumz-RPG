﻿using System.Collections;
using System.Collections.Generic;
using RPG.Quest;
using TMPro;
using UnityEngine;

public class QuestItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI progress;

    private QuestStatus status;

    public void Setup(QuestStatus status)
    {
        this.status = status;
        title.text = status.GetQuest().GetTitle();
        progress.text = status.GetCompletedCount() + "/" + status.GetQuest().GetObjectiveCount();
    }

    public QuestStatus GetQuestStatus()
    {
        return status;
    }
}
