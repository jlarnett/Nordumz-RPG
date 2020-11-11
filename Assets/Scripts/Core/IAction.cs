using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public interface IAction
    {
        //Used To Cancel Old IAction Compatible Actions like Fighter.Attack && Mover.StartMoveAction
        void Cancel();
    }
}
