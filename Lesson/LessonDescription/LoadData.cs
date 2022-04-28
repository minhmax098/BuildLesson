using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.Networking;
using System.IO;
using System;

namespace LessonDescription
{
    public class LoadData : MonoBehaviour
    {
        private string jsonResponse;
        // public TextAsset jsonFileGetLessonDetail; 
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

        public AllLessonDetails GetLessonsByID(string lessonId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format(APIUrlConfig.GetLessonsByID, lessonId)); 
            // request.Headers["Content-Type"] = "application/json";
            request.Method = "GET";
            request.Headers["Authorization"] = PlayerPrefs.GetString("user_token");
            Debug.Log("Call Lesson Detail API !!!!");
            Debug.Log("Lesson id: " + lessonId);
            Debug.Log("User token: " + PlayerPrefs.GetString("user_token"));

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            jsonResponse = reader.ReadToEnd();
            Debug.Log("JSON RESPONSE: ");
            Debug.Log(jsonResponse);
            return JsonUtility.FromJson<AllLessonDetails>(jsonResponse);
            // return JsonUtility.FromJson<AllLessonDetails>(jsonFileGetLessonDetail.text);
        }
    }
}
