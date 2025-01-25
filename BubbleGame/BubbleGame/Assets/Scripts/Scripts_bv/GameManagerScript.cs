using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public int playerHealth = 5;
    public int cash;
    public float gameTimer = 60;
    public int customerWaitTime = 40;
    public GameObject gameOverPanel;
    public TMP_Text playerScoreText;
    [NonSerialized] public bool IsGameOver;
    private float _currentTime;

    private void Start()
    {
        _currentTime = 0;
        cash = 0;
        IsGameOver = false;
        gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime >= gameTimer || playerHealth <= 0)
        {
            IsGameOver = true;
        }
        
        if (IsGameOver)
        {
            gameOverPanel.SetActive(true);
            playerScoreText.text = cash.ToString();
        }
    }
}
