using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.Networking;
using System.IO;
using System; 
using System.Threading.Tasks; 
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

namespace CreateLesson
{
    public class LoadData : MonoBehaviour
    {
        private string jsonResponse;
        private static LoadData instance;
        public static LoadData Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = FindObjectOfType<LoadData>();
                }
                return instance; 
            }
        }
        public ListOrgans getListOrgans()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(APIUrlConfig.GetListOrgans); 
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader= new StreamReader(response.GetResponseStream());
            jsonResponse = reader.ReadToEnd();
            Debug.Log("JSON RESPONSE: ");
            Debug.Log(jsonResponse);
            return JsonUtility.FromJson<ListOrgans>(jsonResponse);
        }
        // public AllList3DModel Get3DModelDetail(string modelId)
        // {
        //     HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format(APIUrlConfig.Get3DModelDetail, modelId));
        //     request.Method = "GET";
        //     request.Headers["Authorization"] = PlayerPrefs.GetString("user_token"); 

        //     HttpWebResponse response = (HttpWebResponse)request.GetResponse(); 
        //     StreamReader reader = new StreamReader(response.GetResponseStream()); 
        //     jsonResponse = reader.ReadToEnd();
        //     return JsonUtility.FromJson<AllList3DModel>(jsonResponse); 
        // }
        public IEnumerator buildLesson(PublicLesson requestBody)
        {   
            var webRequest = new UnityWebRequest(APIUrlConfig.CreateLessonInfo, "POST");
            Debug.Log(requestBody.modelId);
            Debug.Log(requestBody.lessonObjectives);
            // string requestBodyString = JsonConvert.SerializeObject(requestBody);
            string requestBodyString = JsonUtility.ToJson(requestBody);

            Debug.Log(requestBodyString);
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(requestBodyString);

            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", PlayerPrefs.GetString("user_token"));

            // string url = APIUrlConfig.CreateLessonInfo;
            // string requestBodyString = JsonConvert.SerializeObject(requestBody);
            // byte[] requestBodyData = System.Text.Encoding.UTF8.GetBytes(requestBodyString);
            // UnityWebRequest webRequest = UnityWebRequest.Put(url, requestBodyData);
            // webRequest.method = "POST";
            // webRequest.SetRequestHeader("Content-Type", "application/json");
            // webRequest.SetRequestHeader("Authorization", PlayerPrefs.GetString("user_token"));
            // webRequest.downloadHandler = new DownloadHandlerBuffer();
            
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {   
                // Invoke error action
                // onDeleteRequestError?.Invoke(webRequest.error);
                Debug.Log("An error has occur");
                Debug.Log(webRequest.error);
            }
            else
            {
                // Check when response is received
                if (webRequest.isDone)
                {
                    // Invoke success action
                    // onDeleteRequestSuccess?.Invoke("Patch Request Completed");
                    SceneManager.LoadScene(SceneConfig.buildLesson);
                }
            }

        }
        
        // public All3DModel GetModel
    }
}
