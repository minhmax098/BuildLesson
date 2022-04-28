using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TriLibCore;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace UpLoadModel
{
    public class UploadModel : MonoBehaviour
    {
        public Image imgLoadingFill;
        public Text txtResult;
        public GameObject uiUploadModelInfo;
        public Button btnUploadModel3D;

        private void Start()
        {
            SetEventUI();
        }

        public void SetEventUI()
        {
            btnUploadModel3D.onClick.AddListener(HandlerUploadModel);
        }

        public void HandlerUploadModel()
        {
            imgLoadingFill.fillAmount = 0f;

            txtResult.gameObject.SetActive(false);

            AssetLoaderFilePicker.Create()
                .LoadModelFromFilePickerAsync("load model",
                    x =>
                    {
                        var path = $"{x.Filename}";
                        var cam = Camera.main;

                        if (cam != null)
                        {
                            x.RootGameObject.transform.SetParent(cam.transform);
                        }

                        var render = x.RootGameObject.GetComponentsInChildren<MeshRenderer>();

                        foreach (var y in x.MaterialRenderers.Values)
                        {
                            foreach (var mrc in y)
                            {
                                foreach (var r in render)
                                {
                                    if (r.name == mrc.Renderer.name)
                                    {
                                        r.materials = mrc.Renderer.materials;
                                        break;
                                    }
                                }
                            }
                        }

                        var sizeY = x.RootGameObject.GetComponentInChildren<MeshFilter>().mesh.bounds.size.y;

                        while (sizeY<1)
                        {
                            sizeY *= 10f;
                        }

                        sizeY *= 100f;
                        x.RootGameObject.transform.localScale = new Vector3(sizeY, sizeY, sizeY);
                        x.RootGameObject.transform.localPosition = Vector3.zero;
                        x.RootGameObject.transform.localRotation = Quaternion.Euler(Vector3.up * 180f);

                        x.RootGameObject.AddComponent<Rotate>();

                        if (x.RootGameObject.transform.parent != null)
                        {
                            x.RootGameObject.transform.SetParent(null);
                        }

                        StartCoroutine(HandleUploadModel3D(File.ReadAllBytes(path), path));
                        DontDestroyOnLoad(x.RootGameObject);
                    },
                    x => { },
                    (x, y) => { },
                    x => { },
                    x => { },
                    null,
                    ScriptableObject.CreateInstance<AssetLoaderOptions>());
        }

        public IEnumerator HandleUploadModel3D(byte[] fileData, string fileName)
        {
            var form = new WWWForm();

            form.AddBinaryData("model", fileData, fileName);

            const string API_KEY =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJkYXRhIjp7InVzZXJfaWQiOjEsImVtYWlsIjoibnRodW9uZ2dpYW5nLml0QGdtYWlsLmNvbSIsInBhc3N3b3JkIjoiJDJhJDEwJEx2ZmNuQ0lmMDJwMkxXb2dGa29EWC4yYVFMbE16WXc5ZThqRDgud2svRFJobmFYLmtaTThLIiwiZnVsbF9uYW1lIjoiTmd1eeG7hW4gVGjhu4sgSMawxqFuZyBHaWFuZyBOZ3V54buFbiBUaOG7iyBIxrDGoW5nIEdpYW5nIE5ndXnhu4VuIFRo4buLIEjGsMahbmcgR2lhbmcgIiwiYXZhdGFyX2ZpbGVfaWQiOm51bGwsImlzX2FjdGl2ZSI6MSwiY3JlYXRlZF9ieSI6bnVsbCwiY3JlYXRlZF9kYXRlIjpudWxsLCJtb2RpZmllZF9ieSI6bnVsbCwibW9kaWZpZWRfZGF0ZSI6bnVsbH0sImlhdCI6MTY1MDg1Mjg4NCwiZXhwIjoxNjUxNDU3Njg0fQ.8iubPZY_Bo0Nq9VL95uaxvsD1LIfPFyvj4ZeuVAZwFs";

            using var www = UnityWebRequest.Post("https://api.xrcommunity.org/v1/xap/stores/upload3DModel", form);

            www.SetRequestHeader("Authorization", "Bearer " + API_KEY);

            var operation = www.SendWebRequest();

            uiUploadModelInfo.SetActive(true);

            while (!operation.isDone)
            {
                imgLoadingFill.fillAmount = operation.progress * 2f;
                Debug.Log("Percent progress upload:" + operation.progress);
                yield return null;
            }

            uiUploadModelInfo.SetActive(false);
            Debug.Log("Upload API Response: " + www.downloadHandler.text);

            if (www.downloadHandler.text == "Unauthorized" ||
                www.downloadHandler.text.StartsWith("<!DOCTYPE html>"))
            {
                txtResult.gameObject.SetActive(true);
                txtResult.text = $"Upload Failed with response : {www.downloadHandler.text}!";
                yield break;
            }

            var postmodel = JsonConvert.DeserializeObject<PostModel>(www.downloadHandler.text);

            if (postmodel != null)
            {
                txtResult.gameObject.SetActive(true);

                switch (postmodel.Message)
                {
                    case "Successfully!":
                        txtResult.text = "Upload Success!";

                        yield return new WaitForSeconds(1f);

                        SceneManager.LoadScene("InteractiveModel");
                        break;

                    default:
                        txtResult.text = "Upload Failed!";
                        break;
                }
            }
        }
    }
}