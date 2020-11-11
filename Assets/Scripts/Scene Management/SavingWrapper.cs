using System.Collections;
using GameDevTV.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] public float fadeInTime = 0.2f;
        private const string defaultSaveFile = "save";


        private void Awake()
        {
            StartCoroutine(LoadLastScene());                                            //Runs Load last scene
        }

        private void Start()
        {

        }

        public IEnumerator LoadLastScene()                                                     //Becomes a courotine that is automatically ran on AWAKE!! STOPS ERROR
        {
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);   //Loads last scene e.g. save file

            Fader fader = FindObjectOfType<Fader>();                                    //Create fader and assign its value         //pOSITIONALLY IMPOORTANT STOP RACE CONDITIONS
            fader.FadeOutImmediate();                                                   //FadesOut to black on load

            yield return fader.FadeIn(fadeInTime);                                      //Fades in 
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
                 Load();

            if (Input.GetKeyDown(KeyCode.S))
                Save();

            if (Input.GetKeyDown(KeyCode.Delete))
                Delete();

        }

        public void Save()
        {
            //Call to the saving Save system to save
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Load()
        {
            //Call to the saving System to load
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}
