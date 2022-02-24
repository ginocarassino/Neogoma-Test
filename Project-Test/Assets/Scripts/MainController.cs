using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Siccity.GLTFUtility;
using System.IO;

public class MainController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject bottom_panel;
    [SerializeField] GameObject world_canvas;
    [SerializeField] GameObject panelSlider;
    [SerializeField] Slider slider;
    [SerializeField] Text progressTxt;

    GameObject wrapper;
    string filePath;
    TapToPlaceObject ar_manager;

    private void Start()
    {
        ar_manager = (TapToPlaceObject)FindObjectOfType(typeof(TapToPlaceObject));

        filePath = $"{Application.persistentDataPath}/Files/test.glb";
        wrapper = new GameObject
        {
            name = "Model"
        };
    }

    public void Scanned()
    {
        bottom_panel.SetActive(true);
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

                GameObject go = ar_manager.v_GameobjectToInstantiate;

                model.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, go.transform.position.z);
                model.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                GameObject obj = new GameObject("obj_collider", typeof(BoxCollider));
                obj.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, go.transform.position.z);
                BoxCollider bc = obj.GetComponent<BoxCollider>();
                bc.size = new Vector3(0.2f, 0.2f, 0.2f);
                obj.tag = "SpawnedObject";
                obj.transform.SetParent(model.transform);




                //model.transform.SetParent(world_canvas.transform);
                //world_canvas.transform.position = new Vector3(0,0,0);
            }
        }));
    }

    string GetFilePath(string url)
    {
        string[] pieces = url.Split('/');
        string filename = pieces[pieces.Length - 1];

        return $"{filePath}{filename}";
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
        Debug.Log("1");
        if (wrapper != null)
        {
            Debug.Log("2");
            foreach (Transform trans in wrapper.transform)
            {
                Debug.Log("3");
                Destroy(trans.gameObject);
            }
        }
    }

    public void SpawnButton()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("SpawnedObject");
        GameObject wc = Instantiate(world_canvas, new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z), Quaternion.identity);
        world_canvas.SetActive(true);
        wc.transform.SetParent(obj.transform);
    }

}
