using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public string type;
    public int maxAmount;
    public int currAmount = 0;
    public int tentativeAmount = 0;

    public void AddResources(int toAdd) //Add the given amount of resources to the container
    {
        currAmount += toAdd;
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
        currAmount = maxAmount;
    }

    public void AddTentative(int toAdd) //Add the amount of resources being brought by queued resources, in an effort to prevent the container being queued when it is set to be filled
    {
        tentativeAmount += toAdd;
        if (tentativeAmount > maxAmount) tentativeAmount = maxAmount;
    }
}
