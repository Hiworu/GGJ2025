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

    [Header("Order")]
    [SerializeField] private float waitTime;
    
    private GameManagerScript _gameManager;
    
    private bool _isOrderCompleted;
    private float _currentTime;
    
    
    
    private void Awake()
    {
        //get GameManager
        GameObject gameManager = GameObject.Find("GameManager");
        _gameManager = gameManager.GetComponent<GameManagerScript>();
        
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
            if (_isOrderCompleted == false && _currentTime >= waitTime)     { CustomerDissatisfied(); return;}
            if (_isOrderCompleted == true)                                  {CustomerDissatisfied();}
        }
        
    }

    private void CustomerDissatisfied()
    {
        _gameManager.Score = _gameManager.Score -1;
        Destroy(this.gameObject);
    }

    private void CustomerSatisfied()
    {
        _gameManager.Score = _gameManager.Score + 1;
        Destroy(this.gameObject);
    }

    // Customer Behavior
   //
   // 1- GameObject gets instantiated
   // 2- Customer bubble shows order
   // 3- Customer waits x time
   // 4- If Customer gets order ► customer drops x money
   // 4- Else Customer leaves
   //
   // Needs:
   // private bool isCorrectOrder = false;
   // private float currentTimerTime;
   // private float timerCustomer;
  //______________________________
  // Update()
  // {
  // currentTimerTime = timerCustomer-time.deltaTime
  // if(currentTimerTime <=0)
  // {
  // CustomerLeaves();
  // }
  // }
  //
  // each of them selected at random.
  //
  // If given bubble tea's bubble, syrup and topping match the customer's order
  // isCorrectOrder = true;
  // player.cash = player.cash + customerPayment
  //
  // else
  // Debug.Log("Wrong Order");
}
