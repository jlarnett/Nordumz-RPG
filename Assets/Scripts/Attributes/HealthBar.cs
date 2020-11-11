using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] public Health healthComponent = null;      //Character health component
        [SerializeField] public RectTransform foreground = null;    //foreground of healthbar
        [SerializeField] public Canvas rootCanvas = null;

        // Update is called once per frame
        void Update()
        {

            if (Mathf.Approximately(healthComponent.GetFraction(), 0))      //So if characters health component says health is 0 disable root canvas component to disable healthbar
            {
                rootCanvas.enabled = false;
                return;
            }

            if(Mathf.Approximately(healthComponent.GetFraction(), 1))       //If health component says health is full disable healthbar
            {
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;                                                          //Renable health bar incase it isnt 0
            foreground.localScale = new Vector3(healthComponent.GetFraction(), 1, 1);
        }
    }
}
