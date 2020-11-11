using System.Collections;
using System.Collections.Generic;
using UnityEngine;





namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] public DamageText damageTextPrefab = null;

        public void Spawn(float damageAmount)       //Takes the amount of damage the spawner should spawn in text
        {
            DamageText instance = Instantiate<DamageText>(damageTextPrefab, transform);         //We instantiate the dmage text to life at transform location. we call this instance
            instance.SetValue(damageAmount);
        }
    }
}

