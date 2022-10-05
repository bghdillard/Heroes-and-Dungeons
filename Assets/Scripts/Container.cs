using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public string type;
    public int maxAmount;
    [HideInInspector]
    public int currAmount = 0;

    public void AddResources(int toAdd) //Add the given amount of resources to the container
    {
        currAmount += toAdd;
    }

    public bool CheckAmount(int toCheck) //Check if the container can hold the given amount of resources
    {
        return currAmount + toCheck > maxAmount;
    }

    public int GetDifference(int toCheck) //Get by how much the given amount of resources + the current amount of held resources is over the limit by
    {
        return currAmount + toCheck - maxAmount;
    }
}
