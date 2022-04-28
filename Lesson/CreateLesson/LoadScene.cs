using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System; 
using UnityEngine.SceneManagement; 
using UnityEngine.Networking; 
using System.Text;

namespace CreateLesson
{
    public class LoadScene : MonoBehaviour
    {
        public GameObject spinner;
        public List3DModel[] myData;
        public List3DModel currentModel; 
        public GameObject bodyObject; 
        public Button buildLessonBtn;
        public Button cancelBtn;
        private ListOrgans listOrgans;
        public GameObject dropdownObj; 
        private Dropdown dropdown;
        private List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        void Start()
        {
            Screen.orientation = ScreenOrientation.Portrait; 
            StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
            StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
            buildLessonBtn.onClick.AddListener(CreateLessonInfo);

            // myData = LoadData.Instance.GetLessonByID(LessonManager.lessonId.ToString()).data; 
            // currentModel = myData[0];
            // StartCoroutine(LoadCurrentModel(currentModel));

            spinner.SetActive(false);
            dropdown = dropdownObj.GetComponent<Dropdown>();
            updateDropDown();
        }
        
        // IEnumerator LoadCurrentModel(List3DModel currentLesson)
        // {
        //     string imageUri = String.Format(APIUrlConfig.LoadLesson, currentLesson.modelThumbnail);
        //     bodyObject.transform.GetChild(0).GetChild(1).GetComponent<InputField>().text = currentLesson.modelName; 

        //     // bodyObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text = currentLesson.lessonObjectives; 
        //     UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUri);
        //     yield return request.SendWebRequest(); 
        //     if (request.isNetworkError || request.isHttpError)
        //     {
        //     }
        //     if (request.isDone)
        //     {
        //         Texture2D tex = ((DownloadHandlerTexture) request.downloadHandler).texture;
        //         Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
        //         // bodyObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = sprite;
        //     }
        // }
        
        void updateDropDown()
        {
            listOrgans = LoadData.Instance.getListOrgans();
            foreach (ListOrganLesson organ in listOrgans.data)
            {
                options.Add(new Dropdown.OptionData(organ.organsName));
            }
            dropdown.AddOptions(options);
        }
        public void CreateLessonInfo() 
        {
            // var request = new UnityWebRequest(APIUrlConfig.CreateLessonInfo, "POST");
            // byte[] bodyRaw = Encoding.UTF8.GetBytes(createLessonInfoJsonString);
            // request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            // request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            // request.SetRequestHeader("Content-Type", "application/json");

            // // StartCoroutine(WaitForAPIResponse(request));
            // yield return request.SendWebRequest();
            // if (request.error != null)
            // {
            //     Debug.Log("Error: " + request.error);
            // }
            // else
            // {
            //     string response = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
            //     SceneManager.LoadScene(SceneConfig.buildLesson); 
            // }
            // Dictionary<string, string> requestBody = new Dictionary<string, string>()
            // {
            //     {"modelId", ModelStoreManager.modelId.ToString()},
            //     {"lessonTitle", bodyObject.transform.GetChild(0).GetChild(1).GetComponent<InputField>().text},
            //     {"organId", listOrgans.data[dropdown.value].organsId.ToString()},
            //     {"lessonObjectives", bodyObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text},
            //     {"publicLesson", "1"}
            // };
            // PublicLesson newLesson = new PublicLesson
            // {
            //     modelId = ModelStoreManager.modelId;
            //     lessonTitle = bodyObject.transform.GetChild(0).GetChild(1).GetComponent<InputField>().text;
            //     organId = listOrgans.data[dropdown.value].organsId;
            // };
            
            PublicLesson newLesson = new PublicLesson();
            newLesson.modelId = ModelStoreManager.modelId;
            newLesson.lessonTitle = bodyObject.transform.GetChild(0).GetChild(1).GetComponent<InputField>().text;
            newLesson.organId = listOrgans.data[dropdown.value].organsId;
            newLesson.lessonObjectives = bodyObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text;
            newLesson.publicLesson = 1;
            StartCoroutine(LoadData.Instance.buildLesson(newLesson));
            // StartCoroutine(LoadData.Instance.buildLesson(modelId, 
            // bodyObject.transform.GetChild(0).GetChild(1).GetComponent<InputField>().text,
            // listOrgans.data[dropdown.value].organsId, 
            // bodyObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text, 
            // 1
            // ));
        }
    }
}
