using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera :  MonoBehaviour
{

    public Camera camera1;
    public Camera camera2;
    public bool isCamera1 = false;

    void Start()
    {
        camera1.enabled = true;
        camera2.enabled = false;
        isCamera1 = true;

    }

    void Update()
    {

        float scrollInput = Input.mouseScrollDelta.y;
        // Cambia alla telecamera 1 quando premi W o scrollUp
        if (Input.GetKeyDown(KeyCode.W)||scrollInput>0||Input.GetKeyDown(KeyCode.A))
        {
            Switch(1);
        }

        // Cambia alla telecamera 2 quando premi S o scrollDown
        if (Input.GetKeyDown(KeyCode.S)||scrollInput<0||Input.GetKeyDown(KeyCode.D))
        {
            Switch(2);
/*              SoundManager.Instance.PlayAudio("Audios/bruhSound"); */
        }



    }
        void Switch(int cameravalue)
    {
        if(cameravalue==1){
            camera1.enabled = true;
            isCamera1 = true;
            camera2.enabled = false;
        }else{
            camera2.enabled = true;
            isCamera1 = false;
            camera1.enabled = false;
        }
        
    }
}
