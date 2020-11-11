using System;
using UnityEngine;
using UnityEngine.UI;
using RPG.Attributes;


namespace RPG.Combat
{

    public class EnemyHealthDisplay : MonoBehaviour
    {
        private Fighter fighter;            //Defines fighter object

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();         //Gets the fighter component attached to player
        }

        private void Update()
        {
            if (fighter.GetTarget() == null)        //If fighter doesn't have target set Enemey Health Display to N/A
            {
                GetComponent<Text>().text = "N/A";
                return;
            }

            Health health = fighter.GetTarget();                                                        //Get the Enemies health component via fighter class
            GetComponent<Text>().text =
                String.Format("{0:0.0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
        }
    }
}
