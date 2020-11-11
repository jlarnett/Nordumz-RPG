using GameDevTV.Inventories;
using UnityEngine;
using UnityEngine.AI;
using RPG.Stats;


namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {

        //CONFIG DATA
        [Tooltip("How far can the pickups be scattered from the dropper.")]
        [SerializeField] public float scatterDistance = 2;

        [SerializeField] private DropLibrary dropLibray;
        [SerializeField] private int numberOfDrops = 2;

        //CONSTANTS
        private const int ATTEMPTS = 30;

        public void RandomDrop()            //Called from onDie Event of Enemy
        {
            var baseStats = GetComponent<BaseStats>();
            var drops = dropLibray.GetRandomDrops(baseStats.GetLevel());

            foreach (var drop in drops)
            {
                DropItem(drop.item, drop.number);
            }
        }

        protected override Vector3 GetDropLocation()
        {
            for (int i = 0; i < ATTEMPTS; i++)                              // loop through amount of attempts we want to take at finding a location to place item
            {
                Vector3 randomPoint = transform.position;
                randomPoint = randomPoint + Random.insideUnitSphere * scatterDistance;          //Generating the random point using scatterdisstance and unity Random method
                NavMeshHit hit;

                if (NavMesh.SamplePosition(randomPoint, out hit, 0.1f, NavMesh.AllAreas))
                {
                    return hit.position;                                    //RETURN FIRST HIT POSITION WITH SAID PARAMETERS
                }
            }

            return transform.position;      //IF ALL ELSE DEFAULT TO DROPPER LOCATION
        }
    }
}


