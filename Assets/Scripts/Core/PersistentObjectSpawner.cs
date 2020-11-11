using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject persistentObjectPrefab;

        private static bool hasSpawned = false; //static bool variable lives beyond class

        public void Awake()
        {
            if (hasSpawned) return; //if has spawn return dont do anything

            SpawnPersistentObjects();                       //NORACE CONDITION
            hasSpawned = true;

        }

        private void SpawnPersistentObjects()
        {
            GameObject persistentObject = Instantiate(persistentObjectPrefab);          //NO RACE CONDITION ACCESING
            DontDestroyOnLoad(persistentObject);
        }
    }

}
