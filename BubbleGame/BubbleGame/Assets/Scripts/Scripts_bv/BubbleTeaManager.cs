using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleTeaManager : MonoBehaviour
{
    // public List<BubbleSO> selectedBubbles = new List<BubbleSO>();
    // public List <SyrupSO> selectedSyrups = new List<SyrupSO>();
    // public List <ToppingSO> selectedToppings = new List<ToppingSO>();
    //
    public BubbleSO selectedBubble;
    public SyrupSO selectedSyrup;
    public ToppingSO selectedTopping;

    public void SetBubble(BubbleSO bubble)
    {
        // selectedBubbles.Add(bubble);
        selectedBubble = bubble;
    }

    public void SetSyrup(SyrupSO syrup)
    {
        // selectedSyrups.Add(syrup);
        selectedSyrup = syrup;
    }

    public void SetTopping(ToppingSO topping)
    {
        // selectedToppings.Add(topping)
        selectedTopping = topping;
    }

    public void ClearOrder()
    {
        // selectedBubbles.Clear();
        // selectedSyrups.Clear();
        // selectedToppings.Clear();
        selectedBubble = null;
        selectedSyrup = null;
        selectedTopping = null;
    }
}
