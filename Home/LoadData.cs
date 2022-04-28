using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.Networking;
using System.IO;
using System;

namespace Home {
    public class LoadData : MonoBehaviour
    {
        // public TextAsset jsonGetListXRLibrary;
        private string jsonResponse;
        private static LoadData instance;
        public static LoadData Instance
        {
            get {
                if (instance == null) 
                {
                    instance = FindObjectOfType<LoadData>();
                }
                return instance;
            }
        }
        void Start()
        {
            
        }
        // public Lessons GetLessonByCategory(string category_id){
        //     LoadJsonFromWeb api_loader = GameObject.Find("API").GetComponent<LoadJsonFromWeb>();
        //     string json_result = api_loader.GetListLessonByCategory(category_id);
        //     if (json_result != ""){
        //         return JsonUtility.FromJson<Lessons>(json_result);
        //     }
        //     else return null;
        // }
        public ListXRLibrary GetCategoryWithLesson() {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(APIUrlConfig.GetCategoryWithLesson); 
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader= new StreamReader(response.GetResponseStream());
            jsonResponse = reader.ReadToEnd();
            Debug.Log("JSON RESPONSE: ");
            Debug.Log(jsonResponse);
            return JsonUtility.FromJson<ListXRLibrary>(jsonResponse);
        }

        // public Categories GetCategories() {
        //     // Dung ham lay json tu web ve (cs - > )Jos
        //     // LoadJsonFromWeb api_loader = GameObject.Find("API").GetComponent<LoadJsonFromWeb>();
        //     // return JsonUtility.FromJson<Categories>(api_loader.GetCategoryJSON());
        //     return JsonUtility.FromJson<Categories>(jsonFileCategory.text) as Categories;
        // }

    }

}