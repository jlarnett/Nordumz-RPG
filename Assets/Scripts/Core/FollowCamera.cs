using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] public Transform target;

        // Start is called before the first frame update
        void Start()
        {

        }

        void LateUpdate()
        {
            //Assigns Main Camera position in accordance to target or characters position. 
            transform.position = target.position;
        }
    }
}
