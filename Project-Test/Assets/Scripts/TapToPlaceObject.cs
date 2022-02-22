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

    private GameObject v_SpawnedObject;
    private ARPlaneManager planeManager;
    private ARRaycastManager aRRaycastManager;
    private Vector2 v_TouchPosition;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    bool isPlaced = false;
    bool startScan = false;
    
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
        }
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
