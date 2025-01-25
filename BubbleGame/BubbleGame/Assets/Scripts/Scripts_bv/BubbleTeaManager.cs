using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleTeaManager : MonoBehaviour
{
    public List<BubbleSO> selectedBubbles = new List<BubbleSO>();
    public List <SyrupSO> selectedSyrups = new List<SyrupSO>();
    public List <ToppingSO> selectedToppings = new List<ToppingSO>();

    public void AddBubble(BubbleSO bubble)
    { selectedBubbles.Add(bubble); }

    public void AddSyrup(SyrupSO syrup)
    { selectedSyrups.Add(syrup); }

    public void AddTopping(ToppingSO topping)
    { selectedToppings.Add(topping); }

    public void ClearOrder()
    {
        selectedBubbles.Clear();
        selectedSyrups.Clear();
        selectedToppings.Clear();
    }
}
