using GameDevTV.Inventories;
using GameDevTV.Saving;
using RPG.Core;
using UnityEngine;
using RPG.Stats;
using GameDevTV.Utils;
using RPG.Combat;
using UnityEngine.Events;
using System;
using System.Diagnostics.Contracts;
using RPG.Skill;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        //Health Fields
        [SerializeField] public float regenerationPercentage = 100;
        [SerializeField] public TakeDamageEvent takeDamage;                  //Work around unity to get serializable
        [SerializeField] public UnityEvent onDie;                           //Unity event for die sound
        [SerializeField] public float overtimeRegenAmount = 10;
        [SerializeField] public float regenSpeed = 2f;
        LazyValue<float> healthPoints;  //lazy value wrapper or container ensure init is called before first use  //negative to identify if our healthpoints are being update weird in race condition
        private bool isDead = false;

        private float regenTimer = 0;


        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>            //Work around unity to get serializble unity event
        {
        }

        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);      //way of passing function pointers around needs initializer delegate
        }

        private void Start()
        {
            healthPoints.ForceInit();       //if by this point we havent initialized do it now
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }
        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }


        public void TakeDamage(GameObject instigator, float damage)         //Instigator is person who attacked enemy
        {

            //Returns the largest of two values -> newHealthValue or 0 whichever is higher.
            // Keeps health from going below 0
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);

            if (healthPoints.value.Equals(0))                             //Checks if healthpoints = 0 Do Die animation for whatever died
            {
                onDie.Invoke();                                          //Unity Event mainly for sound. Invoked when character dies
                Die();
                AwardExperience(instigator);
            }
            else
            {
                takeDamage.Invoke(damage);        //Triggers off all of the Unity function event list.
            }
        }

        void Update()
        {
            OverTimeRegen();
            regenTimer += Time.deltaTime;
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();          //Creates an experience object and assigns it to instigators Experience component
            SkillExperience exp = instigator.GetComponent<SkillExperience>();

            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward), instigator);         //Adds experience to instigator based upon BaseStats method.


            if (true)
            {
                exp.GainCombatExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward), instigator);
            }
        }

        internal void takeDamageFree(GameObject gameObject, float damage)
        {
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);

            if (healthPoints.value.Equals(0))                             //Checks if healthpoints = 0 Do Die animation for whatever died
            {
                onDie.Invoke();                                          //Unity Event mainly for sound. Invoked when character dies
                Die();
                AwardExperience(gameObject);
            }
            else
            {
                takeDamage.Invoke(damage);        //Triggers off all of the Unity function event list.
            }
        }

        //If someone with health script takes damage and health is = 0 if character is dead it returns else it runs death animation and sets isDead = true;
        private void Die()
        {
            if (isDead) return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }
        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;

            if (healthPoints.value == 0)          //verify that we cant load into a world with 0 health somehow
            {
                Die();
            }
        }

        public void Heal(float healthToRestore)
        {
            healthPoints.value = Mathf.Min(healthPoints.value + healthToRestore, GetMaxHealthPoints());         //Assigns the healthpoints.value == to the min between restored health and max
        }                                                                                                       //Stops from going over max

        private void RegenerateHealth()
        {
            //Handles giving player back 100& health on level up
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * regenerationPercentage / 100;     //Creates a regenHealh for level up
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);          //Gives us the higher value our healthpoints or the regen value. Incase we have boost that raise hp above max
        }

        private float GetInitialHealth()    //function called above in awake //guarantess that function is initialized
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentage()
        {
            return 100 * (healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        public float GetFraction()
        {
            //Returns the health in fraction between 0 & 1 mainly for Healthbar script
            return healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        //public getter method
        public bool IsDead()
        {
            return isDead;
        }

        void OverTimeRegen()
        {
            if (isDead) return;
            if (this.gameObject.tag != "Player" ) return;

            if (regenTimer > regenSpeed / 60)
            {
                float tester = healthPoints.value + overtimeRegenAmount;
                healthPoints.value = Mathf.Min(GetMaxHealthPoints(), tester);
                regenTimer = 0;
            }
        }
    }
}
