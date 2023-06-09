using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : Item // Might want to come back in the future and create an Item interface
{
    public int maxAmount;
    public int currAmount = 0;
    public int tentativeAmount = 0;
    public UIManager UI;

    private void Awake()
    {
        UI = GameObject.Find("GameController").GetComponent<UIManager>();
        GridManager.UpdateResources(this);
        if (itemType == "Gold") UI.UpdateGoldSlider(maxAmount);
        else if (itemType == "Ore") UI.UpdateOreSlider(maxAmount);
    }

    public void AddResources(int toAdd) //Add the given amount of resources to the container
    {
        currAmount += toAdd;
        if (itemType == "Gold") UI.UpdateGoldText(toAdd);
        else if (itemType == "Ore") UI.UpdateOreText(toAdd);
    }

    public bool CheckAmount(int toCheck) //Check if the container can hold the given amount of resources
    {
        return currAmount + toCheck <= maxAmount;
    }

    public bool IsFull() //Check if the container is currently full;
    {
        return tentativeAmount == maxAmount;
    }

    public int GetRemaining() //Get how many more resources the container can store in it
    {
        return maxAmount - currAmount;
    }

    public void Fill()
    {
        if (itemType == "Gold") UI.UpdateGoldText(maxAmount - currAmount);
        else if (itemType == "Ore") UI.UpdateOreText(maxAmount - currAmount);
        currAmount = maxAmount;
    }

    public void AddTentative(int toAdd) //Add the amount of resources being brought by queued resources, in an effort to prevent the container being queued when it is set to be filled
    {
        tentativeAmount += toAdd;
        if (tentativeAmount > maxAmount) tentativeAmount = maxAmount;
    }
}
