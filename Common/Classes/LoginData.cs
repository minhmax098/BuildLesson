using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LoginData
{
    public int code; 
    public string message; 
    public Data[] data;
}

[System.Serializable]
public class Data {
    public string token;
    public UserInfomation user;
}

[System.Serializable]
public class UserInfomation {
    public string fullName;
    public string email;

    // public string password;
    // public string confirmPassword; 
}
