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



    public void ToggleStrawPanel()
    {
        handPanel.SetActive(!handPanel.activeSelf);
    }
}