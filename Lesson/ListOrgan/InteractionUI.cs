using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; 

namespace ListOrgan 
{
    public class InteractionUI : MonoBehaviour
    {
        public GameObject waitingScreen; 
        private GameObject backToHomeBtn; 
        private string emailCheck;
        private static InteractionUI instance; 
        public static InteractionUI Instance
        {
            get 
            {
                if(instance == null)
                {
                    instance = FindObjectOfType<InteractionUI>(); 
                }
                return instance;
            }
        }

        public void onClickItemLesson (int lessonId)
        {
            LessonManager.InitLesson(lessonId);
            SceneNameManager.setPrevScene(SceneConfig.listOrgan);
            if (PlayerPrefs.GetString("user_email") != ""){
                // SceneManager.LoadScene(SceneConfig.lesson);
                StartCoroutine(LoadAsynchronously(SceneConfig.lesson));  
            }
            // else SceneManager.LoadScene(SceneConfig.lesson_nosignin); 
            else StartCoroutine(LoadAsynchronously(SceneConfig.lesson_nosignin)); 
        }   
        void Start()
        {
            InitUI(); 
            SetActions(); 
        }
        void InitUI()
        {
            waitingScreen.SetActive(false);
            backToHomeBtn = GameObject.Find("BackBtn"); 
        }
        void SetActions()
        {
            backToHomeBtn.GetComponent<Button>().onClick.AddListener(BackToHome); 
        }
        void BackToHome()
        {
            emailCheck = PlayerPrefs.GetString("user_email");
            if (emailCheck == "")
            {
                // SceneManager.LoadScene(SceneConfig.home_nosignin); 
                StartCoroutine(LoadAsynchronously(SceneConfig.home_nosignin)); 
            }
            // else SceneManager.LoadScene(SceneConfig.home_user); 
            else StartCoroutine(LoadAsynchronously(SceneConfig.home_user));
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
