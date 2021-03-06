using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 
using UnityEngine.EventSystems; 

namespace CreateLesson_Main
{
    public class InteractionUI : MonoBehaviour
    {
        public GameObject waitingScreen; 
        // private GameObject backToHomeBtn; 
        private GameObject storeBtn; 
        private GameObject importBtn; 
        private static InteractionUI instance; 
        public static InteractionUI Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<InteractionUI>();
                }
                return instance; 
            }
        }
        void Start()
        {
            InitUI();
            SetActions(); 
        }
        void InitUI()
        {
            // backToHomeBtn = GameObject.Find("BackBtn"); 
            storeBtn = GameObject.Find("3DStore");
            importBtn = GameObject.Find("ImportModel");
        }
        void SetActions()
        {
            // backToHomeBtn.GetComponent<Button>().onClick.AddListener(BackToHome);
            storeBtn.GetComponent<Button>().onClick.AddListener(StoreModel);
            importBtn.GetComponent<Button>().onClick.AddListener(HandleBtnImportModel3D);
        }
        // void BackToHome()
        // {
        //     StartCoroutine(LoadAsynchronously(SceneConfig.home_user));
        // }
        void StoreModel()
        {
            StartCoroutine(LoadAsynchronously(SceneConfig.storeModel));
        }

        void HandleBtnImportModel3D()
        {
            SceneManager.LoadScene(SceneConfig.uploadModel);
        }
        public IEnumerator LoadAsynchronously(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            waitingScreen.SetActive(true);
            while(!operation.isDone)
            {
                yield return new WaitForSeconds(3f);
            }
        }
    }
}
