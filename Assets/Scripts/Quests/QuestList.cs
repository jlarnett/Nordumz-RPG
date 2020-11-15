﻿using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Quest;
using UnityEngine;

public class QuestList : MonoBehaviour
{
    List<QuestStatus> statuses = new List<QuestStatus>();

    public event Action onUpdate;
    
    public void AddQuest(Quest quest)
    {

        if (HasQuest(quest)) return;

        QuestStatus newStatus = new QuestStatus(quest);
        statuses.Add(newStatus);


        if(onUpdate == null) return;
        onUpdate();
    }

    public bool HasQuest(Quest quest)
    {
        return GetQuestStatus(quest) != null;
    }

    private QuestStatus GetQuestStatus(Quest quest)
    {
        foreach (QuestStatus status in statuses)
        {
            if (status.GetQuest() == quest)
            {
                return status;
            }
        }

        return null;
    }

    public IEnumerable<QuestStatus> GetStatuses()
    {
        return statuses;
    }

    public void CompleteObjective(Quest quest, string objective)
    {
        QuestStatus status = GetQuestStatus(quest);
        status.CompleteObjective(objective);

        if (onUpdate != null)
        {
            onUpdate();
        }
    }
}
