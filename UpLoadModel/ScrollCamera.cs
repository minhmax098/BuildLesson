using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class ScrollCamera : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
    IPointerUpHandler, IDragHandler
{
    public new Camera camera;
    public bool handle;

    public float max = 90;
    public float min = 30;
    public float speed = 8f;

    private Vector3 _origin;
    private Vector3 _end;
    private Rotate _rotate;

    [FormerlySerializedAs("RenderTexture")]
    public RenderTexture renderTexture;

    public bool onlyMin;
    public bool onlyMax;

    private void Start()
    {
        _rotate = FindObjectOfType<Rotate>();
    }

    private void Update()
    {
        if (camera.targetTexture == null)
        {
            camera.targetTexture = renderTexture;
        }

        if (!handle)
        {
            return;
        }

        var scroll = Input.GetAxisRaw("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            StartCoroutine(ScrollMaxCamera());
        }
        else if (scroll < 0f)
        {
            StartCoroutine(ScrollMinCamera());
        }
        else
        {
            StopAllCoroutines();
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        handle = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        handle = false;
    }

    private IEnumerator ScrollMaxCamera()
    {
        while (camera.fieldOfView < max)
        {
            if (IsVisibleCamera(_rotate.GetComponentInChildren<MeshRenderer>()) || onlyMax)
            {
                onlyMin = false;
                camera.fieldOfView += 2;
                yield return new WaitForEndOfFrame();
            }
            else
            {
                onlyMin = true;
                yield break;
            }
        }
    }

    public bool IsVisibleCamera(Renderer renderer)
    {
        return GeometryUtility
            .TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), renderer.bounds);
    }

    private IEnumerator ScrollMinCamera()
    {
        while (camera.fieldOfView > min)
        {
            if (IsVisibleCamera(_rotate.GetComponentInChildren<MeshRenderer>()) || onlyMin)
            {
                onlyMax = false;
                camera.fieldOfView -= 2;
                yield return new WaitForEndOfFrame();
            }
            else
            {
                onlyMax = true;
                yield break;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _origin = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _origin = _end = Vector3.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _end = Input.mousePosition;

        if ((_end - _origin) != Vector3.zero)
        {
            _rotate.transform.Rotate(Vector3.up, -((_end.x - _origin.x) * speed * Mathf.Deg2Rad));
        }

        _origin = _end;
    }
}