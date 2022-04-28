using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Helper
{
    public const float DETA_TIME_AVARAGRE = 0.0001f;

    public static IEnumerator MoveObject(GameObject moveObject, Vector3 targetPosition)
    {
        float timeSinceStarted = 0f;
        while (true)
        {
            timeSinceStarted += Time.deltaTime;
            moveObject.transform.position = Vector3.Lerp(moveObject.transform.position, targetPosition, timeSinceStarted);
            if (moveObject.transform.position == targetPosition)
            {
                yield break;
            }
            yield return null;
        }
    }

    public static GameObject GetChildOrganOnTouchByTag(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit[] raycastHits;
        raycastHits = Physics.RaycastAll(ray);
        foreach (RaycastHit hit in raycastHits)
        {
            if (hit.collider.transform.root.gameObject.tag == TagConfig.ORGAN_TAG)
            {
                if (hit.collider.transform.parent == ObjectManager.Instance.CurrentObject.transform)
                {
                    return hit.collider.gameObject;
                }
            }
        }
        return null;
    }

    public static bool GetUIByTouch(Vector2 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit[] raycastHits;
        raycastHits = Physics.RaycastAll(ray);
        foreach (RaycastHit hit in raycastHits)
        {
            if (hit.collider.transform.gameObject.tag == TagConfig.UI_TAG)
            {
                return true;
            }
        }
        return false;
    }

    public static Bounds GetRenderBounds(GameObject go)
    {
        var totalBounds = new Bounds();
        totalBounds.SetMinMax(Vector3.one * Mathf.Infinity, -Vector3.one * Mathf.Infinity);
        foreach (var renderer in go.GetComponentsInChildren<Renderer>())
        {
            var bounds = renderer.bounds;
            var totalMin = totalBounds.min;
            totalMin.x = Mathf.Min(totalMin.x, bounds.min.x);
            totalMin.y = Mathf.Min(totalMin.y, bounds.min.y);
            totalMin.z = Mathf.Min(totalMin.z, bounds.min.z);

            var totalMax = totalBounds.max;
            totalMax.x = Mathf.Max(totalMax.x, bounds.max.x);
            totalMax.y = Mathf.Max(totalMax.y, bounds.max.y);
            totalMax.z = Mathf.Max(totalMax.z, bounds.max.z);

            totalBounds.SetMinMax(totalMin, totalMax);
        }

        return totalBounds;
    }

    public static string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public static void RemoveElementOfListFromIndexToEnd(List<GameObject> listOrigin, int indexStartDelete)
    {
        for (int index = indexStartDelete; index < listOrigin.Count; index++)
        {
            listOrigin.RemoveAt(index);
        }
    }

    public static void RemoveElementOfListInListObjectFromIndexToEnd(List<List<GameObject>> listOrigin, int indexStartDelete)
    {
        for (int index = indexStartDelete; index < listOrigin.Count; index++)
        {
            listOrigin.RemoveAt(index);
        }
    }
    public static void RemoveElementOfListInListVector3FromIndexToEnd(List<List<Vector3>> listOrigin, int indexStartDelete)
    {
        for (int index = indexStartDelete; index < listOrigin.Count; index++)
        {
            listOrigin.RemoveAt(index);
        }
    }
    public static void CopyToClipboard(string str)
    {
        GUIUtility.systemCopyBuffer = str;
    }

    public static List<Vector3> GetListchildrenOfOriginPosition(GameObject obj)
    {
        int childCount = obj.transform.childCount;
       
        List<Vector3> listchildrenOfOriginPosition = new List<Vector3>();

        for (int i = 0; i < childCount; i++)
        {
            if (obj.transform.GetChild(i).gameObject.tag != TagConfig.LABEL_TAG)
            {
                listchildrenOfOriginPosition.Add(obj.transform.GetChild(i).localPosition);
            }
        }

        return listchildrenOfOriginPosition;
    } 

    public static void ResetStatusFeature()
    {
        // Label
        LabelManager.Instance.IsShowingLabel = false;
        LabelManager.Instance.HandleLabelView(LabelManager.Instance.IsShowingLabel);
        // Separate
        SeparateManager.Instance.IsSeparating = false;
        SeparateManager.Instance.HandleSeparate(SeparateManager.Instance.IsSeparating);
    }

    public static IEnumerator EffectScaleObject(GameObject scaleObject, float timeScale, Vector3 targetLocalScale)
    {
        Vector3 startScale = targetLocalScale / 10f;
        scaleObject.transform.localScale = startScale;
        int numberFrame = Convert.ToInt32(timeScale / DETA_TIME_AVARAGRE);
        while (true)
        {
            scaleObject.transform.localScale += (targetLocalScale - startScale) / numberFrame;
            if (Vector3.SqrMagnitude(scaleObject.transform.localScale - targetLocalScale) < DETA_TIME_AVARAGRE)
            {
                yield break;
            }
            yield return null;
        }
    }
   
}
