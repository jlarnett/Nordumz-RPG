using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {

        [SerializeField] private GameObject targetToDestroy = null;


        // Update is called once per frame
        void Update()
        {
            if (!GetComponentInChildren<ParticleSystem>().IsAlive())
            {
                Debug.Log("Destroy Gameobject");
                Destroy(gameObject);

//                if (!GetComponent<ParticleSystem>().IsAlive())
//                {
//                    if (targetToDestroy != null)
//                    {
//                        Destroy(targetToDestroy);
//                    }
//                    else
//                    {
//                        Destroy(gameObject);
//                    }
//                }
            }
        }
    }
}
