using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText
{

    public class DamageText : MonoBehaviour
    {
        [SerializeField] public Text damageText = null;

        public void SetValue(float amount)
        {
            damageText.text = String.Format("{0:0}", amount);
        }
    }
}
