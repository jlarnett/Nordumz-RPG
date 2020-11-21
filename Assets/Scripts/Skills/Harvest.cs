using System.Collections;
using RPG.Control;
using UnityEngine;
using Random = System.Random;

namespace RPG.Skill
{
    public class Harvest : MonoBehaviour, IRaycastable
    {
        [SerializeField] private float currentHealth = 1000f;
        [SerializeField] private bool harvested;

        [SerializeField] private float animationTimer = 10;
        [SerializeField] private string HarvestFallAnimationEventText;
        [SerializeField] private string HarvestRespawnAnimationEventText;

        [SerializeField] private Harvestable harvestable;

        private Animator animator;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void Start()
        {
            if (harvestable == null) return;
            if (HarvestFallAnimationEventText != null && HarvestRespawnAnimationEventText != null) return;

            currentHealth = harvestable.GetStartingHealth();
            harvested = false;

            animator.ResetTrigger(HarvestFallAnimationEventText);
            animator.ResetTrigger(HarvestRespawnAnimationEventText);
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

            if (HarvestFallAnimationEventText == null || HarvestRespawnAnimationEventText == null) return;

            if (currentHealth.Equals(0))                    //Checks if healthpoints = 0 Do Die animation for whatever died
            {
                FinishHarvest(instigator);
                AwardExperience(instigator);

                harvestable.AwardHarvestableReward(instigator);

                animator.ResetTrigger(HarvestRespawnAnimationEventText);
                animator.SetTrigger(HarvestFallAnimationEventText);

                StartCoroutine(RespawnTree());
            }
        }

        private IEnumerator RespawnTree()
        {
            yield return new WaitForSeconds(animationTimer);

            animator.ResetTrigger(HarvestFallAnimationEventText);
            animator.SetTrigger(HarvestRespawnAnimationEventText);

            RespawnHarvest();
        }

        private void RespawnHarvest()
        {
            harvested = false;
            currentHealth = harvestable.GetStartingHealth();
        }

        private void FinishHarvest(GameObject instigator)
        {
            harvested = true;
            harvestable.Cancel(instigator);
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



