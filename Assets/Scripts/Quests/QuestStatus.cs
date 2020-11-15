using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Quest
{
    public class QuestStatus : IPredicateEvaluator
    {
        private Quest quest;
        private List<string> completedObjectives = new List<string>();

        [System.Serializable]
        class QuestStatusRecord
        {
            public string questName;
            public List<string> completedObjectives;
        }

        public QuestStatus(Quest quest)
        {
            this.quest = quest;
        }

        public QuestStatus(object objectState)
        {
            QuestStatusRecord state = objectState as QuestStatusRecord;
            quest = Quest.GetByName(state.questName);
            completedObjectives = state.completedObjectives;
        }

        public Quest GetQuest()
        {
            return quest;
        }

        public int GetCompletedCount()
        {
            return completedObjectives.Count;
        }

        public bool IsObjectiveComplete(string objective)
        {
            return completedObjectives.Contains(objective);
        }

        public void CompleteObjective(string objective)
        {
            //Completes the objective passed in.
            if (quest.HasObjective(objective))
            {
                completedObjectives.Add(objective);
            }
        }

        public object CaptureState()
        {
            QuestStatusRecord state = new QuestStatusRecord();
            state.questName = quest.name;
            state.completedObjectives = completedObjectives;

            return state;
        }

        public bool IsComplete()
        {
            //Go through all objectives and make sure they are all in completed objectives
            foreach (var objective in quest.GetObjectives())
            {
                if (!completedObjectives.Contains(objective.reference))
                {
                    return false;
                }
            }

            return true;
        }

        public bool? Evalulate(string predicate, string[] parameters)
        {

            if (predicate != "IsObjectiveCompleted") return null;

            switch (predicate)
            {
                    case "IsObjectiveCompleted":
                        return IsObjectiveComplete(parameters[0]);
            }

            return null;
        }
    }
}





