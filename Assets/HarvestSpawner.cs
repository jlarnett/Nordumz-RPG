using System.Collections;
using System.Collections.Generic;
using RPG.Skill;
using UnityEngine;

public class HarvestSpawner : MonoBehaviour
{
    [SerializeField] Harvestable harvestable = null;
    [SerializeField] private Transform spawnLocation = null;
        

    void Awake()
    {
    }

    void Start()
    {
        if (harvestable == null || spawnLocation == null) return;
        Instantiate(harvestable.GetBasePrefab(), transform.parent);
    }

}
