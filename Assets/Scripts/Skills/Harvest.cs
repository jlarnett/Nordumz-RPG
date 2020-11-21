using System.Collections;
using RPG.Control;
using UnityEngine;
using Random = System.Random;

namespace RPG.Skill
{
    public class Harvest : MonoBehaviour, IRaycastable
    {
        [SerializeField] private GameObject harvestFinishDisable;
        [SerializeField] private float currentHealth = 1000f;
        [SerializeField] private bool harvested;
        [SerializeField] private Harvestable harvestable;

        private Animator animator;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void Start()
        {
            if (harvestable == null) return;

            currentHealth = harvestable.GetStartingHealth();
            harvested = false;
        }

        public CursorType GetCursorType()
        {
            return harvestable.GetHarvestCursor();
        }

        public bool HandleRaycast(PlayerController callingController)
        { 
            if (!enabled) return false;

            if (!harvestable.CanActuate(gameObject, callingController))
            {
                return false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                harvestable.Actuate(gameObject, callingController);
            }

            return true;
        }

        public void TakeDamage(GameObject instigator, int damage)
        {
            if (currentHealth.Equals(0)) currentHealth = harvestable.GetStartingHealth();

            Random random = new Random();
            random.Next(10);

            if (random.Next(4) > 1)
                harvestable.AwardHarvestableReward(instigator);

            if (random.Next(10) > 6)
                currentHealth = Mathf.Max(currentHealth - damage, 0);

            if (currentHealth.Equals(0))                    //Checks if healthpoints = 0 Do Die animation for whatever dieds
            {
                FinishHarvest(instigator);
                AwardExperience(instigator);

                harvestable.AwardHarvestableReward(instigator);
                StartCoroutine(RespawnTree());
            }
        }

        private IEnumerator RespawnTree()
        {
            yield return new WaitForSeconds(harvestable.GetRespawnTime());
            RespawnHarvest();
        }

        private void RespawnHarvest()
        {
            harvested = false;
            currentHealth = harvestable.GetStartingHealth();
            harvestFinishDisable.SetActive(true);
        }

        private void FinishHarvest(GameObject instigator)
        {
            harvested = true;
            harvestable.Cancel(instigator);
            harvestFinishDisable.SetActive(false);
        }

        private void AwardExperience(GameObject instigator)
        {
            SkillExperience exp = instigator.GetComponent<SkillExperience>();

            if (exp == null) return;
            exp.GainSkillExperience(harvestable.GetExperienceReward(), instigator, harvestable.GetSkill());
        }

        public Vector3 GetPosition()
        {
            if (transform.position == null) return Vector3.zero;
            return transform.position;
        }

        public bool isHarvested()
        {
            return harvested;
        }
    }
}



