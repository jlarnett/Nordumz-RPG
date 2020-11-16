using System;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.Saving;
using RPG.Core;
using RPG.Quest;
using UnityEngine;

public class QuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
{
    List<QuestStatus> statuses = new List<QuestStatus>();
    Dictionary<string, Quest> CompletedStatuses = new Dictionary<string, Quest>();

    public event Action onUpdate;
    
    public void AddQuest(Quest quest)
    {
        QuestStatus newStatus = new QuestStatus(quest);

        if (HasQuest(quest)) return;
        if (CompletedQuest(newStatus)) return;

        statuses.Add(newStatus);

        if(onUpdate == null) return;
        onUpdate();
    }

    public bool HasQuest(Quest quest)
    {
        return GetQuestStatus(quest) != null;
    }

    public bool CompletedQuest(QuestStatus QuestStatus)
    {
        return CompletedStatuses.ContainsKey(QuestStatus.GetQuest().GetTitle());
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

        if (!status.IsObjectiveComplete(objective))
        {
            status.CompleteObjective(objective);
        }

        if (status.IsComplete())
        {
            GiveReward(quest);
            CompletedStatuses.Add(quest.GetTitle(), quest);
            statuses.Remove(status);
        }

        if (onUpdate != null)
        {
            onUpdate();
        }
    }

    private void GiveReward(Quest quest)
    {
        foreach (var reward in quest.GetRewards())
        {
            bool itemAdded = GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, reward.number);

            if (!itemAdded)
            {
                GetComponent<ItemDropper>().DropItem(reward.item, reward.number);
            }
        }
    }

    public object CaptureState()
    {
        List<object> state = new List<object>();

        foreach (QuestStatus status in statuses)
        {
            state.Add(status.CaptureState());
        }

        return state;
    }

    public void RestoreState(object state)
    {
        //Restores the quest list to the appropriate settings
        List<object> stateList = state as List<object>;
        if (stateList == null) return;

        statuses.Clear();

        foreach (object objectState in stateList)
        {
            statuses.Add(new QuestStatus(objectState));
        }
    }

    public bool? Evalulate(string predicate, string[] parameters)
    {
        //This evaluates and checks whether player has specified quest
        if(predicate != "HasQuest") return null;

        switch (predicate)
        {
            case "HasQuest":
                return HasQuest(Quest.GetByName(parameters[0]));

            case "CompletedQuest":
                return CompletedQuest(new QuestStatus(parameters[0]));
        }

        return null;
    }
}
