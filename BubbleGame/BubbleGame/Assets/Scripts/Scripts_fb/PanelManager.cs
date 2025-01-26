using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject handPanel;

    private void Start()
    {
        handPanel.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            ToggleUI();
        }
    }

    private void ToggleUI()
    {
        handPanel.SetActive(!handPanel.activeSelf);
        
        //cursore non scompare
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
    }
}