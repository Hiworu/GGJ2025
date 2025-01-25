using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerOrder : MonoBehaviour
{
    public ScriptableObject scriptableObject;
    
    //OnInstantiate()
    //if scriptableObject.toppings is empty
    //get random topping from list  (Assets/Resources/Toppings)
    //if scriptableObject.bubbles is empty
    //get random bubble from list   (Assets/Resources/Bubbles)
    //if scriptableObject.syrups is empty
    //get random syrup from list    (Assets/Resources/Syrups)
    
    //Camera: Fixed
    // ws up down screen
  //
  // palline
  // sciroppi 
  // topping
  //
  //
  //
  // Customer Behavior
  //
  // 1- GameObject gets instantiated
  // 2- Customer bubble shows order
  // 3- Customer waits x time
  // 4- If Customer gets order ► customer drops x money
  // 4- Else Customer leaves
  //
  // Needs:
  // ScriptableObject CustomerSO
  // public List<Bubble> bubbles;
  // public List<Syrup> syrups;
  // public List<Topping> toppings;
  // private bool isCorrectOrder = false;
  // private float currentTimerTime;
  // private float timerCustomer;
  //
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
