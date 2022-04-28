using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LessonDetail
{
    public string lessonTitle; 
    public string lessonThumbnail;
    public int viewed;  
    public int isFavorate; 
    public string lessonObjectives; 
    public int lessonId; 
    public string size; 
    public int createdBy; 
    public string authorAvatar; 
    public string authorName; 
    public string createdDate; 
    public int isMyLesson; 
}

public class AllLessonDetails
{
    public int code; 
    public string message; 
    public LessonDetail[] data; 
}
