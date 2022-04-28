using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Linq;

namespace Home
{
    public class LoadScene : MonoBehaviour
    {
        public GameObject waitingScreen;
        public GameObject contentItemCategoryWithLesson;
        public GameObject contentItemCategory;
        public GameObject searchBox;
        // add 3 record: searchBtn, xBtn, sumLesson
        public GameObject searchBtn;
        public GameObject xBtn;
        private ListXRLibrary x;
        private int numberCategories;
        // private UnityWebRequest request;
        public ScrollRect scroll; // Lesson scroll
        private List<UnityWebRequestAsyncOperation> requests = new List<UnityWebRequestAsyncOperation>(); 
        private List<GameObject> organLessonList = new List<GameObject>(); 
        private int calculatedSize = 30; 

        void Start()
        {
            waitingScreen.SetActive(false);
            Screen.orientation = ScreenOrientation.Portrait;
            StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
            StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
            // search record
            searchBtn.SetActive(true);
            xBtn.SetActive(false);
            xBtn.transform.GetComponent<Button>().onClick.AddListener(ClearInput); //
            // searchBox.GetComponent<InputField>().onValueChanged.AddListener(UpdateList); 
            // LoadCategories();
            StartCoroutine(LessonByCategory());
        }
        
        void Update()
        {
            if (searchBox.GetComponent<InputField>().isFocused == true)
            {
                Debug.Log("Search box is focused !!!");
                PlayerPrefs.SetString("user_input", "");

                StartCoroutine(LoadAsynchronously(SceneConfig.xrLibrary));
            }
        }
        private string formatString(string inputString, int maxSize)
        {
            if (inputString.Length > maxSize)
            {
                return inputString.Remove(maxSize) + "..."; 
            }
            return inputString;
        }
        // public void LoadCategories()
        // {
        //     // Load from file json
        //     x = LoadData.Instance.GetCategoryWithLesson();
        //     numberCategories = x.data.Length;
        //     foreach (OrganForHome organ in x.data)
        //     {
        //         GameObject categoryObject = Instantiate(Resources.Load(DemoConfig.demoItemCategoryPath) as GameObject);
        //         categoryObject.name = organ.organsId.ToString();
        //         categoryObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = organ.organsName;
        //         Button categoryBtn = categoryObject.GetComponent<Button>();
        //         categoryBtn.onClick.AddListener(() => scrollContent(organ.organsId.ToString()));
        //         categoryObject.transform.parent = contentItemCategory.transform;
        //     }
        // }

        // void UpdateList(string data)
        // {
        //     if(data == "")
        //     {
        //         xBtn.SetActive(false);
        //         searchBtn.SetActive(true);
        //     }
        //     else
        //     {
        //         // xBtn.SetActive(true);
        //         // searchBtn.SetActive(false);
        //         PlayerPrefs.SetString("user_input", data);
        //         SceneManager.LoadScene(SceneConfig.xrLibrary);
        //     }
        // }
        void scrollContent(string id)
        {
            GameObject currentBtnObj = scroll.transform.Find("Viewport/Content/" + id).gameObject;
            Debug.Log("Scroll content active !!!");
            // currentBtnObj.GetComponent<Image>().color = new Color32(255,255,225,100);
            Debug.Log("Value: " + (float)(currentBtnObj.transform.GetSiblingIndex() + 1) / numberCategories);
            scroll.verticalNormalizedPosition = 1f;
            scroll.verticalNormalizedPosition = 1f - (float)(currentBtnObj.transform.GetSiblingIndex() + 1) / numberCategories + 0.05f;
        }

        void ClearInput()
        {
            searchBox.GetComponent<InputField>().SetTextWithoutNotify(""); 
            xBtn.SetActive(false);
            searchBtn.SetActive(true);
        }

