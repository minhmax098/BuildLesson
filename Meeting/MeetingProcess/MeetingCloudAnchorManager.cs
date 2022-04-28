using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;
using System;

public class MeetingCloudAnchorManager : MonoBehaviour
{
    // time for storing cloud anchor (day)
    private const int EXPIRE_TIME = 1; 
    // ARAnchorManger
    public ARAnchorManager arAnchorManager;
    
    // cloud anchor ID: for retry resolve after failure in first time
    public string cloudAnchorID;

    // Cloud anchor transform: for reset object in AR mode (init at the same previous position)
    public Vector3 CloudAnchorPosition;
    public Vector3 CloudAnchorRotation;

    // Check if master client is hosting cloud anchor.
    public bool IsHostingCloudAnchor = false;

    // Check if member is resolving cloud anchor.
    public bool IsResolvingCloudAnchor = false;

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Declare triggers for failed events of cloud anchor handlers
    /// </summary>
    public static event Action<string> onHostCloudAnchorFailed;
    public static event Action<string> onResolveCloudAnchorFailed;

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Singleton pattern: Create only one instance of MeetingCloudAnchorManager class
    /// Note: All CLOUD ANCHOR processes will be handled in instance of MeetingCloudAnchorManager class
    /// </summary>
    private static MeetingCloudAnchorManager instance;
    public static MeetingCloudAnchorManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MeetingCloudAnchorManager>();
            }
            return instance;
        }   
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Host cloud anchor
    /// </summary>
    /// <param name="localAnchor">Anchor will be hosted</param>
    public void HostCloudAnchor(ARAnchor localAnchor)
    {
        StartCoroutine(AsyncHostCloudAnchor(localAnchor));
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Repeat hosting cloud anchor until AR session is tracking state
    /// </summary>
    /// <param name="localAnchor">Anchor will be hosted</param>
    /// <returns></returns>
    IEnumerator AsyncHostCloudAnchor(ARAnchor localAnchor)
    {
        while (true)
        {
            if (IsHostingCloudAnchor || (localAnchor == null) || (ModeManager.Instance.Mode != ModeManager.MODE_EXPERIENCE.MODE_AR))
            {
                yield break;
            }
            if (ARSession.state != ARSessionState.SessionTracking)
            {
                yield return null;
            }
            ARCloudAnchor arCloudAnchor = arAnchorManager.HostCloudAnchor(localAnchor, EXPIRE_TIME);
            if (arCloudAnchor != null)
            {
                IsHostingCloudAnchor = true;
                StartCoroutine(CheckHostingAnchorProcess(arCloudAnchor));
            }
            else
            {
                IsHostingCloudAnchor = false;
                onHostCloudAnchorFailed?.Invoke(MeetingConfig.unableToHostCloudAnchor);
            }
            yield break;
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Repeat checking host cloud anchor state until success or error 
    /// </summary>
    /// <param name="currentARCloudAnchor">Anchor is checked state</param>
    /// <returns></returns>
    IEnumerator CheckHostingAnchorProcess(ARCloudAnchor currentARCloudAnchor)
    {
        while (true)
        {
            if ((!IsHostingCloudAnchor) || (currentARCloudAnchor == null) || (ModeManager.Instance.Mode != ModeManager.MODE_EXPERIENCE.MODE_AR))
            {
                yield break;
            }
            if (currentARCloudAnchor.cloudAnchorState == CloudAnchorState.Success)
            {
                IsHostingCloudAnchor = false;
                cloudAnchorID = currentARCloudAnchor.cloudAnchorId;
                CloudAnchorPosition = currentARCloudAnchor.transform.position;
                CloudAnchorRotation = currentARCloudAnchor.transform.rotation.eulerAngles;
                MeetingExperienceManager.Instance.ShareCloudAnchorId(currentARCloudAnchor.cloudAnchorId);
                yield break;
            }
            else if (currentARCloudAnchor.cloudAnchorState == CloudAnchorState.TaskInProgress)
            {
                yield return null;
            }
            else
            {
                IsHostingCloudAnchor = false;
                onHostCloudAnchorFailed?.Invoke(MeetingConfig.GetAnchorStateMessage(currentARCloudAnchor.cloudAnchorState));
                yield break;
            }
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Resolve cloud anchor by id
    /// </summary>
    /// <param name="cloudAnchorId">Cloud anchor id will be resolved</param>
    public void ResolveCloudAnchorById(string cloudAnchorId)
    {
        StartCoroutine(AsyncResolveCloudAnchorById(cloudAnchorId));
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Repeat resolving cloud anchor by id until AR session is tracking state
    /// </summary>
    /// <param name="cloudAnchorId">Cloud anchor id will be resolved</param>
    /// <returns></returns>
    IEnumerator AsyncResolveCloudAnchorById(string cloudAnchorId)
    {
        while (true)
        {
            if (IsResolvingCloudAnchor || (string.IsNullOrEmpty(cloudAnchorId)) || ModeManager.Instance.Mode != ModeManager.MODE_EXPERIENCE.MODE_AR)
            {
                yield break;
            }
            if (ARSession.state != ARSessionState.SessionTracking)
            {
                yield return null;
            }
            cloudAnchorID = cloudAnchorId;
            ARCloudAnchor arCloudAnchor = arAnchorManager.ResolveCloudAnchorId(cloudAnchorId);
            if (arCloudAnchor != null)
            {
                IsResolvingCloudAnchor = true;
                StartCoroutine(CheckResolvingAnchorProcess(arCloudAnchor));
            }
            else
            {
                IsResolvingCloudAnchor = false;
                onResolveCloudAnchorFailed?.Invoke(MeetingConfig.unableToResolveCloudAnchor);
            }
            yield break;
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Repeat checking resolve cloud anchor state until success or error 
    /// </summary>
    /// <param name="currentARCloudAnchor">Cloud anchor is checked state</param>
    /// <returns></returns>
    IEnumerator CheckResolvingAnchorProcess(ARCloudAnchor currentARCloudAnchor)
    {
        while (true)
        {
            if ((!IsResolvingCloudAnchor) || (currentARCloudAnchor == null) || (ModeManager.Instance.Mode != ModeManager.MODE_EXPERIENCE.MODE_AR))
            {
                yield break;
            }
            if (currentARCloudAnchor.cloudAnchorState == CloudAnchorState.Success)
            {
                IsResolvingCloudAnchor = false;
                CloudAnchorPosition = currentARCloudAnchor.transform.position;
                CloudAnchorRotation = currentARCloudAnchor.transform.rotation.eulerAngles;
                MeetingExperienceManager.Instance.ResolveCloudAnchorTransform(currentARCloudAnchor.transform);
                yield break;
            }
            else if (currentARCloudAnchor.cloudAnchorState == CloudAnchorState.TaskInProgress)
            {
                yield return null;
            }
            else
            {
                IsResolvingCloudAnchor = false;
                onResolveCloudAnchorFailed?.Invoke(MeetingConfig.GetAnchorStateMessage((currentARCloudAnchor.cloudAnchorState)));
                yield break;
            }
        }
    }
}
