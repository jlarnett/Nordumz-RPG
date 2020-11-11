using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        public const float waypointGizmoRadius = 0.3f;

        private void OnDrawGizmos()
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);        //sets J = next waypoint index value
                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmoRadius);     // Draws waypoint hub
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));        //Draws lines from each waypoint to next. Accounts for end of patrolPath
            }
        }

        public int GetNextIndex(int i)
        {
            //Gets next waypoint to draw line too returns 0 if at end of children waypoint
            if (i + 1 == transform.childCount)      
            {
                return 0;
            }

            return i + 1;
        }

        public Vector3 GetWaypoint(int i)
        {
            //returns waypoint Position at specified index
            return transform.GetChild(i).position;
        }
    }
}
