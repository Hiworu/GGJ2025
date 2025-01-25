using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public int playerHealth;
    public int cash;
    public int customerWaitTime = 40;

    private void Start()
    {
        cash = 0;
        playerHealth = 0;
    }
}
