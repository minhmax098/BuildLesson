using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class APIUrlConfig
{
    public static string SignIn = "https://api.xrcommunity.org/v1/xap/user/login";
    public static string SignUp = "https://api.xrcommunity.org/v1/xap/user/signup";
    public static string ForgotPass = "https://api.xrcommunity.org/v1/xap/user/forgotPassword";
    public static string ResetPass = "https://api.xrcommunity.org/v1/xap/user/resetPassword";
    public static string LoadLesson = "https://api.xrcommunity.org/v1/xap/{0}"; 
    public static string GetCategoryWithLesson = "https://api.xrcommunity.org/v1/xap/organs/getListXRLibrary"; 
    public static string GetListLessons = "https://api.xrcommunity.org/v1/xap/organs/getListLessonByOrgan?organId=&searchValue={0}&offset={1}&limit={2}"; 
    public static string GetListLessonsByOrgan = "https://api.xrcommunity.org/v1/xap/organs/getListLessonByOrgan?organId={0}&searchValue={1}&offset={2}&limit={3}"; 
    public static string GetLessonsByID = "https://api.xrcommunity.org/v1/xap/lessons/getLessonDetail/{0}"; 
    public static string GetListMyLesson = "https://api.xrcommunity.org/v1/xap/lessons/getListMyLesson?limit={0}&offset={1}&searchValue={2}"; 
    public static string GetListOrgans = "https://api.xrcommunity.org/v1/xap/organs/getListOrgans";
    public static string GetList3DModel = "https://api.xrcommunity.org/v1/xap/models/getList3DModel?type={0}&searchValue={1}&offset={2}&limit={3}";
    public static string UpdateLessonInfo = "https://api.xrcommunity.org/v1/xap/lessons/updateLessonInfo/{0}";
    public static string CreateLessonInfo = "https://api.xrcommunity.org/v1/xap/lessons/createLessonInfo"; 
    public static string Get3DModelDetail = "https://api.xrcommunity.org/v1/xap/models/get3DModelDetail/{0}";
}
