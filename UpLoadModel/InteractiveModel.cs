using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InteractiveModel : MonoBehaviour
{
    public Text txtModelName;
    public Button btnCreateThumbnail;
    public Button btnBack;
    public int resWidth = 2550;
    public int resHeight = 3300;
    public Image imgScreenShot;
    public new Camera camera;
    public Transform parent2;
	public Vector3 offset;

    private void Start()
    {
        var rotate = FindObjectOfType<Rotate>();

        if (rotate != null)
        {
            var parent = rotate.transform;

            parent.SetParent(parent2);

            parent.localPosition = offset;
            txtModelName.text = parent.name;
        }

        InitEvents();
    }

    private void InitEvents()
    {
        btnBack.onClick.AddListener(() => SceneManager.LoadScene("UploadModel"));
        btnCreateThumbnail.onClick.AddListener(HandleCreateThumbnail);
    }

    private void HandleCreateThumbnail()
    {
        LateUpdates();
    }

    public static string ScreenShotName(int width, int height)
    {
        return
            $"{Application.dataPath}/ScreenShots/screen_{width}x{height}_{System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.png";
    }

    private void LateUpdates()
    {
        var rt = new RenderTexture(resWidth, resHeight, 24);

        camera.targetTexture = rt;

        var screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);

        camera.Render();

        RenderTexture.active = rt;

        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        screenShot.Apply();

        camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors

        Destroy(rt);

        imgScreenShot.sprite = Sprite.Create(screenShot, new Rect(0, 0, resWidth, resHeight), new Vector2(0, 0));

        File.WriteAllBytes(ScreenShotName(resWidth, resHeight), screenShot.EncodeToPNG());
    }
}