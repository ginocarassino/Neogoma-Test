using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickButtonCanvas : MonoBehaviour
{
    MainController main_controller;

    // Start is called before the first frame update
    void Start()
    {
        main_controller = (MainController)FindObjectOfType(typeof(MainController));
    }

    public void Clear()
    {
        Debug.Log("SCHIACCIATO");
        main_controller.ResetWrapper();
    }
}
