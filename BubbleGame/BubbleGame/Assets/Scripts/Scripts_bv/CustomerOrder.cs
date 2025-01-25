using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class CustomerOrder : MonoBehaviour
{
    [Header ("Customer")]
    [SerializeField] private CustomerSO customer;
    
    private GameManagerScript _gameManager;
    private BubbleTeaManager _bubbleTeaManager;
    private SeagullManager _seagullManager;
    private WaveManagerScript _waveManager;
    

    private bool _isOrderCompleted;
    private float _currentTime;
    private float _currentSeagullTime;
    
    
    
    private void Awake()
    {
        //get GameManager
        GameObject gameManager = GameObject.Find("GameManager");
        _gameManager = gameManager.GetComponent<GameManagerScript>();
        _bubbleTeaManager = gameManager.GetComponent<BubbleTeaManager>();
        _seagullManager = gameManager.GetComponent<SeagullManager>();
        _waveManager = gameManager.GetComponent<WaveManagerScript>();

        //RANDOMIZED INGREDIENTS IF LIST == NULL
        if (customer.Bubbles == null)       { customer.Bubbles = new List<BubbleSO>(); }
        if (customer.Bubbles == null || customer.Bubbles.Count == 0)
        { customer.Bubbles.Add(RandomFromResources<BubbleSO>("Bubbles")); }        
        if (customer.Syrups == null)        { customer.Syrups = new List<SyrupSO>(); }
        if (customer.Syrups == null || customer.Syrups.Count == 0)
        { customer.Syrups.Add(RandomFromResources<SyrupSO>("Syrups")); }
        if (customer.Toppings == null)      { customer.Toppings = new List<ToppingSO>(); }
        if (customer.Toppings == null || customer.Toppings.Count == 0)
        { customer.Toppings.Add(RandomFromResources<ToppingSO>("Toppings")); }
    }

    private T RandomFromResources<T>(string resourceFolder) where T : Object
    {
        T[] assets = Resources.LoadAll<T>(resourceFolder);
        if (assets.Length > 0)
        { return assets[Random.Range(0, assets.Length)]; }
        return null;
    }

    private void Start()
    {
        _currentTime = 0;
        _isOrderCompleted = false;
    }

    private void Update()
    {
        if (this.gameObject != null)
        {
            //timer
            _currentTime += Time.deltaTime;
            if (CompareChoices()) {_isOrderCompleted = true;}
            
            if (!_isOrderCompleted && _currentTime >= _gameManager.customerWaitTime)     { CustomerDissatisfied(); return;}
            if (_seagullManager.IsAttackedBySeagull && _seagullManager.SeagullHasWon)
            {
                Debug.Log("gabbiano ha vinto :(");
                CustomerDissatisfied();
                return;
            }
            
            if (_isOrderCompleted == true)                          { CustomerSatisfied(); }
        }
        
    }

    private bool CompareChoices()
    {
        if (!CompareLists(customer.Bubbles, _bubbleTeaManager.selectedBubbles)) return false;
        if (!CompareLists(customer.Syrups, _bubbleTeaManager.selectedSyrups)) return false;
        if (!CompareLists(customer.Toppings, _bubbleTeaManager.selectedToppings)) return false;
        return true;
    }
    private bool CompareLists<T>(List<T> list1, List<T> list2) where T : Object
    {
        if (list1.Count != list2.Count) return false;

        for (int i = 0; i < list1.Count; i++)
        { if (list1[i] != list2[i]) return false; }
        return true;
    }
    
    private void CustomerDissatisfied()
    {
        _gameManager.playerHealth -= -1;
        _waveManager.removeCustomer(this.gameObject);
        Debug.Log($"customer removed:{this.gameObject.name}");
    }

    private void CustomerSatisfied()
    {
        _gameManager.cash += customer.CashGiven;
        _waveManager.removeCustomer(this.gameObject);
    }
    
}
