using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject uiPanel; // Assegna la UI nell'Inspector

    private void Start()
    {
        // Nascondi la UI all'avvio
        if(uiPanel != null)
            uiPanel.SetActive(false);
    }

    private void Update()
    {
        // Controlla il tasto destro del mouse
        if(Input.GetMouseButtonDown(1))
        {
            ToggleUI();
        }
    }

    private void ToggleUI()
    {
        if(uiPanel == null)
        {
            Debug.LogError("UI Panel non assegnato!");
            return;
        }

        // Inverte lo stato attivo/inattivo
        uiPanel.SetActive(!uiPanel.activeSelf);


    Cursor.lockState = CursorLockMode.None; // <-- Nuova riga
    Cursor.visible = true; // <-- Nuova riga
    }
}