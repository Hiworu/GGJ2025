using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManagerScript : MonoBehaviour
{
    public float[][] waves = new float[][]
    {
        new float[] { 0f, 2f, 4f },
        new float[] { 0f, 0f, 0f },
        new float[] { 1f },
        new float[] { 3f, 3f, 3f },
        new float[] { 2f, 3f, 4f },
        new float[] { 0f, 0f, 4f, 4f }
    };
    public int waveCounter = 0;
    public CustomerSO[] customers;
    public int activeTimers = 0;

    void Start()
    {
        Debug.Log("start WaveManagerScript");

    }

    void Update()
    {
        
        if (customers.Length == 0&&activeTimers ==0)
        {
            Debug.Log("Nuova wave");
            foreach (float timer in waves[waveCounter])
            {
                if(timer>0){
                    activeTimers++;
                    StartCoroutine(StartTimer(timer));
                }else{
                    Debug.Log("timer-->"+timer);
                }
            }
            waveCounter++;
        }
    }

    IEnumerator StartTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        Debug.Log("timer-->"+duration);
        activeTimers--;
    }

}
