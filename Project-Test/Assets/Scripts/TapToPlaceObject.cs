using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.AI;

[RequireComponent(typeof(ARRaycastManager))]
public class TapToPlaceObject : MonoBehaviour
{
    public GameObject v_GameobjectToInstantiate;
    MainController main_controller;
    public GameObject PanelScan;

    private GameObject v_SpawnedObject;
    private ARPlaneManager planeManager;
    private ARRaycastManager aRRaycastManager;
    private Vector2 v_TouchPosition;
    [SerializeField] Camera arCamera;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    bool isPlaced = false;
    bool startScan = false;

    private void Start()
    {
        main_controller = (MainController)FindObjectOfType(typeof(MainController));
    }

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();

        planeManager.enabled = false;
    }

    void Update()
    {
        if (startScan == true)
        {
            if (isPlaced == false)
            {
                Place();
            }
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            v_TouchPosition = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                if (Physics.Raycast(ray, out hitObject))
                {
                    GameObject obj = GameObject.FindGameObjectWithTag("SpawnedObject");
                    GameObject obj_butt = GameObject.FindGameObjectWithTag("3DButton");

                    if (obj != null)
                    {
                        Debug.Log("Touched");
                        main_controller.SpawnButton();
                    }
                    else if (obj_butt != null)
                    {
                        Debug.Log("PREMUTO BOTTONE");
                        main_controller.ResetWrapper();
                    }
                }
            }
        }
    }

    public void StartScanning()
    {
        startScan = true;
        planeManager.enabled = true;
    }

    bool TryGetTouchPosition(out Vector2 p_touchPosition)
    {
        if (Input.touchCount > 0)
        {
            p_touchPosition = Input.GetTouch(0).position;
            return true;
        }

        p_touchPosition = default;
        return false;
    }

    private void Place()
    {
        if (!TryGetTouchPosition(out Vector2 touchPos))
        {
            return;
        }

        if (aRRaycastManager.Raycast(touchPos, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPos = hits[0].pose;

            v_GameobjectToInstantiate.SetActive(true);
            v_GameobjectToInstantiate.transform.position = hitPos.position;
            v_GameobjectToInstantiate.transform.rotation = hitPos.rotation;

            DisablePlanes();
            main_controller.Scanned();
            PanelScan.SetActive(false);
        }
    }

    public void PlaceSelectedObject()
    {
        v_GameobjectToInstantiate.SetActive(true);
    }

    public void PlaceTest()
    {
        v_GameobjectToInstantiate.SetActive(true);
    }

    private void DisablePlanes()
    {
        isPlaced = true;
        planeManager.enabled = false;

        GameObject[] planes = GameObject.FindGameObjectsWithTag("Plane");

        if (planes.Length != 0)
        {
            foreach (GameObject a in planes)
            {
                a.SetActive(false);
            }
        }
    }
}
