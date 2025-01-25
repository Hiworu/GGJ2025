using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSO : ScriptableObject
{
    public Sprite sprite;
    public List<ToppingSO> Toppings = new List<ToppingSO>();
    public List<BubbleSO> Bubbles = new List<BubbleSO>();
    public List<SyrupSO> Syrups = new List<SyrupSO>();
}
