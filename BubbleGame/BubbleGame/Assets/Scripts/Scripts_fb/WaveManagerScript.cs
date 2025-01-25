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
    public List<GameObject> customers;
    public int activeTimers = 0;
    public Transform spawnTransform;
    public GameObject customerPrefab;

    void Start()
    {
        Debug.Log("start WaveManagerScript");

    }

    void Update()
    {
        
        if (customers.Count == 0 && activeTimers == 0)
        {
            Debug.Log("Nuova wave");
            foreach (float timer in waves[waveCounter])
            {
                if(timer>0){
                    activeTimers++;
                    StartCoroutine(StartTimer(timer));
                }else{
                    Debug.Log("timer-->"+timer);
                    InstantiateCustomer(); // Istanzia il prefab
                }
            }
            waveCounter++;
        }
    }

    IEnumerator StartTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        Debug.Log("timer-->"+duration);
        InstantiateCustomer(); // Istanzia il prefab
        activeTimers--;
    }
    void InstantiateCustomer()
    {
        // Istanzia il prefabn
        Vector3 spawnPosition = spawnTransform.position;
        GameObject newCustomer = Instantiate(customerPrefab, spawnPosition, Quaternion.identity);
        customers.Add(newCustomer);
        Debug.Log("Customer instantiated. Total customers: " + customers.Count);

        // Log the position of the new customer
        Debug.Log("New customer position: " + newCustomer.transform.position);
    }


    public void removeCustomer(GameObject customer)
    {
        customers.Remove(customer);
        Destroy(customer);
    }
}
