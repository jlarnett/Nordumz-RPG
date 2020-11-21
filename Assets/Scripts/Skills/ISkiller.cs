using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace RPG.Skill
{
    interface ISkiller
    {
        bool CanAction(GameObject target);
        void Action(GameObject actiontarget);
    }
}
