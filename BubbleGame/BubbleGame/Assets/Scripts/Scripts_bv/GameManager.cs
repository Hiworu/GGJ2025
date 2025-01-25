using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [NonSerialized] public int Score;
    [NonSerialized] public int Cash;

    private void Start()
    {
        Cash = 0;
        Score = 0;
    }
}
