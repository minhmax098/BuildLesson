using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System; 
using UnityEngine.Networking;

namespace UpdateLessonObjectives
{
    public class LoadScene : MonoBehaviour
    {
        public GameObject spinner;
        public LessonDetail[] myData;
        public LessonDetail currentLesson; 
        public GameObject bodyObject; 
        public Button updateBtn;
        private ListOrgans listOrgans;
        public GameObject dropdownObj; 
        private Dropdown dropdown;
        private List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        void Start()
        {
            Screen.orientation = ScreenOrientation.Portrait; 
            StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
            StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
            // updateBtn = transform.GetComponent<Button>();
            updateBtn.onClick.AddListener(UpdateLessonObjective);
            myData = LoadData.Instance.GetLessonByID(LessonManager.lessonId.ToString()).data;
            // currentLesson = Array.Find(myData, lesson => lesson.lessonId == LessonManager.lessonId);
            currentLesson = myData[0];
            StartCoroutine(LoadCurrentLesson(currentLesson));
            spinner.SetActive(false);
            dropdown = dropdownObj.GetComponent<Dropdown>();
            updateDropDown();
        }

        IEnumerator LoadCurrentLesson(LessonDetail currentLesson)
        {
            string imageUri = String.Format(APIUrlConfig.LoadLesson, currentLesson.lessonThumbnail);
            bodyObject.transform.GetChild(0).GetChild(1).GetComponent<InputField>().text = currentLesson.lessonTitle; 
            bodyObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text = currentLesson.lessonObjectives; 
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUri);
            yield return request.SendWebRequest(); 
            if (request.isNetworkError || request.isHttpError)
            {

            }
            if (request.isDone)
            {
                Texture2D tex = ((DownloadHandlerTexture) request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
                // bodyObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = sprite;
            }
        }
        void updateDropDown()
        {
            listOrgans = LoadData.Instance.getListOrgans();
            foreach (ListOrganLesson organ in listOrgans.data)
            {
                options.Add(new Dropdown.OptionData(organ.organsName));
            }
            dropdown.AddOptions(options);
        }
        void UpdateLessonObjective()
        {
            // Check form valid
            Debug.Log("form submit");
            Debug.Log("Index choose: " + dropdown.value);
            // Reference the Real index by the API 
            Debug.Log("Real index: " + listOrgans.data[dropdown.value].organsId); 
            Debug.Log("Lesson name modified: " + bodyObject.transform.GetChild(0).GetChild(1).GetComponent<InputField>().text);
            Debug.Log("Lesson obj mod : " + bodyObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text);
            
            Dictionary<string, string> requestBody = new Dictionary<string, string>()
            {
                {"modelId", "55" },
                {"lessonTitle", bodyObject.transform.GetChild(0).GetChild(1).GetComponent<InputField>().text},
                {"organId", listOrgans.data[dropdown.value].organsId.ToString()},
                {"lessonObjectives", bodyObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text},
                {"publicLesson", "1"}
            };
            StartCoroutine(LoadData.Instance.Submit(LessonManager.lessonId, requestBody));
        }   
    }
}
