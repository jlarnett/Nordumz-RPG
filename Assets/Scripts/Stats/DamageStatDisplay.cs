using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class DamageStatDisplay : MonoBehaviour
    {
        private BaseStats BaseStats;

        private void Awake()
        {
            BaseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            GetComponent<TextMeshProUGUI>().text = String.Format("{0:0}", BaseStats.GetStat(Stat.Damage));
        }
    }
}
