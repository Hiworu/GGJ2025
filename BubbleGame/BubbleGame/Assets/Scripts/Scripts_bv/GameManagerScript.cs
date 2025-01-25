using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    [NonSerialized] public int PlayerHealth;
    [NonSerialized] public int Cash;

    private void Start()
    {
        Cash = 0;
        PlayerHealth = 0;
    }
}
