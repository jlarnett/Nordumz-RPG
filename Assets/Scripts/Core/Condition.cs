using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    [System.Serializable]
    public class Condition
    {
        //The default for a condition is true if we cant check it for some reason.
        [SerializeField]
        private Disjunction [] and;

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            foreach (Disjunction dis in and)
            {
                if (!dis.Check(evaluators))
                {
                    return false;
                }
            }
            return true;
        }


        [System.Serializable]
        class Disjunction
        {
            //We are wrapping Predicate class to be able to calculate any boolan math predicate scenario.
            [SerializeField] private Predicate[] or;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (Predicate pred in or)
                {
                    if (pred.Check(evaluators))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [System.Serializable]
        class Predicate
        {
            //Predicate base. Allows user to setup bool predicates using Ipredicate interface to determine if quest dialogue is populate or other things.
            [SerializeField] private string predicate;
            [SerializeField] private string[] parameters;
            [SerializeField] private bool negate = false;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (var evaluator in evaluators)
                {
                    bool? result = evaluator.Evalulate(predicate, parameters);

                    if (result == null)
                        continue;

                    if (result == negate) return false;
                }

                return true;
            }
        }
    }
}



