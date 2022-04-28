using System.Security;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.ARFoundation;

public class ObjectManager : MonoBehaviour
{
    private static ObjectManager instance;
    public static ObjectManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ObjectManager>();
            }
            return instance;
        }
    }
    private const float TIME_SCALE_FOR_APPEARANCE = 0.04f;
    public static event Action onInitOrganSuccessfully;
    public static event Action<string> onChangeCurrentObject;
    public static event Action onResetObject;

    public OrganInfor OriginOrganData { get; set; }
    public Material OriginOrganMaterial { get; set; }
    public GameObject OriginObject { get; set; }
    public List<Vector3> ListchildrenOfOriginPosition { get; set; }
    public GameObject CurrentObject { get; set; }
    public Vector3 OriginPosition { get; set; }
    public Quaternion OriginRotation { get; set; }
    public Vector3 OriginScale { get; set; }

    void Start() 
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    public void LoadDataOrgan()
    {
        OriginOrganData = new OrganInfor("", "Brain_Demo", "");
    }

    public void InitOriginalExperience()
    {
        LoadDataOrgan();
        GameObject prefabMainOrgan = Resources.Load(PathConfig.PRE_PATH_MODEL + OriginOrganData.Name, typeof(GameObject)) as GameObject;
        GameObject objectInstance = Instantiate(prefabMainOrgan, Vector3.zero, Quaternion.Euler(Vector3.zero)) as GameObject;
        InitObject(objectInstance);
    }

    public void InitObject(GameObject newObject)
    {   
        OriginObject = newObject;
        OriginOrganMaterial = OriginObject.GetComponent<Renderer>().materials[0];
        ChangeCurrentObject(OriginObject);
        OriginPosition = OriginObject.transform.position;
        OriginRotation = OriginObject.transform.rotation;
        OriginScale = OriginObject.transform.localScale;

        onInitOrganSuccessfully?.Invoke();
    }

    public void ChangeCurrentObject(GameObject newGameObject)
    {
        CurrentObject = newGameObject;
        ListchildrenOfOriginPosition = Helper.GetListchildrenOfOriginPosition(CurrentObject);
        onChangeCurrentObject?.Invoke(CurrentObject.name);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Instantiate object at specified position/rotation in AR mode
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void InstantiateARObject(Vector3 position, Quaternion rotation, bool isHost)
    {
        OriginObject.transform.position = position;
        OriginObject.transform.rotation = rotation;
        if (isHost && (!ARUIManager.Instance.IsStartAR))
        {
            OriginObject.transform.localScale *= ModelConfig.scaleFactorInARMode;
        }
        OriginObject.SetActive(true);
        AddARAnchorToObject();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Show zoom up affect for object
    /// </summary>
    public void ShowZoomUpAffectForObject()
    {
        StartCoroutine(Helper.EffectScaleObject(OriginObject, TIME_SCALE_FOR_APPEARANCE, OriginObject.transform.localScale));
    }

    public void Instantiate3DObject()
    {
        OriginObject.transform.position = OriginPosition;
        OriginObject.transform.rotation = OriginRotation;
        OriginObject.transform.localScale = OriginScale;
        OriginObject.SetActive(true);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Add ARAnchor component to object
    /// </summary>
    public void AddARAnchorToObject()
    {
        if (OriginObject != null)
        {
            if (OriginObject.GetComponent<ARAnchor>() == null)
            {
                ARAnchor localAnchor = OriginObject.AddComponent<ARAnchor>();
                localAnchor.destroyOnRemoval = false;
            }
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Get ARAnchor component from object
    /// </summary>
    /// <returns></returns>
    public ARAnchor GetARAnchorComponent()
    {
        if (OriginObject == null)
        {
            return null;
        }
        return OriginObject.GetComponent<ARAnchor>();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Destroy original object
    /// </summary>
    public void DestroyOriginalObject()
    {
        if (OriginObject != null)
        {
            Destroy(OriginObject);
        }
        if (CurrentObject != null)
        {
            Destroy(CurrentObject);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Destroy AR Anchor component
    /// </summary>
    public void DestroyARAnchorComponent()
    {
        if (OriginObject != null)
        {
            ARAnchor localAnchor = OriginObject.GetComponent<ARAnchor>();
            if (localAnchor != null)
            {
                Destroy(localAnchor);
            }
        }
    }

    // public void Init3DObject
}
