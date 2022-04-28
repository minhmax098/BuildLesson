using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using UnityEngine.EventSystems; 

public class InteractionUI : MonoBehaviour
{
    public GameObject waitingScreen; 
    // private GameObject backToHomeBtn; 
    // private string emailCheck;
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
        // InitUI();
        // SetActions();
    }
    // void InitUI()
    // {
    //     backToHomeBtn = GameObject.Find("BackBtn"); 
    // }
    // void SetActions()
    // {
    //     backToHomeBtn.GetComponent<Button>().onClick.AddListener(BackToHome);
    // }

    public void onClickItemLesson(int lessonId)
    {
        LessonManager.InitLesson(lessonId);
        SceneNameManager.setPrevScene(SceneConfig.myLesson); 
        if (PlayerPrefs.GetString("user_email") != "")
        {
            StartCoroutine(LoadAsynchronously(SceneConfig.lesson_edit)); 
        }
    }

    // void BackToHome()
    // {
    //     emailCheck = PlayerPrefs.GetString("user_email"); 
    //     if (emailCheck == "")
    //     {
    //         StartCoroutine(LoadAsynchronously(SceneConfig.home_nosignin)); 
    //     }
    //     else StartCoroutine(LoadAsynchronously(SceneConfig.home_user)); 
    // }
    
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
