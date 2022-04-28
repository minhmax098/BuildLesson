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

namespace UpdateLessonObjectives
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

        public AllLessonDetails GetLessonByID(string lessonId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format(APIUrlConfig.GetLessonsByID, lessonId));
            request.Method = "GET";
            request.Headers["Authorization"] = PlayerPrefs.GetString("user_token"); 
            HttpWebResponse response = (HttpWebResponse)request.GetResponse(); 
            StreamReader reader = new StreamReader(response.GetResponseStream()); 
            jsonResponse = reader.ReadToEnd();
            return JsonUtility.FromJson<AllLessonDetails>(jsonResponse); 
        }

        public IEnumerator Submit(int lessonId, Dictionary<string, string> requestBody)
        {
            string url = String.Format(APIUrlConfig.UpdateLessonInfo, lessonId);
            // Serialize body as a Json string
            string requestBodyString = JsonConvert.SerializeObject(requestBody);
            // Convert Json body string into a byte array
            byte[] requestBodyData = System.Text.Encoding.UTF8.GetBytes(requestBodyString);
            // Create new UnityWebRequest, pass on our url and body as a byte array
            UnityWebRequest webRequest = UnityWebRequest.Put(url, requestBodyData);
            // Specify that our method is of type 'patch'
            webRequest.method = "PATCH";
            // Set request headers i.e. conent type, authorization etc
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", PlayerPrefs.GetString("user_token"));
            // Set the default download buffer
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            // Send the request itself
            yield return webRequest.SendWebRequest();
            // Check for errors
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {   
                // Invoke error action
                // onDeleteRequestError?.Invoke(webRequest.error);
                Debug.Log("an error has occur");
            }
            else
            {
                // Check when response is received
                if (webRequest.isDone)
                {
                    // Invoke success action
                    // onDeleteRequestSuccess?.Invoke("Patch Request Completed");
                    SceneManager.LoadScene(SceneConfig.lesson_edit);
                }
            }
        }
    }
}
