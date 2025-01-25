using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera :  MonoBehaviour
{

    public Camera camera1;
    public Camera camera2;

    void Start()
    {
        camera1.enabled = true;
        camera2.enabled = false;
    }

    void Update()
    {

        float scrollInput = Input.mouseScrollDelta.y;
        // Cambia alla telecamera 1 quando premi W o scrollUp
        if (Input.GetKeyDown(KeyCode.W)||scrollInput>0)
        {
            Switch(1);
        }

        // Cambia alla telecamera 2 quando premi S o scrollDown
        if (Input.GetKeyDown(KeyCode.S)||scrollInput<0)
        {
            Switch(2);
        }
    }
        void Switch(int cameravalue)
    {
        if(cameravalue==1){
            camera1.enabled = true;
            camera2.enabled = false;
        }else{
            camera2.enabled = true;
            camera1.enabled = false;
        }
        
    }
}
