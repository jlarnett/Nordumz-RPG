using GameDevTV.Inventories;
using UnityEngine;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]  //Scriptable object!
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        //Animation override
        [SerializeField] public AnimatorOverrideController weaponAnimatorOverride = null;       //Things that will cahnge based upon having a different weapon
        [SerializeField] public Weapon equippedPrefab = null; //Player weapon prefab
        [SerializeField] public float weaponDamage = 5f;
        [SerializeField] public float percentageBonus = 0f;         //Weapon percent modifier
        [SerializeField] public float weaponRange = 2f;                      //Weapon range -> distance character stops away from target
        [SerializeField] public bool isRightHanded = true;  //Determines if weapon is right or left handed.
        [SerializeField] public Projectile projectile = null;
        [SerializeField] public WeaponType weaponType;

        private const string weaponName = "Weapon";


        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator) //Takes in hand transform & normal animator
        {
            DestroyOldWeapon(rightHand, leftHand);  //dESTROYS OLD WEAPON and passes in both hands

            Weapon weapon = null;           //Weapon is defaulted to null 

            if (equippedPrefab != null)   //If weapon prefab is not equal to null then instantiate a weapon prefrab. if not dont prevents null error if unaremd?
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                weapon = Instantiate(equippedPrefab, handTransform); //Makes weapon prefab at correct hand Transform location.
                weapon.gameObject.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (weaponAnimatorOverride != null) //If weapon override animator is not equal to null than assign ovverride otherwise use default
            {
                animator.runtimeAnimatorController =
                    weaponAnimatorOverride; //Assign override weapon animation to animator
            }
            else if(overrideController != null)
            {
                //Handles a weapon animator bug
                //Will be null if the thing in animator.runtimeanimatorcontroller is character animator controller other wise it will have the animator override controller
                //Get weapon override parent?

                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);       //finds child of hand with weapon name
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);  //Checks if no weapon is in right hand check left hand. still nothing nothing to do
            }
            if(oldWeapon == null) return;

            oldWeapon.name = "DESTROYING";  //bug require
            Destroy(oldWeapon.gameObject);  //Destroys the weapon that was in hand before pickup
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;

            if (isRightHanded)      //Checks if weapon is a right handed weapon or not.
            {
                handTransform = rightHand;
            }
            else
            {
                handTransform = leftHand;
            }

            return handTransform;
        }

        public bool HasProjectile() //Called from fighter
        {
            return projectile != null;      //Lets fighter know if weapon has a projectile or not
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance =                         
                Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);       //projectile instance = instantiatie at hand position

            projectileInstance.SetTarget(target, instigator, calculatedDamage);   //projectile instance sets target & passes weapon damage

        }                                                                                   // Quaternion is rotation of instance?

        public float GetDamage()
        {
            return weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public float GetRange()
        {
            return weaponRange;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return weaponDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return percentageBonus;
            }
        }
    }
}
