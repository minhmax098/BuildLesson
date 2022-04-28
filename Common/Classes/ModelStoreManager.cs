using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class ModelStoreManager
{
    public static int modelId; 
    public static void InitModelStore(int _modelId)
    {
        modelId = _modelId;
    }
}
