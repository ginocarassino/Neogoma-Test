using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Siccity.GLTFUtility;
using System.IO;

public class TestDownload : MonoBehaviour
{
    GameObject wrapper;
    string filePath;
    public GameObject panelSlider;
    public Slider slider;
    public Text progressTxt;
    [SerializeField] GameObject world_canvas;

    private void Start()
    {
        filePath = $"{Application.persistentDataPath}/Files/test.glb";
        wrapper = new GameObject
        {
            name = "Model"
        };
    }

    public void DownloadFile(string url)
    {
        StartCoroutine(GetFileRequest(url, (UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError)
            {
                // Log any errors that may happen
                Debug.Log($"{req.error} : {req.downloadHandler.text}");
            }
            else
            {
                // Save the model into our wrapper
                ResetWrapper();
                GameObject model = Importer.LoadFromFile(filePath);
                model.transform.SetParent(wrapper.transform);
                model.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                //Create collider
                GameObject obj = new GameObject("obj_collider", typeof(BoxCollider));
                BoxCollider bc = obj.GetComponent<BoxCollider>();
                bc.size = new Vector3(0.2f, 0.2f, 0.2f);
                obj.tag = "SpawnedObject";
                obj.transform.SetParent(model.transform);
                

                //model.transform.SetParent(world_canvas.transform);
                //world_canvas.transform.position = new Vector3(0, 0, 0);
            }
        }));
    }


    IEnumerator GetFileRequest(string url, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.downloadHandler = new DownloadHandlerFile(filePath);
            req.SendWebRequest();
            panelSlider.SetActive(true);

            while (!req.isDone)
            {
                Debug.Log(req.downloadProgress * 100);
                slider.value = req.downloadProgress * 100;
                progressTxt.text = (int)(req.downloadProgress * 100) + "%";
                yield return null;
                
            }

            callback(req);
            panelSlider.SetActive(false);
        }
    }

    public void ResetWrapper()
    {
        if (wrapper != null)
        {
            foreach (Transform trans in wrapper.transform)
            {
                Destroy(trans.gameObject);
            }
        }

        GameObject[] colliders = GameObject.FindGameObjectsWithTag("SpawnedObject");
        foreach (GameObject enemy in colliders)
            GameObject.Destroy(enemy);
    }
}
