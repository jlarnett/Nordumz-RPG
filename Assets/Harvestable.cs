using System.Collections;
using GameDevTV.Inventories;
using RPG.Control;
using UnityEngine;
using Random = System.Random;

namespace RPG.Skill
{
    [CreateAssetMenu(fileName = "Harvest", menuName = "Harvestable/newItem", order = 2)]  //Scriptable object!
    public class Harvestable : ScriptableObject
    {
        [SerializeField] private float harvestExpReward = 50;
        [SerializeField] private float startingHealth = 10;
        [SerializeField] private float respawnTime = 5;
        [SerializeField] private int resourcesRewarded = 1;
        [SerializeField] private bool isHarvested = false;
        [SerializeField] private Skill rewardSkill;
        [SerializeField] private CursorType cursorType = CursorType.Woodcutting;
        [SerializeField] private ActionItem testItem;

        public float GetStartingHealth()
        {
            return startingHealth;
        }

        public bool IsHarvested()
        {
            return isHarvested;
        }

        public float GetRespawnTime()
        {
            return respawnTime;
        }

        public IEnumerator Wait()
        {
            yield return new WaitForSeconds(respawnTime);
        }

        public void SetHarvested(bool set)
        {
            isHarvested = set;
        }

        public CursorType GetHarvestCursor()
        {
            return cursorType;
        }

        public void AwardHarvestableReward(GameObject instigator)
        {
            System.Random random = new Random();
            int randomResource = random.Next(resourcesRewarded);


            Inventory playerInventory = instigator.GetComponent<Inventory>();
            playerInventory.AddToFirstEmptySlot(testItem, randomResource);
        }

        public float GetExperienceReward()
        {
            return harvestExpReward;
        }

        public Skill GetSkill()
        {
            return rewardSkill;
        }
    }
}
