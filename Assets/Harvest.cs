using System.Collections;
using GameDevTV.Inventories;
using RPG.Control;
using RPG.Skill;
using UnityEngine;
using Random = System.Random;

public class Harvest : MonoBehaviour, IRaycastable
{

    [SerializeField] private string HarvestFallAnimationEventText;
    [SerializeField] private string HarvestRespawnAnimationEventText;

    [SerializeField] private Harvestable harvestable;
    private Animator animator;
    private float health = 10;

    

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (harvestable == null) return;

    }

    void Start()
    {
        health = harvestable.GetStartingHealth();

        if (HarvestFallAnimationEventText != null && HarvestRespawnAnimationEventText != null) return;

        animator.ResetTrigger(HarvestFallAnimationEventText);
        animator.ResetTrigger(HarvestRespawnAnimationEventText);
    }

    private IEnumerator WaitRespawn()
    {
        yield return new WaitForSeconds(5);         //Animation gap


        animator.ResetTrigger(HarvestFallAnimationEventText);
        animator.SetTrigger(HarvestRespawnAnimationEventText);

        harvestable.Wait();

        harvestable.SetHarvested(false);
        health = harvestable.GetStartingHealth();

        yield return true;
    }

    public CursorType GetCursorType()
    {
        return harvestable.GetHarvestCursor();
    }

    public bool HandleRaycast(PlayerController callingController)
    {
        if (!enabled) return false;

        if (!callingController.GetComponent<WoodCutter>().CanChop(gameObject))
        {
            return false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            callingController.GetComponent<WoodCutter>().Chop(gameObject);
        }

        return true;
    }

    public void TakeDamage(GameObject instigator, int damage)
    {
        Random random = new Random();
        random.Next(10);

        if (random.Next(10) > 6)
        {
            health = Mathf.Max(health - damage, 0);
        }

        if (random.Next(4) > 1)
        {
            harvestable.AwardHarvestableReward(instigator);
        }

        if (health.Equals(0))                             //Checks if healthpoints = 0 Do Die animation for whatever died
        {
            harvestable.SetHarvested(true);

            if (HarvestFallAnimationEventText == null || HarvestRespawnAnimationEventText == null) return;

            animator.ResetTrigger(HarvestRespawnAnimationEventText);
            animator.SetTrigger(HarvestFallAnimationEventText);

            instigator.GetComponent<WoodCutter>().Cancel();
            AwardExperience(instigator);
            harvestable.AwardHarvestableReward(instigator);


            StartCoroutine(WaitRespawn());
        }
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
        if (harvestable != null)
        {
            return harvestable.IsHarvested();
        }

        return false;
    }
}
