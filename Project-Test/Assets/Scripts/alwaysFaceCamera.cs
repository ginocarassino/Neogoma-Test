using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class alwaysFaceCamera : MonoBehaviour
{
    GameObject obj;

    void Start()
    {
        //camera = Camera.main.transform;
        obj = GameObject.FindGameObjectWithTag("ARCamera");
    }

    void Update()
    {
        // Rotate the camera every frame so it keeps looking at the target
        obj.transform.LookAt(this.transform);
    }
}
