using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public string resourceType;
    public int minAmount;
    public int maxAmount;
    [HideInInspector]
    public bool isGrabbed = false;

    [SerializeField]
    private int amount;
    private Container target;

    private void Awake()
    {
        amount = Random.Range(minAmount, maxAmount + 1);
        target = GridManager.GetClosestValidContainer(this, resourceType);
        if (target != null) GridManager.AddtoLowQueue(new Order("Transport", target, this));
        else Debug.Log("No valid container of type: " + resourceType);
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null) // might want to move this somewhere else, but that's a future problem
        {
            target = GridManager.GetClosestValidContainer(this, resourceType);
            if (target != null) GridManager.AddtoLowQueue(new Order("Transport", target, this));
            else Debug.Log("Still no valid container of type: " + resourceType);
        }
    }

    public int GetAmount()
    {
        return amount;
    }

    public void SubtractAmount(int toSubtract)
    {
        amount -= toSubtract;
    }
}