        IEnumerator LessonByCategory()
        {
            // var requests = new List<UnityWebRequestAsyncOperation>(); 
            // var organLessonList = new List<GameObject>(); 
            x = LoadData.Instance.GetCategoryWithLesson();
            numberCategories = x.data.Length;
            //load from file json
            foreach (OrganForHome organ in x.data)
            {
                // check if m have a lesson, load image and neu khong co lesson, k load 
                if (organ.listLesson.Length > 0)
                {
                    GameObject itemCategoryObject = Instantiate(Resources.Load(DemoConfig.demoItemCategoryWithLessonPath) as GameObject);
                    itemCategoryObject.name = organ.organsId.ToString();
                    itemCategoryObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = organ.organsName;
                    itemCategoryObject.transform.parent = contentItemCategoryWithLesson.transform;
                    itemCategoryObject.transform.localScale = Vector3.one;

                    Button moreLessonBtn = itemCategoryObject.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Button>();
                    moreLessonBtn.onClick.AddListener(() => updateOrganManager(organ.organsId, organ.organsName));

                    GameObject subContent = itemCategoryObject.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
                    
                    foreach (LessonForHome lesson in organ.listLesson)
                    {
                        // StartCoroutine(LoadLessons(subContent, lesson));
                        string imageUri = String.Format(APIUrlConfig.LoadLesson, lesson.lessonThumbnail);
                        var www = UnityWebRequestTexture.GetTexture(imageUri); 
                        requests.Add(www.SendWebRequest()); 

                        GameObject lessonObject = Instantiate(Resources.Load(DemoConfig.demoLessonObjectPath) as GameObject);

                        Debug.Log("lesson.lessonId.ToString() = "+ lesson.lessonId.ToString());
                        organLessonList.Add(lessonObject);
                        
                        lessonObject.name = lesson.lessonId.ToString();
                        lessonObject.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = formatString(lesson.lessonTitle, calculatedSize);
                        lessonObject.transform.parent = subContent.transform;
                        
                        Button lessonBtn = lessonObject.GetComponent<Button>();
                        lessonBtn.onClick.AddListener(() => InteractionUI.Instance.onClickItemLesson(lesson.lessonId));
                    }

                }
            }
            yield return new WaitUntil(() => AllRequestDone(requests)); 
            HandleAllRequestsWhenFinished(requests, organLessonList); 
        }

        void LateUpdate(){
            if (organLessonList.Count > 0){
                for (var i = 0; i <organLessonList.Count; i++){
                    organLessonList[i].GetComponent<RectTransform>().localScale = Vector3.one;
                }
            }
        }

        private bool AllRequestDone(List<UnityWebRequestAsyncOperation> requests)
        {
            return requests.All(r => r.isDone); 
        }

        private void HandleAllRequestsWhenFinished(List<UnityWebRequestAsyncOperation> requests, List<GameObject>organLessonList)
        {
            for(var i = 0; i < requests.Count; i++)
            {
                var www = requests[i].webRequest;
                if(www.isNetworkError || www.isHttpError) 
                {
                    // Dont modify any thing
                }
                else 
                {
                    Texture2D tex = ((DownloadHandlerTexture) www.downloadHandler).texture;
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
                    // Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    // Sprite spriteFromWeb = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0, 0));
                    // Change the image with the specific item
                    organLessonList[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite = sprite;  
                    organLessonList[i].transform.GetChild(2).gameObject.SetActive(false);
                }
            }
        }

        void updateOrganManager(int id, string name)
        {
            OrganManager.InitOrgan(id, name);
            Debug.Log(id);
            // SceneNameManager.setPrevScene(SceneConfig.home_noSignIn);
            // SceneManager.LoadScene(SceneConfig.listOrgan);
            StartCoroutine(LoadAsynchronously(SceneConfig.listOrgan));
        }

        // IEnumerator LoadLessons(GameObject parentObject, LessonForHome lesson)
        // {
        //     string imageUri = String.Format("https://api.xrcommunity.org/v1/xap/{0}", lesson.lessonThumbnail);
        //     UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUri);
        //     yield return request.SendWebRequest();
        //     while (!request.isDone)
        //     {
        //         yield return null;
        //     }
        //     Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
        //     Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
        //     GameObject lessonObject = Instantiate(Resources.Load(DemoConfig.demoLessonObjectPath) as GameObject);
        //     lessonObject.name = lesson.lessonId.ToString();
        //     lessonObject.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = lesson.lessonTitle;
        //     Debug.Log(lesson.lessonThumbnail.Split('.')[0].Substring(1));
        //     lessonObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite = sprite;
        //     lessonObject.transform.parent = parentObject.transform;
        //     Button lessonBtn = lessonObject.GetComponent<Button>();
        //     lessonBtn.onClick.AddListener(() => InteractionUI.Instance.onClickItemLesson(lesson.lessonId));
        // }

        public IEnumerator LoadAsynchronously(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            waitingScreen.SetActive(true);
            while (!operation.isDone)
            {
                yield return new WaitForSeconds(.2f);
            }
        }
    }
}