using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSO : ScriptableObject
{
    public Sprite sprite;
    public List<ToppingSO> toppings = new List<ToppingSO>();
    public List<BubbleSO> bubbles = new List<BubbleSO>();
    public List<SyrupSO> syrups = new List<SyrupSO>();
}
