using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public interface IPredicateEvaluator
    {
        bool? Evalulate(string predicate, string[] parameters);
    }
}



